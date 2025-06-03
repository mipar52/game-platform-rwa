using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;
using WebApp.ViewModels;
using WebApp.Utilities;

namespace WebApp.Controllers
{
    public class LoginController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;

        public LoginController(IHttpClientFactory clientFactory)
        {
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
    }
}

