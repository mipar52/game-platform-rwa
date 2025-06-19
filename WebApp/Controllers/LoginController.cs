using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;
using GamePlatformBL.ViewModels;
using WebApp.Services;
using AutoMapper;
using GamePlatformBL.DTOs;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using GamePlatformBL.Utilities;

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
        public IActionResult Index(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginUserViewModel
            {
                ReturnUrl = returnUrl
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(LoginUserViewModel loginModel)
        {
            if (!ModelState.IsValid)
                return View(loginModel);

            var client = _clientFactory.CreateClient();
            var json = JsonSerializer.Serialize(loginModel, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.KebabCaseLower });
            DebugHelper.AppPrintDebugMessage($"Login JSON: {json}");
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("http://localhost:5062/api/User/Login", content);

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();

                var loginResponse = JsonSerializer.Deserialize<LoginResponseViewModel>(responseBody, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (loginResponse != null)
                {
                    

                    var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, loginModel.Username),
        new Claim(ClaimTypes.Role, loginResponse.Role),
        new Claim("JwtToken", loginResponse.Token)
    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties { IsPersistent = true };

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties
                    );

                    if( !string.IsNullOrEmpty(loginModel.ReturnUrl) && Url.IsLocalUrl(loginModel.ReturnUrl))
                    {
                        return LocalRedirect(loginModel.ReturnUrl);
                    }

                    if (loginResponse.Role == "Admin")
                    {
                        return RedirectToAction("Index", "Admin");
                    }
                    return RedirectToAction("Index", "Home");
                }

            }
            DebugHelper.AppPrintDebugMessage($"Login error: {response.StatusCode} - {response.RequestMessage}");
            TempData["Error"] = "Invalid username or password.";
            return View(loginModel);
        }

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
                return View(model);

            var dto = _mapper.Map<UserDto>(model);
            HttpResponseMessage response = await _apiService.PostAsync("User/RegisterUser", dto);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Registration successful! You can now log in.";
                return RedirectToAction("Index");
            } else
            {
                var errorMessage = await response.Content.ReadAsStringAsync();
                TempData["ErrorMessage"] = $"Issue with registration! Reason: {errorMessage}";
                return View(model);
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("LoggedOut");
        }

        [HttpGet]
        public IActionResult LoggedOut()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Forbidden()
        {
            return View();
        }

    }
}
