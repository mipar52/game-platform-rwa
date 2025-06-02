using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
    public class GamesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
