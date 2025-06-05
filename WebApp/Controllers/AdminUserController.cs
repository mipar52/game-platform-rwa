using Microsoft.AspNetCore.Mvc;
using WebApp.Services;
using WebApp.ViewModels;

namespace WebApp.Controllers
{
    public class AdminUserController : Controller
    {

            private readonly ApiService _apiService;

            public AdminUserController(ApiService apiService)
            {
                _apiService = apiService;
            }

            public async Task<IActionResult> Index()
            {
            var users = await _apiService.GetAsync<List<UserViewModel>>("User/GetAllUsers");

            foreach (var user in users)
            {
                var fullUser = await _apiService.GetAsync<UserViewModel>($"User/GetUserById?id={user.Id}");
                user.RoleId = fullUser.RoleId; // or assign `user.Role = fullUser.Role` if you need the full object
            }

            return View(users);
        }

            public async Task<IActionResult> Create(int? id)
            {
                if (id == null)
                    return View(new UserViewModel());

                var user = await _apiService.GetAsync<UserViewModel>($"User/GetUserById?id={id}");
                return View(user);
            }

            [HttpPost]
            public async Task<IActionResult> Create(UserViewModel model)
            {
                if (!ModelState.IsValid) return View(model);

                HttpResponseMessage response = model.Id == 0
                    ? await _apiService.PostAsync("User/RegisterUser", model)
                    : await _apiService.PutWithResponseAsync($"User/Update/{model.Id}", model);

                if (response.IsSuccessStatusCode)
                    return RedirectToAction("Index");

                TempData["Error"] = "Failed to save user.";
                return View(model);
            }

            [HttpPost]
            public async Task<IActionResult> Delete(int id)
            {
                await _apiService.DeleteAsync($"User/Delete/{id}");
                return RedirectToAction("Index");
            }

        [HttpPost]
        public async Task<IActionResult> Promote(string username)
        {
            var response = await _apiService.PostAsync($"User/PromoteUser?username={username}", username);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = $"Role changed for user {username}";
            }
            else
            {
                TempData["ErrorMessage"] = $"Failed to change role for user {username}";
            }

            return RedirectToAction("Index");
        }

    }
}
