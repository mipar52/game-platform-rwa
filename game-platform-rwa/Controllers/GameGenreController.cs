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

        [HttpGet]
        public ActionResult<IEnumerable<Genre>> GetAll()
        {
            return Ok(context.Genres.ToList());
        }

        [HttpGet("{id}")]
        public ActionResult<Genre> GetById(int id)
        {
            var genre = context.Genres.Find(id);
            return genre == null ? NotFound() : Ok(genre);
        }

        [HttpPost]
        public IActionResult Create([FromBody] Genre genre)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            context.Genres.Add(genre);
            context.SaveChanges();
            return CreatedAtAction(nameof(GetById), new { id = genre.Id }, genre);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] Genre updated)
        {
            var existing = context.Genres.Find(id);
            if (existing == null) return NotFound();

            existing.Name = updated.Name;
            context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
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
