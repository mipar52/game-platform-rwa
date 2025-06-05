// CONTROLLER: AdminGameTypeController.cs
using Microsoft.AspNetCore.Mvc;
using WebApp.Services;
using WebApp.Utilities;
using WebApp.ViewModels;

namespace WebApp.Controllers.Admin
{
    public class AdminGameTypeController : Controller
    {
        private readonly ApiService _apiService;

        public AdminGameTypeController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            var gameTypes = await _apiService.GetAsync<List<GameTypeViewModel>>("GameType/GetAllGameTypes");
            return View(gameTypes);
        }

        public async Task<IActionResult> Create(int? id)
        {
            if (id == null)
                return View(new GameTypeViewModel());

            var gameType = await _apiService.GetAsync<GameTypeViewModel>($"GameType/GetGameTypeById?id={id}");
            return View(gameType);
        }

        [HttpPost]
        public async Task<IActionResult> Create(GameTypeViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            HttpResponseMessage response = model.Id == 0
                ? await _apiService.PostAsync("GameType/CreateGameType", model)
                : await _apiService.PutWithResponseAsync($"GameType/UpdateGameType?id={model.Id}", model);

            DebugHelper.PrintDebugMessage($"GameType action status: {response}");

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