using WebApp.Services;
using GamePlatformBL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using GamePlatformBL.DTOs;
using AutoMapper;

namespace WebApp.Controllers
{
    public class AllGamesController : Controller
    {
        private readonly ApiService _apiService;
        private readonly IMapper _mapper;
        public AllGamesController(ApiService apiService, IMapper mapper)
        {
            _apiService = apiService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1)
        {
            const int pageSize = 6;

            var pagedResult = await _apiService.GetAsync<PagedResult<SimpleGameDto>>(
                $"Game/GetPagedGames?page={page}&pageSize={pageSize}");
            var map = _mapper.Map<PagedResult<GameViewModel>>(pagedResult);
            var two = _mapper.Map<PagedResult<GameListViewModel>>(map);

            var viewModel = new PagedGameViewModel
            {
                Games = two.Items,
                CurrentPage = pagedResult.CurrentPage,
                PageSize = pagedResult.PageSize,
                TotalCount = pagedResult.TotalCount
            };
            return View(viewModel);
        }

    }
}
