// GameDetailsController.cs
using Microsoft.AspNetCore.Mvc;
using WebApp.Services;
using WebApp.ViewModels;

namespace WebApp.Controllers
{
    public class GameDetailsController : Controller
    {
        private readonly ApiService _apiService;

        public GameDetailsController(ApiService apiService)
        {
            _apiService = apiService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int id)
        {
            var game = await _apiService.GetAsync<GameDetailsViewModel>($"Game/GetGameById?id={id}");
            return View(game);
        }

        [HttpPost]
        public async Task<IActionResult> AddComment(int GameId, string ReviewText, int Rating)
        {
            var review = new
            {
                GameId,
                UserId = 1, // TODO: Replace with actual logged-in user ID from session
                Rating,
                ReviewText,
                CreatedAt = DateTime.UtcNow
            };

          //  await _apiService.PostAsync("Review/CreateReview", review);
            return RedirectToAction("Index", new { id = GameId });
        }
    }
}
