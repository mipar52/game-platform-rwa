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
        public async Task<IActionResult> Index(int selectedGameTypeId, List<int> selectedGenreIds, int page = 1)
        {
            const int pageSize = 6;
            string endpoint;
            DebugHelper.AppPrintDebugMessage("HERE!!");
            if (selectedGenreIds == null || selectedGenreIds.Count == 0)
            {
                endpoint = $"GameType/GetGamesWithGameTypePaged?id={selectedGameTypeId}&page={page}&pageSize={pageSize}";
            }
            else
            {
                var queryString = string.Join("&", selectedGenreIds.Select(id => $"genreIds={id}"));
                endpoint = $"Game/GetGamesByTypeAndGenresPaged?gameTypeId={selectedGameTypeId}&{queryString}&page={page}&pageSize={pageSize}";
            }

            var pagedResult = await _apiService.GetAsync<PagedResult<SimpleGameDto>>(endpoint);

            if (pagedResult == null || pagedResult.Items.Count == 0)
            {
                return View(new PagedGameViewModel
                {
                    Games = [],
                    CurrentPage = page,
                    PageSize = pageSize,
                    TotalCount = 0,
                    SelectedGameTypeId = selectedGameTypeId,
                    SelectedGenreIds = selectedGenreIds
                });
            }

            var gameVM = _mapper.Map<PagedResult<GameViewModel>>(pagedResult);
            var gameListVM = _mapper.Map<PagedResult<GameListViewModel>>(gameVM);

            var viewModel = new PagedGameViewModel
            {
                Games = gameListVM.Items,
                CurrentPage = pagedResult.CurrentPage,
                PageSize = pagedResult.PageSize,
                TotalCount = pagedResult.TotalCount,
                SelectedGameTypeId = selectedGameTypeId,
                SelectedGenreIds = selectedGenreIds
            };

            return View(viewModel);
        }


        [HttpGet]
        public async Task<IActionResult> All(int page = 1)
        {
            const int pageSize = 6;

            var pagedResult = await _apiService.GetAsync<PagedResult<SimpleGameDto>>(
                $"Game/GetPagedGames?page={page}&pageSize={pageSize}");
            var gameVM = _mapper.Map<PagedResult<GameViewModel>>(pagedResult);
            var gameListVM = _mapper.Map<PagedResult<GameListViewModel>>(gameVM);

            var viewModel = new PagedGameViewModel
            {
                Games = gameListVM.Items,
                CurrentPage = pagedResult.CurrentPage,
                PageSize = pagedResult.PageSize,
                TotalCount = pagedResult.TotalCount
            };
            return View(viewModel);
        }
    }
}
