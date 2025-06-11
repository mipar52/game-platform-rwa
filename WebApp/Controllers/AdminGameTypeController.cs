// CONTROLLER: AdminGameTypeController.cs
using WebApp.Services;
using GamePlatformBL.Utilities;
using GamePlatformBL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using GamePlatformBL.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace WebApp.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    public class AdminGameTypeController : Controller
    {
        private readonly ApiService _apiService;
        private readonly IMapper _mapper;
        public AdminGameTypeController(ApiService apiService, IMapper mapper)
        {
            _apiService = apiService;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var gameTypes = _mapper.Map<List<GameTypeViewModel>>(await _apiService.GetAsync<List<GameTypeDto>>("GameType/GetAllGameTypes"));
            return View(gameTypes);
        }

        public async Task<IActionResult> Create(int? id)
        {
            if (id == null)
                return View(new GameTypeViewModel());

            var gameType = _mapper.Map<GameTypeViewModel>(await _apiService.GetAsync<GameTypeDto>($"GameType/GetGameTypeById?id={id}"));
            return View(gameType);
        }

        [HttpPost]
        public async Task<IActionResult> Create(GameTypeViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var dto = _mapper.Map<GameTypeDto>(model);
            HttpResponseMessage response = model.Id == 0
                ? await _apiService.PostAsync("GameType/CreateGameType", dto)
                : await _apiService.PutWithResponseAsync($"GameType/UpdateGameType?id={dto.Id}", dto);

            DebugHelper.AppPrintDebugMessage($"GameType action status: {response}, message: {response.RequestMessage}");

            if (response.IsSuccessStatusCode)
                return RedirectToAction("Index");

            TempData["Error"] = "Failed to save game type.";
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _apiService.DeleteAsync($"GameType/DeleteGameType?id={id}");
            return RedirectToAction("Index");
        }
    }
}