using game_platform_rwa.Models;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace game_platform_rwa.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameReviewController : ControllerBase
    {
        private readonly GamePlatformRwaContext context;

        public GameReviewController(GamePlatformRwaContext context)
        {
            this.context = context;
        }

        [HttpGet("[action]")]
        public ActionResult<IEnumerable<Review>> GetAllReviewsForGame(int gameId)
        {
            var reviews = context.Reviews
                .Where(r => r.GameId == gameId)
                .ToList();

            if (!reviews.Any())
                return NotFound($"No reviews found for game ID {gameId}");

            return Ok(reviews);
        }

        [HttpPost("[action]")]
        public IActionResult ApproveReview(int gameId, int userId)
        {
            var review = context.Reviews.FirstOrDefault(r => r.GameId == gameId && r.UserId == userId);
            if (review == null)
                return NotFound("Review not found.");

            review.Approved = true;
            context.SaveChanges();

            return Ok("Review approved.");
        }


    }
}
