// GameDetailsController.cs
using Microsoft.AspNetCore.Mvc;
using WebApp.Services;
using WebApp.Utilities;
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
            var userInfo = await _apiService.GetAsync<SimpleUserViewModel>("User/whoami");
            DebugHelper.PrintDebugMessage($"WHOAMI: ID: {userInfo.Id}, Username: {userInfo.Username}, Role: {userInfo.Role}");
            ViewBag.UserId = userInfo?.Id ?? 0;
            return View(game);
        }

        [HttpPost]
        public async Task<IActionResult> AddComment(CreateReviewViewModel viewModel)
        {
            // Optional: validate again
            if (viewModel.Rating < 1 || viewModel.Rating > 10)
            {
                TempData["Error"] = "Invalid rating.";
                return RedirectToAction("Index", new { id = viewModel.GameId });
            }

            DebugHelper.PrintDebugMessage($"UserID posted a review: {viewModel.UserId}");
            var response = await _apiService.PostAsync("GameReview/CreateReview", viewModel);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Review submitted successfully!";
            }
            else
            {
                DebugHelper.PrintDebugMessage($"Create review error: {response.StatusCode}");
                TempData["Error"] = "Failed to submit review.";
            }

            return RedirectToAction("Index", new { id = viewModel.GameId });
        }
    }
}
