using game_platform_rwa.DTO_generator;
using game_platform_rwa.DTOs;
using game_platform_rwa.Logger;
using game_platform_rwa.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

[Route("api/[controller]")]
[ApiController]
public class GameTypeController : ControllerBase
{
    private readonly GamePlatformRwaContext context;
    private readonly LogService logService;

    public GameTypeController(GamePlatformRwaContext context, LogService logService)
    {
        this.context = context;
        this.logService = logService;
    }

    [HttpGet("[action]")]
    public ActionResult<IEnumerable<GameType>> GetAllGameTypes()
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var result = context.GameTypes;
            var mappedResult = result.Select(x => GameDTOGenerator.generateGameTypeDto(x));
            if (!mappedResult.Any())
            {
                logService.Log($"Could not find any games types.", "No results");
            }
            logService.Log($"Found {mappedResult.Count()} GameTypes.", "Success");
            return Ok(mappedResult);
        }
        catch (Exception ex)
        {
            logService.Log($"Could not get any game types. Error: {ex.Message}", "Error");
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("[action]")]
    public ActionResult<GameType> GetGameTypeById(int id)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var result = context.GameTypes.FirstOrDefault(x => x.Id == id);

            if (result == null)
            {
                logService.Log($"Could not find game with ID {id}", "No results");
                return NotFound($"Could not find game with ID {id}");
            }

            var mappedResult = GameDTOGenerator.generateGameTypeDto(result);
            logService.Log($"Found {mappedResult.Id} GameType.", "Success");
            return Ok(mappedResult);
        }
        catch (Exception ex)
        {
            logService.Log($"Could not get game type with id={id}. Error: {ex.Message}", "Error");
            return StatusCode(500, ex.Message);
        }
    }

    [HttpGet("[action]")] 
    public ActionResult<IEnumerable<GameDto>> GetGamesWithGameType(int id)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        try
        {
            var gameType = context.GameTypes
            .FirstOrDefault(gt => gt.Id == id);
            if (gameType == null)
            {
                logService.Log($"Could not find game type with id={id}", "No results");
                return NotFound($"Could not find game type with id {id}");
            }
            var games = context.Games
                .Where(g => g.GameTypeId == gameType.Id)
                .Include(g => g.GameType)
                .Include(g => g.GameGenres).ThenInclude(gg => gg.Genre)
                .Include(g => g.Reviews)
                .ToList();


            if (games == null)
            {
                logService.Log($"Could not find any games with game type id={id}", "No results");
                return NotFound($"Could not find any games with game type id{id}");
            }

            var mappedResult = games.Select(x => GameDTOGenerator.generateGameDto(x));
            logService.Log($"Found {mappedResult.Count()} games with GameType id={id}.", "Success");
            Console.WriteLine($"Found {mappedResult.Count()} games with GameType id={id}.");
            return Ok(mappedResult);
        }
        catch (Exception ex)
        {
            logService.Log($"Could not get games with GameType id={id}. Error: {ex.Message}", "Error");
            return StatusCode(500, ex.Message);
        }
    }

    [HttpPost("[action]")]
    public IActionResult CreateGameType([FromBody] GameTypeCreateDto gameType)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        try
        {
            var newGameType = new GameType
            {
                Name = gameType.Name
            };

            context.GameTypes.Add(newGameType);

            context.SaveChanges();
            logService.Log($"Created new {newGameType.Name} GameType.", "Success");
            return CreatedAtAction(nameof(GetGameTypeById), new { name = newGameType.Name }, new { newGameType.Id });
        } catch (Exception ex)
        {
            logService.Log($"Error with creating new GameType with name={gameType.Name}. Error: {ex.Message}", "Error");
            return StatusCode(500, ex.Message);
        }

    }

    [HttpPut("[action]")]
    public IActionResult UpdateGameType(int id, [FromBody] GameTypeDto updated)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        try
        {
            var existing = context.GameTypes.Include(g => updated).FirstOrDefault(g => g.Id == id);
            if (existing == null)
            {
                logService.Log($"Could not update any GameType with id={updated.Id}", "No results");
                return NotFound($"GameType ID {id} not found");
            }

            existing.Name = updated.Name;

            context.SaveChanges();
            logService.Log($"Updated GameType with id={updated.Id}", "Success");
            return NoContent();
        } catch (Exception ex)
        {
            logService.Log($"Could not update GameType with id={updated.Id}. Error: {ex.Message}", "Error");
            return StatusCode(500, ex.Message);
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("[action]")]
    public IActionResult DeleteGameType(int id)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        try
        {
            var type = context.GameTypes.Include(t => t.Games).FirstOrDefault(t => t.Id == id);
            if (type == null) {

                logService.Log($"Could not delete any GameType with id={id}", "No results");
                return NotFound($"GameType ID {id} not found");
            }
            if (type.Games.Any())
            {
                logService.Log("Cannot delete type with games assigned.", "Error");
                return BadRequest("Cannot delete type with games assigned.");
            }

            context.GameTypes.Remove(type);
            context.SaveChanges();
            logService.Log($"Deleted GameType with id={id}.", "Success");
            return NoContent();
        } catch (Exception ex)
        {
            logService.Log($"Could not delete GameType with id={id}. Error: {ex.Message}");
            return StatusCode(500, ex.Message);
        }
    }
}
