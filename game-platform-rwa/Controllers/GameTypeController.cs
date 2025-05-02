using game_platform_rwa.DTO_generator;
using game_platform_rwa.DTOs;
using game_platform_rwa.Models;
using Microsoft.AspNetCore.Authorization;
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
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var result = context.GameTypes;
            var mappedResult = result.Select(x => GameDTOGenerator.generateGameTypeDto(x));

            return Ok(mappedResult);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("{id}")]
    public ActionResult<GameType> GetGameTypeById(int id)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var result =
                context.GameTypes
                    .FirstOrDefault(x => x.Id == id);

            if (result == null)
                return NotFound($"Could not find game with ID {id}");

            var mappedResult = GameDTOGenerator.generateGameTypeDto(result);

            return Ok(mappedResult);
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPost]
    public IActionResult CreateGameType([FromBody] GameTypeDto gameType)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var newGameType = new GameType
        {
            Name = gameType.Name,
            Games = gameType.Games.Select(g => new Game
            {
                Id = g.Id,
                Name = g.Name,
                Description = g.Description,
                ReleaseDate = g.ReleaseDate,
                GameUrl = g.GameUrl,
                GameTypeId = g.GameTypeId
            }).ToList()
        };

        context.GameTypes.Add(newGameType);
        context.SaveChanges();

        return CreatedAtAction(nameof(GetGameTypeById), new { name = newGameType.Name }, new { newGameType.Id });
    }

    [HttpPut("[action]")]
    public IActionResult UpdateGameType(int id, [FromBody] GameTypeDto updated)
    {
        var existing = context.GameTypes.Include(g => updated).FirstOrDefault(g => g.Id == id);
        if (existing == null) return NotFound($"Genre ID {id} not found");

        existing.Name = updated.Name;
        existing.Games = updated.Games;

        context.SaveChanges();
        return NoContent();
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("[action]")]
    public IActionResult DeleteGameType(int id)
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
