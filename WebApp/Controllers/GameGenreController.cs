using Azure;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;
using WebApp.Services;
using WebApp.Utilities;
using WebApp.ViewModels;

namespace WebApp.Controllers
{
    public class GameGenreController : Controller
    {
        private readonly ApiService _apiService;
      //  private readonly IMapper _mapper;

        public GameGenreController(ApiService apiService)//, IMapper mapper)
        {
            _apiService = apiService;
       //     _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var genres = await _apiService.GetAsync<List<GameGenre>>($"GameGenre/GetAllGenres");
            DebugHelper.PrintDebugMessage($"Genres: {genres}");

            var viewModel = genres.Select(gt => new GameGenreViewModel
            {
                Id = gt.GenreId,
                Name = gt.Genre.Name
            }).ToList();

            return View(viewModel);
        }
    }
}

