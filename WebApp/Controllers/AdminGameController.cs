using WebApp.Services;
using GamePlatformBL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using GamePlatformBL.DTOs;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;

namespace WebApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminGameController : Controller
    {
        private readonly ApiService _apiService;
        private readonly IMapper _mapper;
        public AdminGameController(ApiService apiService, IMapper mapper)
        {
            _apiService = apiService;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var games = await _apiService.GetAsync<List<SimpleGameDto>>("Game/GetAllGames");
            var result = _mapper.Map<List<AdminGameViewModel>>(games);
            return View(result);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.GameTypes = _mapper.Map<List<GameTypeViewModel>>(await _apiService.GetAsync<List<GameTypeDto>>("GameType/GetAllGameTypes"));
            ViewBag.Genres = _mapper.Map<List<GenreViewModel>>(await _apiService.GetAsync<List<GenreDto>>("GameGenre/GetAllGenres"));

            var viewModel = new AdminGameViewModel();
            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(AdminGameViewModel model)
        {
            if (!ModelState.IsValid)
            {

                ViewBag.GameTypes = _mapper.Map<List<GameTypeViewModel>>(await _apiService.GetAsync<List<GameTypeDto>>("GameType/GetAllGameTypes"));
                ViewBag.Genres = _mapper.Map<List<GenreViewModel>>(await _apiService.GetAsync<List<GenreDto>>("GameGenre/GetAllGenres"));
                model.GameTypeId = model.GameType.Id;
                model.genreIds = model.GameGenres.Select(x => x.Genre.Id);
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

            ViewBag.GameTypes = _mapper.Map<List<GameTypeViewModel>>(await _apiService.GetAsync<List<GameTypeDto>>("GameType/GetAllGameTypes"));
            ViewBag.Genres = _mapper.Map<List<GenreViewModel>>(await _apiService.GetAsync<List<GenreDto>>("GameGenre/GetAllGenres"));
            return View("Create", game);
        }

        [HttpPost]
        public async Task<IActionResult> Update(AdminGameViewModel model)
        {
            if (!ModelState.IsValid)
            {

                ViewBag.GameTypes = _mapper.Map<List<GameTypeViewModel>>(await _apiService.GetAsync<List<GameTypeDto>>("GameType/GetAllGameTypes"));
                ViewBag.Genres = _mapper.Map<List<GenreViewModel>>(await _apiService.GetAsync<List<GenreDto>>("GameGenre/GetAllGenres"));

                return View(model);
            }

            model.GameTypeId = model.GameType.Id;
            model.genreIds = model.GameGenres.Select(x => x.Genre.Id);
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
         //   model.Genres = SelectedGenreIds.Select(id => new GameGenreViewModel { Genre.Id = id }).ToList(); 
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
