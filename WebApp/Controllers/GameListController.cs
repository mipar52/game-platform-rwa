using Microsoft.AspNetCore.Mvc;
using System.Linq;
using WebApp.Models;
using WebApp.Services;
using WebApp.ViewModels;

namespace WebApp.Controllers
{
    public class GameListController : Controller
    {
        private readonly ApiService _apiService;

        public GameListController(ApiService apiService)
        {
            _apiService = apiService;
        }

        [HttpPost]
        public async Task<IActionResult> Index(int selectedGameTypeId, List<int> selectedGenreIds)
        {
            var queryString = string.Join("&", selectedGenreIds.Select(id => $"genreIds={id}"));
            var endpoint = $"Game/GetGamesByTypeAndGenres?gameTypeId={selectedGameTypeId}&{queryString}";

            var games = await _apiService.GetAsync<List<GameViewModel>>(endpoint);

            var viewModel = games.Select(g => new GameListViewModel
            {
                Id = g.Id,
                Name = g.Name,
                Description = g.Description,
                GenreName = string.Join(", ", g.Genres.Select(x => x.Name)),
                GameType = new GameTypeViewModel
                {
                    Id = g.GameType.Id,
                    Name = g.GameType.Name
                }
            }).ToList();

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> All()
        {
            var allGames = await _apiService.GetAsync<List<GameViewModel>>("Game/GetAllGames");

            var viewModel = allGames.Select(g => new GameListViewModel
            {
                Id = g.Id,
                Name = g.Name,
                Description = g.Description,
                GenreName = string.Join(", ", g.Genres.Select(x => x.Name)),
                GameType = new GameTypeViewModel
                {
                    Id = g.GameType.Id,
                    Name = g.GameType.Name
                }
            }).ToList();

            return View("Index", viewModel); // reuse existing Index view
        }

    }
}
