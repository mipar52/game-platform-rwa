using game_platform_rwa.DTO_generator;
using game_platform_rwa.DTOs;
using game_platform_rwa.Logger;
using game_platform_rwa.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

        [Authorize(Roles = "Admin")]
        [HttpPost("[action]")]
        public IActionResult ApproveReview(int gameId, int userId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var review = context.Reviews.FirstOrDefault(r => r.GameId == gameId && r.UserId == userId);
                if (review == null)
                {
                    logService.Log($"Review with gameid={gameId} not found", "No results");
                    return NotFound("Review not found.");
                }

                review.Approved = true;
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
        public IActionResult UpdateReview(int gameId, int userId, [FromBody] GameReviewDto updated)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                var review = context.Reviews.FirstOrDefault(r => r.GameId == gameId && r.UserId == userId);
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
        public IActionResult DeleteReview(int gameId, int userId)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            try
            {
                var review = context.Reviews.FirstOrDefault(r => r.GameId == gameId && r.UserId == userId);
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
