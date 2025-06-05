// Controller: GamesCategoryController.cs
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Services;
using System.Threading.Tasks;
using System.Collections.Generic;
using WebApp.ViewModels;
using WebApp.Utilities;
using Azure.Core;

namespace WebApp.Controllers
{
    public class GameTypeController : Controller
    {
        private readonly ApiService _apiService;

        public GameTypeController(ApiService apiService)
        {
            _apiService = apiService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var gameTypes = await _apiService.GetAsync<List<GameTypeViewModel>>("GameType/GetAllGameTypes");
            var genres = await _apiService.GetAsync<List<Genre>>("GameGenre/GetAllGenres");
            DebugHelper.PrintDebugMessage($"Got game types: {gameTypes}, and game genres: {genres}");
            var viewModel = new GameTypeGenreSelectionViewModel
            {
                GameTypes = gameTypes.Select(gt => new GameTypeViewModel { Id = gt.Id, Name = gt.Name }).ToList(),
                GameGenres = genres.Select(g => new GameGenreViewModel { Id = g.Id, Name = g.Name}).ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Browse(GameSearchRequestViewModel request)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Invalid form submission.";
                return RedirectToAction("Index");
            }
            string url;
            DebugHelper.PrintDebugMessage($"Selected genres: {request.SelectedGenreIds}");
            if (request.SelectedGenreIds == null || request.SelectedGenreIds.Count == 0)
            {
                url = $"GameType/GetGamesWithGameType?id={request.GameTypeId}";
            } else
            {
                var genreQuery = string.Join("&genreIds=", request.SelectedGenreIds);
                url = $"Game/GetGamesByTypeAndGenres?gameTypeId={request.GameTypeId}&genreIds={genreQuery}";
            }

            var games = await _apiService.GetAsync<List<GameViewModel>>(url);

            if (games == null || games.Count == 0)
            {
                TempData["Error"] = "Looks like you are too picky! No games found, choose another filter!";
                return RedirectToAction("Index", "GameType");
            }

            return View("GameList", games);
        }
    }
    }
