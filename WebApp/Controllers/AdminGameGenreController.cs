using Microsoft.AspNetCore.Mvc;
using WebApp.Services;
using WebApp.Utilities;
using WebApp.ViewModels;

namespace WebApp.Controllers
{
    public class AdminGameGenreController : Controller
    {
        private readonly ApiService _apiService;

        public AdminGameGenreController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            var gameTypes = await _apiService.GetAsync<List<GameGenreViewModel>>("GameGenre/GetAllGenres");
            return View(gameTypes);
        }

        public async Task<IActionResult> Create(int? id)
        {
            if (id == null)
                return View(new GameGenreViewModel());

            var gameType = await _apiService.GetAsync<GameGenreViewModel>($"GameGenre/GetGenreById?id={id}");
            return View(gameType);
        }

        [HttpPost]
        public async Task<IActionResult> Create(GameGenreViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            HttpResponseMessage response = model.Id == 0
                ? await _apiService.PostAsync("GameGenre/CreateGenre", model)
                : await _apiService.PutWithResponseAsync($"GameGenre/UpdateGenre?id={model.Id}", model);

            DebugHelper.PrintDebugMessage($"GameGenre action status: {response}");

            if (response.IsSuccessStatusCode)
                return RedirectToAction("Index");

            TempData["Error"] = "Failed to save game genre.";
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _apiService.DeleteAsync($"GameGenre/DeleteGenre?id={id}");
            return RedirectToAction("Index");
        }
    }
}
