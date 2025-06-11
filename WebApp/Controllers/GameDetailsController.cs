// GameDetailsController.cs
using WebApp.Services;
using GamePlatformBL.Utilities;
using GamePlatformBL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using GamePlatformBL.DTOs;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;

namespace WebApp.Controllers
{
    [Authorize]
    public class GameDetailsController : Controller
    {
        private readonly ApiService _apiService;
        private readonly IMapper _mapper;
        public GameDetailsController(ApiService apiService, IMapper mapper)
        {
            _apiService = apiService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int id)
        {
            var game = await _apiService.GetAsync<GameDto>($"Game/GetGameById?id={id}");
            var result = _mapper.Map<GameDetailsViewModel>(game);
            var userInfo = await _apiService.GetAsync<SimpleUserViewModel>("User/whoami");
            DebugHelper.AppPrintDebugMessage($"WHOAMI: ID: {userInfo.Id}, Username: {userInfo.Username}, Role: {userInfo.Role}");
            ViewBag.UserId = userInfo?.Id ?? 0;
            return View(result);
        }

        [HttpPost]
        public async Task<IActionResult> AddComment(CreateReviewViewModel viewModel)
        {
            if (viewModel.Rating < 1 || viewModel.Rating > 10)
            {
                TempData["Error"] = "Invalid rating.";
                return RedirectToAction("Index", new { id = viewModel.GameId });
            }

            DebugHelper.AppPrintDebugMessage($"UserID posted a review: {viewModel.UserId}");
            var response = await _apiService.PostAsync("GameReview/CreateReview", viewModel);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Review submitted successfully! The platform administrator needs to approve it first, and then it will be visible here. :)";
            }
            else
            {
                DebugHelper.AppPrintDebugMessage($"Create review error: {response.StatusCode}");
                TempData["Error"] = "Failed to submit review.";
            }

            return RedirectToAction("Index", new { id = viewModel.GameId });
        }
    }
}
