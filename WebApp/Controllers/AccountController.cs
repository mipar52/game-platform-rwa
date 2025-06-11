using WebApp.Services;
using GamePlatformBL.Utilities;
using GamePlatformBL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GamePlatformBL.DTOs;

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
        public async Task<IActionResult> Manage([FromBody] EditUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                DebugHelper.AppPrintDebugMessage($"User new info model state: {ModelState}");
                return BadRequest(ModelState);
            }
                

            DebugHelper.AppPrintDebugMessage($"User new info: {model}");
            var result = await _apiService.PutWithResponseAsync($"User/Update/{model.Id}", model);

            if (result.IsSuccessStatusCode)
            {
                var updatedUser = await _apiService.GetAsync<EditUserViewModel>($"User/GetUserById?id={model.Id}");
                if (updatedUser != null)
                    HttpContext.Session.SetString("Username", updatedUser.Username);

                return Ok(new { message = "Updated successfully" });
            }

            return BadRequest(new { error = "Update failed." });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                DebugHelper.AppPrintDebugMessage($"Change password model state: {ModelState}");
                return BadRequest("Invalid model.");
            }
                

            DebugHelper.AppPrintDebugMessage($"Current: {model.CurrentPassword}, new: {model.NewPassword}");
            if (model.NewPassword != model.CurrentPassword)
                return BadRequest("Passwords do not match.");

            var response = await _apiService.PostAsync("User/ChangePassword", model);

            if (response.IsSuccessStatusCode)
                return Ok("Password updated successfully.");

            var errorMsg = await response.Content.ReadAsStringAsync();
            return BadRequest(errorMsg);
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
