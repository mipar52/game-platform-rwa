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
                Name = g.Name,
                Description = g.Description,
                GenreName = string.Join(", ", g.Genres.Select(x => x.Name)),
                TypeName = g.GameType
            }).ToList();

            return View(viewModel);
        }
    }
}
