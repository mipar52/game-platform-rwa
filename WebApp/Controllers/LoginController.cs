using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;
using GamePlatformBL.ViewModels;
using WebApp.Services;
using GamePlatformBL.Utilities;
using AutoMapper;
using GamePlatformBL.DTOs;

namespace WebApp.Controllers
{
    public class LoginController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly ApiService _apiService;
        private readonly IMapper _mapper;
        public LoginController(ApiService apiService, IHttpClientFactory clientFactory, IMapper mapper)
        {
            _apiService = apiService;
            _clientFactory = clientFactory;
            _mapper = mapper;
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
            DebugHelper.AppPrintDebugMessage($"Login Status: {response.IsSuccessStatusCode}");

            if (response.IsSuccessStatusCode)
            {
                var token = await response.Content.ReadAsStringAsync();
                HttpContext.Session.SetString("JwtToken", token);
                HttpContext.Session.SetString("Username", loginModel.Username);
                return RedirectToAction("Index", "Home");

            }
            TempData["Error"] = "Invalid username or password.";
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

            var dto = _mapper.Map<UserDto>(model);
            HttpResponseMessage response = await _apiService.PostAsync("User/RegisterUser", dto);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Registration successful! You can now log in.";
                return RedirectToAction("Index");
            }

            TempData["Error"] = $"Failed to save user. Reason: {response.StatusCode}";
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

