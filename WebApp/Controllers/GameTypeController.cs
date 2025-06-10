// Controller: GamesCategoryController.cs
using Microsoft.AspNetCore.Mvc;
using GamePlatformBL.DTOs;
using GamePlatformBL.Utilities;
using GamePlatformBL.ViewModels;
using WebApp.Services;
using AutoMapper;
using GamePlatformBL.Models;

namespace WebApp.Controllers
{
    public class GameTypeController : Controller
    {
        private readonly ApiService _apiService;
        private readonly IMapper _mapper;
        public GameTypeController(ApiService apiService, IMapper mapper)
        {
            _apiService = apiService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var gameTypes = await _apiService.GetAsync<List<GameTypeDto>>("GameType/GetAllGameTypes");
            var genres = await _apiService.GetAsync<List<GenreDto>>("GameGenre/GetAllGenres");

            DebugHelper.AppPrintDebugMessage($"Got game types: {gameTypes}, and game genres: {genres}");
            var viewModel = new GameTypeGenreSelectionViewModel
            {
                GameTypes =_mapper.Map<List<GameTypeViewModel>>(gameTypes),
                Genres = _mapper.Map<List<GenreViewModel>>(genres),
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
            DebugHelper.AppPrintDebugMessage($"Selected genres: {request.SelectedGenreIds}, Selected gametypes: {request.GameTypeId}");
            if (request.SelectedGenreIds == null || request.SelectedGenreIds.Count == 0)
            {
                url = $"GameType/GetGamesWithGameType?id={request.GameTypeId}";
            } else
            {
                var genreQuery = string.Join("&genreIds=", request.SelectedGenreIds);
                url = $"Game/GetGamesByTypeAndGenres?gameTypeId={request.GameTypeId}&genreIds={genreQuery}";
            }


            var games = await _apiService.GetAsync<List<SimpleGameDto>>(url);
            DebugHelper.AppPrintDebugMessage($"Got games: {games.FirstOrDefault().GameGenres.FirstOrDefault().Genre.Name}");
            var result = _mapper.Map<List<GameViewModel>>(games);
            DebugHelper.AppPrintDebugMessage($"Got GAMEVM: {result.FirstOrDefault().GameGenres.FirstOrDefault().Genre.Name}");

            var viewModel = _mapper.Map<IEnumerable<GameListViewModel>>(result).ToList();
            DebugHelper.AppPrintDebugMessage($"Got GAME L VM: {viewModel.FirstOrDefault().GenreName}");

            if (games == null || games.Count == 0)
            {
                TempData["Error"] = "Looks like you are too picky! No games found, choose another filter!";
                return RedirectToAction("Index", "GameType");
            }

            return View("GameList", games);
        }
    }
    }
