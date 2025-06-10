using WebApp.Services;
using GamePlatformBL.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
    public class AllGamesController : Controller
    {
        private readonly ApiService _apiService;

        public AllGamesController(ApiService apiService)
        {
            _apiService = apiService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var games = await _apiService.GetAsync<List<GameViewModel>>("Game/GetAllGames");
            return View(games);
        }
    }
}
