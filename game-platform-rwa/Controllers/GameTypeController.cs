using game_platform_rwa.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
public class GameTypeController : ControllerBase
{
    private readonly GamePlatformRwaContext context;

    public GameTypeController(GamePlatformRwaContext context)
    {
        this.context = context;
    }

    [HttpGet]
    public ActionResult<IEnumerable<GameType>> GetAll()
    {
        return Ok(context.GameTypes.ToList());
    }

    [HttpGet("{id}")]
    public ActionResult<GameType> GetById(int id)
    {
        var type = context.GameTypes.Find(id);
        return type == null ? NotFound() : Ok(type);
    }

    [HttpPost]
    public IActionResult Create([FromBody] GameType gameType)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        context.GameTypes.Add(gameType);
        context.SaveChanges();
        return CreatedAtAction(nameof(GetById), new { id = gameType.Id }, gameType);
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] GameType updated)
    {
        var existing = context.GameTypes.Find(id);
        if (existing == null) return NotFound();

        existing.Name = updated.Name;
        context.SaveChanges();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var type = context.GameTypes.Include(t => t.Games).FirstOrDefault(t => t.Id == id);
        if (type == null) return NotFound();

        if (type.Games.Any())
            return BadRequest("Cannot delete type with games assigned.");

        context.GameTypes.Remove(type);
        context.SaveChanges();
        return NoContent();
    }
}
