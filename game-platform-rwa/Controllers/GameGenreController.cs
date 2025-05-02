using game_platform_rwa.DTO_generator;
using game_platform_rwa.DTOs;
using game_platform_rwa.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace game_platform_rwa.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameGenreController : ControllerBase
    {
        private GamePlatformRwaContext context;

        public GameGenreController(GamePlatformRwaContext context)
        {
            this.context = context;
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

                return Ok(mappedResult);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id}")]
        public ActionResult<Genre> GetGenreById(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result =
                    context.Genres
                        .FirstOrDefault(x => x.Id == id);

                var mappedResult = GameDTOGenerator.generateGenreDto(result);

                return Ok(mappedResult);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        public IActionResult CreateGenre([FromBody] GenreDto genre)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

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

            context.SaveChanges();

            return CreatedAtAction(nameof(GetGenreById), new { name = newGenre.Name }, new { newGenre.Id });
        }

        [HttpPut("{id}")]
        public IActionResult UpdateGenre(int id, [FromBody] Genre updated)
        {
            var existing = context.Genres.Include(g => g.GameGenres).FirstOrDefault(g => g.Id == id);
            if (existing == null) return NotFound($"Genre ID {id} not found");

            existing.Name = updated.Name;
            existing.GameGenres = updated.GameGenres;

            context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("[action]")]
        public IActionResult DeleteGenre(int id)
        {
            var genre = context.Genres.Include(g => g.GameGenres).FirstOrDefault(g => g.Id == id);
            if (genre == null) return NotFound();

            if (genre.GameGenres.Any())
                return BadRequest("Cannot delete genre assigned to games.");

            context.Genres.Remove(genre);
            context.SaveChanges();
            return NoContent();
        }
    }
}
