using game_platform_rwa.DTO_generator;
using game_platform_rwa.DTOs;
using game_platform_rwa.Logger;
using game_platform_rwa.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace game_platform_rwa.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class GameGenreController : ControllerBase
    {
        private GamePlatformRwaContext context;
        private readonly LogService logService;

        public GameGenreController(GamePlatformRwaContext context, LogService logService)
        {
            this.context = context;
            this.logService = logService;
        }

        [HttpGet("[action]")]
        public ActionResult<IEnumerable<Genre>> GetAllGenres()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = context.Genres;
                var mappedResult = result.Select(x => GameDTOGenerator.generateGenreDto(x));

                if (!mappedResult.Any())
                {
                    logService.Log($"GetAllGenres requested. No genres found.", "No results");
                    return NotFound("No genres found.");
                }

                logService.Log($"GetAllGenres requested. Found {mappedResult.Count()} games.", "Success");
                return Ok(mappedResult);
            }
            catch (Exception ex)
            {
                logService.Log($"Failed to get all genres. Error: {ex.Message}", "Error");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("[action]")]
        public ActionResult<Genre> GetGenreById(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = context.Genres.FirstOrDefault(x => x.Id == id);

                if (result == null)
                {
                    logService.Log($"GetGenreById requested. No genre found with ID {id}.", "No results");
                    return NotFound($"Could not find genre with ID {id}");

                }

                var mappedResult = GameDTOGenerator.generateGenreDto(result);
                logService.Log($"Genre '{mappedResult.Name}' with id={id} found.");
                return Ok(mappedResult);
            }
            catch (Exception ex)
            {
                logService.Log($"Failed to get genre by Id. Error: {ex.Message}", "Error");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("[action]")]
        public IActionResult CreateGenre([FromBody] GenreDto genre)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var newGenre = new Genre
                {
                    Name = genre.Name,
                    GameGenres = genre.GameGenres.Select(g => new GameGenre
                    {
                        GameId = g.GameId,
                        GenreId = g.GenreId,
                        AddedOn = DateTime.Now
                    }).ToList()
                };

                context.Genres.Add(newGenre);
                context.SaveChanges();

                logService.Log($"Ganre '{newGenre.Name}' with id={newGenre.Id} created.", "Success");
                return CreatedAtAction(nameof(GetGenreById), new { name = newGenre.Name }, new { newGenre.Id });

            } catch (Exception ex)
            {
                logService.Log($"Failed to add genre. Error: {ex.Message}", "Error");
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("[action]")]
        public IActionResult UpdateGenre(int id, [FromBody] Genre updated)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var existing = context.Genres.Include(g => g.GameGenres).FirstOrDefault(g => g.Id == id);
                if (existing == null) return NotFound($"Genre ID {id} not found");

                existing.Name = updated.Name;
                existing.GameGenres = updated.GameGenres;

                context.SaveChanges();
                logService.Log($"Genre '{existing.Name}' with id={existing.Id} updated.");

                return NoContent();
            } catch (Exception ex)
            {
                logService.Log($"Failed to update genre. Error: {ex.Message}", "Error");
                return StatusCode(500, ex.Message);
            }

        }
        
        [Authorize(Roles = "Admin")]
        [HttpDelete("[action]")]
        public IActionResult DeleteGenre(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var genre = context.Genres.Include(g => g.GameGenres).FirstOrDefault(g => g.Id == id);
                if (genre == null)
                {
                    logService.Log($"Failed to delete genre with id={id}.", "No results");
                    return NotFound($"Genre with ID {id} not found.");
                }

                if (genre.GameGenres.Any())
                {
                    logService.Log($"Failed to delete genre with id={id}. Cannot delete genre assigned to games. ", "Error");
                    return BadRequest("Cannot delete genre assigned to games.");
                }

                context.Genres.Remove(genre);
                context.SaveChanges();
                logService.Log($"Deleted genre with id={id}. ", "Success");
                return NoContent();
            } catch (Exception ex)
            {
                logService.Log($"Failed to delete genre with id={id}. Error: {ex.Message}", "Error");
                return StatusCode(500, ex.Message);
            }
        }
    }
}
