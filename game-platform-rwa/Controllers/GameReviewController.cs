using game_platform_rwa.DTO_generator;
using game_platform_rwa.DTOs;
using game_platform_rwa.Logger;
using game_platform_rwa.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace game_platform_rwa.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class GameReviewController : ControllerBase
    {
        private readonly GamePlatformRwaContext context;
        private readonly LogService logService;

        public GameReviewController(GamePlatformRwaContext context, LogService logService)
        {
            this.context = context;
            this.logService = logService;
        }

        [HttpGet("GetAllReviews")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetAllReviews()
        {
            try
            {
                var reviews = context.Reviews
                    .Include(r => r.Game)
                    .Include(r => r.User)
                    .ToList();

                if (!reviews.Any())
                {
                    logService.Log("No reviews found.", "No results");
                    return NotFound("No reviews found.");
                }

                var result = reviews.Select(GameDTOGenerator.generateGameReviewDto).ToList();

                logService.Log($"Retrieved all {result.Count} reviews.", "Success");
                return Ok(result);
            }
            catch (Exception ex)
            {
                logService.Log($"Error retrieving all reviews: {ex.Message}", "Error");
                return StatusCode(500, ex.Message);
            }
        }


        [HttpGet("[action]")]
        public ActionResult<IEnumerable<Review>> GetAllReviewsForGame(int gameId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var reviews = context.Reviews
                    .Where(r => r.GameId == gameId)
                    .ToList();

                if (!reviews.Any())
                {
                    logService.Log($"No reviews found for game ID {gameId}", "No results");
                    return NotFound($"No reviews found for game ID {gameId}");

                }
                var result = reviews.Select(x => GameDTOGenerator.generateGameReviewDto(x));
                logService.Log($"Found {result.Count()} for game with id={gameId}", "Success");
                return Ok(result);
            }
            catch (Exception ex)
            {
                logService.Log($"Could not get reviews for game with id={gameId}. Error {ex.Message}", "Error");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("[action]")]
        public ActionResult<IEnumerable<UserGameReviewDto>> GetAllReviewsForUser(int userId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Get all reviews for user including game info in a single query
                var reviews = context.Reviews
                    .Include(r => r.Game)
                    .Where(r => r.UserId == userId)
                    .ToList();

                if (!reviews.Any())
                {
                    logService.Log($"No reviews found for user ID {userId}", "No results");
                    return NotFound($"No reviews found for user ID {userId}");
                }

                // Map to DTO and assign game name directly
                var gameReviews = reviews.Select(r => GameDTOGenerator.generateUserGameReviewDto(r));

                logService.Log($"Found {gameReviews.Count()} reviews for user with id={userId}", "Success");
                return Ok(gameReviews);
            }
            catch (Exception ex)
            {
                logService.Log($"Could not get reviews for user with id={userId}. Error {ex.Message}", "Error");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("[action]")]
        public IActionResult CreateReview([FromBody] GameReviewCreateDto review)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var newReview = new Review
                {
                    UserId = review.UserId,
                    GameId = review.GameId,
                    Rating = review.Rating,
                    ReviewText = review.ReviewText,
                    Approved = false,
                    CreatedAt = DateTime.UtcNow
                };
                context.Reviews.Add(newReview);

                var game = context.Games.FirstOrDefault(g => g.Id == review.GameId);
                if (game == null)
                {
                    logService.Log($"Game with id={review.GameId} not found", "No results");
                    return NotFound("Game not found.");
                }

                game.Reviews.Add(newReview);

                var user = context.Users.FirstOrDefault(u => u.Id == review.UserId);
                if (user == null)
                {
                    logService.Log($"User with id={review.UserId} not found", "No results");
                    return NotFound("User not found.");
                }
                user.Reviews.Add(newReview);

                context.SaveChanges();
                logService.Log($"Created a new review for game with id={review.GameId}", "Success");
                return CreatedAtAction(nameof(GetAllReviewsForGame), new { gameId = review.GameId }, review);
            }
            catch (Exception ex)
            {
                logService.Log($"Could not create a new review for game with id={review.GameId}. Error: {ex.Message}", "Error");
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("[action]")]
        public IActionResult ApproveReview(int gameId, int userId, int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var review = context.Reviews.FirstOrDefault(r => r.GameId == gameId && r.UserId == userId && r.Id == id);
                if (review == null)
                {
                    logService.Log($"Review with gameid={gameId} not found", "No results");
                    return NotFound("Review not found.");
                }

                if (review.Approved == null)
                {
                    review.Approved = true;
                } else
                {
                    review.Approved = review.Approved == true ? false : true;
                }


                    context.SaveChanges();

                logService.Log($"Review with gameid={gameId}, for user={userId} approved", "Success");
                return Ok("Review approved.");
            } catch (Exception ex)
            {
                logService.Log($"Could not approve review with gameId={gameId}, from user={userId}. Error: {ex.Message}", "Error");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("[action]")]
        public IActionResult UpdateReview(int gameId, int userId, int id, [FromBody] GameReviewCreateDto updated)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                var review = context.Reviews.FirstOrDefault(r => r.GameId == gameId && r.UserId == userId && r.Id == id);
                if (review == null)
                {
                    logService.Log($"Could not update the review with gameId={gameId}. It could be found.", "No results");
                    return NotFound("Review not found.");
                }

                review.Rating = updated.Rating;
                review.ReviewText = updated.ReviewText;
                review.Approved = updated.Approved;

                context.SaveChanges();
                logService.Log($"Updated the review with gameId={gameId}.", "Success");
                return Ok("Review updated.");
            } catch (Exception ex)
            {
                logService.Log($"Could not update the review for gameId={gameId}. Error: {ex.Message}", "Error");
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("[action]")]
        public IActionResult DeleteReview(int gameId, int userId, int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                var review = context.Reviews.FirstOrDefault(r => r.GameId == gameId && r.UserId == userId && r.Id == id);
                if (review == null)
                {
                    logService.Log($"Could not update the review with gameId={gameId}. It could be found.", "No results");
                    return NotFound("Review not found.");
                }

                context.Reviews.Remove(review);
                context.SaveChanges();

                logService.Log($"Deleted the review with gameId={gameId}.", "Success");
                return NoContent();
            } catch (Exception ex)
            {
                logService.Log($"Could not delete the review for gameId={gameId}. Error: {ex.Message}", "Error");
                return StatusCode(500, ex.Message);
            }
        }


    }
}
