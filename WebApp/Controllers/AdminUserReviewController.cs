using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration.UserSecrets;
using WebApp.Services;
using WebApp.Utilities;
using WebApp.ViewModels;

namespace WebApp.Controllers
{
    public class AdminUserReviewController : Controller
    {
        private readonly ApiService _apiService;

        public AdminUserReviewController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            var reviews = await _apiService.GetAsync<List<AdminGameReviewViewModel>>("GameReview/GetAllReviews");
            return View(reviews);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int gameId, int userId)
        {
            await _apiService.DeleteAsync($"GameReview/DeleteReview?gameId={gameId}&userId={userId}");
            
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> ToggleApproval(int gameId, int userId, AdminGameReviewViewModel model)
        {
            var response = await _apiService.PostAsync($"GameReview/ApproveReview?gameId={gameId}&userId={userId}", model);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            } else
            {
                DebugHelper.PrintDebugMessage($"ToggleApproval Response: {response.StatusCode}");
                TempData["Error"] = "Failed to toggle approval.";
                return RedirectToAction("Index");
            }

        }
    }
}
