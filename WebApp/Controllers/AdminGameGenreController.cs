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


            if (model.Id == 0)
            {
                var response = await _apiService.PostAsync("GameGenre/CreateGenre", model);
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Game Genre successfully created. You can always add another genre!";
                    return View(new GenreViewModel());
                } 
                else
                {
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    TempData["ErrorMessage"] = $"Failed to create Game Genre. Reason: {errorMessage}";
                    return View(model);
                }
            } else
            {
                var response = await _apiService.PutWithResponseAsync($"GameGenre/UpdateGenre?id={model.Id}", model);
                DebugHelper.AppPrintDebugMessage($"GameGenre action status: {response}");

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Game Genre successfully updated.";
                    return RedirectToAction("Index");
                }
                var errorMessage = await response.Content.ReadAsStringAsync();
                TempData["ErrorMessage"] = $"Failed to save game genre. Reason: {errorMessage}";
                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _apiService.DeleteAsync($"GameGenre/DeleteGenre?id={id}");
            return RedirectToAction("Index");
        }
    }
}
