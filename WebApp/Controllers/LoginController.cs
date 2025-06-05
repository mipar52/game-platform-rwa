using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;
using WebApp.ViewModels;
using WebApp.Utilities;
using WebApp.Services;

namespace WebApp.Controllers
{
    public class LoginController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly ApiService _apiService;

        public LoginController(ApiService apiService, IHttpClientFactory clientFactory)
        {
            _apiService = apiService;
            _clientFactory = clientFactory;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(LoginUserViewModel loginModel)
        {
            if (!ModelState.IsValid)
                return View(loginModel);

            var client = _clientFactory.CreateClient();
            var json = JsonSerializer.Serialize(loginModel, new JsonSerializerOptions { PropertyNamingPolicy = null });
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("http://localhost:5062/api/User/Login", content);
            DebugHelper.PrintDebugMessage($"Login Status: {response.IsSuccessStatusCode}");

            if (response.IsSuccessStatusCode)
            {
                var token = await response.Content.ReadAsStringAsync();
                HttpContext.Session.SetString("JwtToken", token);
                HttpContext.Session.SetString("Username", loginModel.Username);
                return RedirectToAction("Index", "Home");

            }

            ModelState.AddModelError(string.Empty, "Invalid username or password.");
            return View(loginModel);
        }

        // GET: Login/RegisterUser
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(UserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            HttpResponseMessage response = await _apiService.PostAsync("User/RegisterUser", model);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Registration successful! You can now log in.";
                return RedirectToAction("Index");
            }

            TempData["Error"] = "Failed to save user.";
            return View(model);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

    }
}

