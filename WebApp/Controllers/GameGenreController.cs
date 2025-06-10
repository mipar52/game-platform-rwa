using AutoMapper;
using Azure;
using GamePlatformBL.DTOs;
using WebApp.Services;
using GamePlatformBL.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace WebApp.Controllers
{
    [Authorize]
    public class GameGenreController : Controller
    {
        private readonly ApiService _apiService;
        private readonly IMapper _mapper;

        public GameGenreController(ApiService apiService, IMapper mapper)
        {
            _apiService = apiService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var genres = await _apiService.GetAsync<List<GenreDto>>($"GameGenre/GetAllGenres");
            DebugHelper.AppPrintDebugMessage($"Genres: {genres}");

            var viewModel = _mapper.Map<List<GameGenreController>>(genres);
            return View(viewModel);
        }
    }
}

