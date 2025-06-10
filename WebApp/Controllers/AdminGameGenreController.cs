using WebApp.Services;
using GamePlatformBL.Utilities;
using GamePlatformBL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using GamePlatformBL.DTOs;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace WebApp.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminGameGenreController : Controller
    {
        private readonly ApiService _apiService;
        private readonly IMapper _mapper;
        public AdminGameGenreController(ApiService apiService, IMapper mapper)
        {
            _apiService = apiService;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index()
        {
            var gameTypes = _mapper.Map<List<GenreViewModel>>(await _apiService.GetAsync<List<GenreDto>>("GameGenre/GetAllGenres"));
            return View(gameTypes);
        }

        public async Task<IActionResult> Create(int? id)
        {
            if (id == null)
                return View(new GenreViewModel());

            var gameType = _mapper.Map<GenreViewModel>(await _apiService.GetAsync<GenreDto>($"GameGenre/GetGenreById?id={id}"));
            return View(gameType);
        }

        [HttpPost]
        public async Task<IActionResult> Create(GenreViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var dto = _mapper.Map<GenreDto>(model);
            HttpResponseMessage response = model.Id == 0
                ? await _apiService.PostAsync("GameGenre/CreateGenre", dto)
                : await _apiService.PutWithResponseAsync($"GameGenre/UpdateGenre?id={dto.Id}", dto);

            DebugHelper.AppPrintDebugMessage($"GameGenre action status: {response}");

            if (response.IsSuccessStatusCode)
                return RedirectToAction("Index");

            TempData["Error"] = "Failed to save game genre.";
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _apiService.DeleteAsync($"GameGenre/DeleteGenre?id={id}");
            return RedirectToAction("Index");
        }
    }
}
