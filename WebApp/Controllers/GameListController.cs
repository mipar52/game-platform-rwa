using AutoMapper;
using Azure.Core;
using GamePlatformBL.DTOs;
using WebApp.Services;
using GamePlatformBL.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using GamePlatformBL.Utilities;
using System;

namespace WebApp.Controllers
{
    public class GameListController : Controller
    {
        private readonly ApiService _apiService;
        private readonly IMapper _mapper;
        public GameListController(ApiService apiService, IMapper mapper)
        {
            _apiService = apiService;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> Index(int selectedGameTypeId, List<int> selectedGenreIds)
        {
            string endpoint;
            if (selectedGenreIds == null || selectedGenreIds.Count == 0)
            {
                endpoint = $"GameType/GetGamesWithGameType?id={selectedGameTypeId}";
            }
            else
            {
                var queryString = string.Join("&", selectedGenreIds.Select(id => $"genreIds={id}"));
                endpoint = $"Game/GetGamesByTypeAndGenres?gameTypeId={selectedGameTypeId}&{queryString}";
            }

            var games = await _apiService.GetAsync<List<SimpleGameDto>>(endpoint);
            if (games == null || games.Count == 0)
            {
                return View();
            }
            var result = _mapper.Map<List<GameViewModel>>(games);

            var viewModel = _mapper.Map<IEnumerable<GameListViewModel>>(result).ToList();

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> All()
        {
            var allGames = await _apiService.GetAsync<List<SimpleGameDto>>("Game/GetAllGames");
            var gameVM = _mapper.Map<List<GameViewModel>>(allGames);
            var viewModel = _mapper.Map<IEnumerable<GameListViewModel>>(gameVM).ToList();
            return View("Index", viewModel);
        }

    }
}
