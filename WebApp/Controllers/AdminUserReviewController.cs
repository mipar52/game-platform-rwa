using WebApp.Services;
using GamePlatformBL.Utilities;
using GamePlatformBL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration.UserSecrets;
using Microsoft.AspNetCore.Authorization;

namespace WebApp.Controllers
{
    [Authorize(Roles = "Admin")]

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
        public async Task<IActionResult> Delete(int gameId, int userId, int id)
        {
            await _apiService.DeleteAsync($"GameReview/DeleteReview?gameId={gameId}&userId={userId}&Id={id}");
            
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> ToggleApproval(int gameId, int userId, int id, AdminGameReviewViewModel model)
        {
            var response = await _apiService.PostAsync($"GameReview/ApproveReview?gameId={gameId}&userId={userId}&id={id}", model);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            } else
            {
                DebugHelper.AppPrintDebugMessage($"ToggleApproval Response: {response.StatusCode}");
                TempData["Error"] = "Failed to toggle approval.";
                return RedirectToAction("Index");
            }

        }
    }
}
