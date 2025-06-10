using WebApp.Services;
using GamePlatformBL.Utilities;
using GamePlatformBL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace WebApp.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly ApiService _apiService;

        public AccountController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public IActionResult Index()
        {
            //ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Manage()
        {
            // Get current user info
            var userInfo = await _apiService.GetAsync<SimpleUserViewModel>("User/whoami");
            if (userInfo == null || userInfo.Id <= 0)
            {
                TempData["Error"] = "You must be logged in to manage your account.";
                return RedirectToAction("Index");
            }

            var user = await _apiService.GetAsync<EditUserViewModel>($"User/GetUserById?id={userInfo.Id}");
            DebugHelper.AppPrintDebugMessage($"Got the user with ID: {user.Id}");
            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction("Index");
            }

            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Manage(EditUserViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _apiService.PutWithResponseAsync($"User/Update/{model.Id}", model);

            if (result.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Your account has been updated.";

                var updatedUser = await _apiService.GetAsync<EditUserViewModel>($"User/GetUserById?id={model.Id}");

                if (updatedUser != null)
                    HttpContext.Session.SetString("Username", updatedUser.Username);

                return RedirectToAction("Index");
            }

            ModelState.AddModelError("", "Update failed. Please try again.");
            return View(model);
        }

        public async Task<IActionResult> Reviews()
        {
            var userInfo = await _apiService.GetAsync<SimpleUserViewModel>("User/whoami");

            if (userInfo == null) return Unauthorized();

            var reviews = await _apiService.GetAsync<List<UserReviewViewModel>>($"GameReview/GetAllReviewsForUser?userId={userInfo.Id}");
            return View(reviews);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteReview(int id, int userId)
        {
            var response = await _apiService.DeleteAsync($"GameReview/DeleteReview?gameId={id}&userId={userId}");

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Review deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Could not delete review.";
            }

            return RedirectToAction("Reviews");
        }

    }
}
