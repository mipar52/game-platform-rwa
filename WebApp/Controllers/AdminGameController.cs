using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using WebApp.Services;
using WebApp.ViewModels;

namespace WebApp.Controllers
{
    public class AdminGameController : Controller
    {
        private readonly ApiService _apiService;

        public AdminGameController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> Index()
        {
            var games = await _apiService.GetAsync<List<AdminGameViewModel>>("Game/GetAllGames");
            return View(games);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.GameTypes = await _apiService.GetAsync<List<GameTypeViewModel>>("GameType/GetAllGameTypes");
            ViewBag.Genres = await _apiService.GetAsync<List<GameGenreViewModel>>("GameGenre/GetAllGenres");

            var viewModel = new AdminGameViewModel();
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(AdminGameViewModel model)
        {
            if (!ModelState.IsValid)
            {

                ViewBag.GameTypes = await _apiService.GetAsync<List<GameTypeViewModel>>("GameType/GetAllGameTypes");
                ViewBag.Genres = await _apiService.GetAsync<List<GameGenreViewModel>>("GameGenre/GetAllGenres");
                model.GameTypeId = model.GameType.Id;
                model.genreIds = model.Genres.Select(x => x.Id);
                return View(model);
            }

            var response = await _apiService.PostAsync("Game/CreateGame", model);
            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Game successfully created.";
                return RedirectToAction("Index");
            }

            TempData["ErrorMessage"] = "Failed to create game.";
            return View(model);
        }

        public async Task<IActionResult> Update(int id)
        {
            var game = await _apiService.GetAsync<AdminGameViewModel>($"Game/GetGameById?id={id}");
            if (game == null)
            {
                return NotFound();
            }

            ViewBag.GameTypes = await _apiService.GetAsync<List<GameTypeViewModel>>("GameType/GetAllGameTypes");
            ViewBag.Genres = await _apiService.GetAsync<List<GameGenreViewModel>>("GameGenre/GetAllGenres");
            return View("Create", game);
        }

        [HttpPost]
        public async Task<IActionResult> Update(AdminGameViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.GameTypes = await _apiService.GetAsync<List<GameTypeViewModel>>("GameType/GetAll");
                ViewBag.Genres = await _apiService.GetAsync<List<GameGenreViewModel>>("Genre/GetAll");

                return View(model);
            }

            model.GameTypeId = model.GameType.Id;
            model.genreIds = model.Genres.Select(x => x.Id);
            var response = await _apiService.PostAsync($"Game/UpdateGame?id={model.Id}", model);
            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Game successfully updated.";
                return RedirectToAction("Index");
            }

            TempData["ErrorMessage"] = "Failed to update game.";
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Save(AdminGameViewModel model, int SelectedGameTypeId, List<int> SelectedGenreIds)
        {
            model.GameType = new GameTypeViewModel { Id = SelectedGameTypeId };
            model.Genres = SelectedGenreIds.Select(id => new GameGenreViewModel { Id = id }).ToList();
            model.GameTypeId = SelectedGameTypeId;
            model.genreIds = SelectedGenreIds;

            if (model.Id == 0)
               await _apiService.PostAsync("Game/CreateGame", model);
            else
                await _apiService.PutWithResponseAsync($"Game/UpdateGame?id={model.Id}", model);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _apiService.DeleteAsync($"Game/DeleteGame?id={id}");
            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Game successfully deleted.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to delete game.";
            }

            return RedirectToAction("Index");
        }
    }
}
