// Controller: GamesCategoryController.cs
using Microsoft.AspNetCore.Mvc;
using GamePlatformBL.DTOs;
using GamePlatformBL.Utilities;
using GamePlatformBL.ViewModels;
using WebApp.Services;
using AutoMapper;
using GamePlatformBL.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Azure;

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
        public async Task<IActionResult> Browse(GameSearchRequestViewModel request, int page = 1)
        {
            const int pageSize = 6;

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Invalid form submission.";
                return RedirectToAction("Index");
            }

            string url;
            if (request.SelectedGenreIds == null || request.SelectedGenreIds.Count == 0)
            {
                url = $"GameType/GetGamesWithGameTypePaged?id={request.GameTypeId}&page={page}&pageSize={pageSize}";
            }
            else
            {
                var genreQuery = string.Join("&genreIds=", request.SelectedGenreIds);
                url = $"Game/GetGamesByTypeAndGenresPaged?gameTypeId={request.GameTypeId}&genreIds={genreQuery}&page={page}&pageSize={pageSize}";
            }

            var pagedResult = await _apiService.GetAsync<PagedResult<SimpleGameDto>>(url);

            if (pagedResult == null || pagedResult.Items.Count == 0)
            {
                TempData["Error"] = "Looks like you are too picky! No games found, choose another filter!";
                return RedirectToAction("Index", "GameType");
            }

            var gameVM = _mapper.Map<PagedResult<GameViewModel>>(pagedResult);
            var gameListVM = _mapper.Map<PagedResult<GameListViewModel>>(gameVM);

            var viewModel = new PagedGameViewModel
            {
                Games = gameListVM.Items,
                CurrentPage = gameListVM.CurrentPage,
                PageSize = gameListVM.PageSize,
                TotalCount = gameListVM.TotalCount,
                SelectedGameTypeId = request.GameTypeId,
                SelectedGenreIds = request.SelectedGenreIds
            };

            return View("GameList", viewModel);
        }

    }
}
