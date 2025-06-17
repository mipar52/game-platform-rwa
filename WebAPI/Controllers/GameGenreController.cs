using AutoMapper;
using GamePlatformBL.DTOs;
using GamePlatformBL.Logger;
using GamePlatformBL.Models;
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
        private readonly IMapper _mapper;

        public GameGenreController(GamePlatformRwaContext context, LogService logService, IMapper mapper)
        {
            this.context = context;
            this.logService = logService;
            _mapper = mapper;
        }

        [HttpGet("[action]")]
        public ActionResult<IEnumerable<GenreDto>> GetAllGenres()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = context.Genres;
                var mappedResult = _mapper.Map<IEnumerable<GenreDto>>(result);

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
        public ActionResult<GenreDto> GetGenreById(int id)
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

                var mappedResult = _mapper.Map<GenreDto>(result);
                logService.Log($"Genre '{mappedResult.Name}' with id={id} found.");
                return Ok(mappedResult);
            }
            catch (Exception ex)
            {
                logService.Log($"Failed to get genre by Id. Error: {ex.Message}", "Error");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("[action]")]
        public ActionResult<IEnumerable<GameDto>> GetGamesWithGenres([FromQuery] List<int> genreIds)
        {
            if (!ModelState.IsValid || genreIds == null || !genreIds.Any())
            {
                return BadRequest("No genre IDs provided.");
            }

            try
            {
                // Get games that match at least one of the selected genres
                var gameIds = context.GameGenres
                    .Where(gg => genreIds.Contains(gg.GenreId))
                    .Select(gg => gg.GameId)
                    .Distinct()
                    .ToList();

                var games = context.Games
                    .Where(g => gameIds.Contains(g.Id))
                    .Include(g => g.GameType)
                    .Include(g => g.GameGenres).ThenInclude(gg => gg.Genre)
                    .Include(g => g.Reviews)
                    .ToList();

                if (!games.Any())
                {
                    logService.Log($"No games found with genre IDs: {string.Join(",", genreIds)}", "No results");
                    return NotFound("No games matched the selected genres.");
                }

                var mappedResult = _mapper.Map<IEnumerable<GameDto>>(games);
                logService.Log($"Found {mappedResult.Count()} games with genre IDs: {string.Join(",", genreIds)}", "Success");
                return Ok(mappedResult);
            }
            catch (Exception ex)
            {
                logService.Log($"Error retrieving games with genres {string.Join(",", genreIds)}: {ex.Message}", "Error");
                return StatusCode(500, ex.Message);
            }
        }


        [HttpPost("[action]")]
        public IActionResult CreateGenre([FromBody] GenreCreateDto genre)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var trimmedName = genre.Name.Trim();
                if (context.Genres.Any(x => x.Name.Equals(trimmedName)))
                {
                    logService.Log($"Genre with name {trimmedName} already exists", "Error");
                    return BadRequest($"Genre with name {trimmedName} already exists");
                }
                var newGenre = _mapper.Map<Genre>(genre);

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
        public IActionResult UpdateGenre(int id, [FromBody] GenreCreateDto updated)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var existing = context.Genres.Include(g => g.GameGenres).FirstOrDefault(g => g.Id == id);
                if (existing == null) return NotFound($"Genre ID {id} not found");
               // _mapper.Map(updated, existing);
                existing.Name = updated.Name;
                /*
                                 context.GameGenres.RemoveRange(existing.GameGenres);
                foreach (var genre in existing.GameGenres)
                {
                    context.GameGenres.Add(new GameGenre { GameId = id, GenreId = context.Genres.FirstOrDefault(g => g.Id == existing.Id)?.Id ?? 0 });
                }
                 */
                context.SaveChanges();
                logService.Log($"Genre '{existing.Name}' with id={existing.Id} updated.");

                return CreatedAtAction(nameof(GetGenreById), new { name = updated.Name }, new { existing.Id });
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

                //var gameGenres = context.GameGenres.Where(gg => gg.GenreId == id);
                //context.GameGenres.RemoveRange(gameGenres);

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
