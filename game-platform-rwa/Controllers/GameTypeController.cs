using AutoMapper;
using GamePlatformBL.DTOs;
using GamePlatformBL.Logger;
using GamePlatformBL.Models;
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
    private readonly IMapper _mapper;
    public GameTypeController(GamePlatformRwaContext context, LogService logService, IMapper mapper)
    {
        this.context = context;
        this.logService = logService;
        _mapper = mapper;
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
            var mappedResult = _mapper.Map<IEnumerable<GameTypeDto>>(result);
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

            var mappedResult = _mapper.Map<GameTypeDto>(result);
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
    public ActionResult<IEnumerable<SimpleGameDto>> GetGamesWithGameType(int id)
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
                .Include(g => g.GameGenres).ThenInclude(gg => gg.Genre);


            if (games == null)
            {
                logService.Log($"Could not find any games with game type id={id}", "No results");
                return NotFound($"Could not find any games with game type id{id}");
            }

            var mappedResult = _mapper.Map<IEnumerable<SimpleGameDto>>(games);
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

    [HttpGet("GetGamesWithGameTypePaged")]
    public async Task<ActionResult<PagedResult<SimpleGameDto>>> GetGamesWithGameTypePaged(
    int id, int page = 1, int pageSize = 6)
    {
        if (page <= 0 || pageSize <= 0)
            return BadRequest("Page and pageSize must be greater than 0.");

        try
        {
            var gameType = await context.GameTypes.FirstOrDefaultAsync(gt => gt.Id == id);
            if (gameType == null)
            {
                logService.Log($"GameType not found with id={id}", "No Results");
                return NotFound($"GameType id={id} not found.");
            }

            var query = context.Games
                .Where(g => g.GameTypeId == id)
                .Include(g => g.GameType)
                .Include(g => g.GameGenres).ThenInclude(gg => gg.Genre);

            var totalCount = await query.CountAsync();
            var pagedGames = await query
                .OrderBy(g => g.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var mapped = _mapper.Map<List<SimpleGameDto>>(pagedGames);

            return Ok(new PagedResult<SimpleGameDto>
            {
                Items = mapped,
                CurrentPage = page,
                PageSize = pageSize,
                TotalCount = totalCount
            });
        }
        catch (Exception ex)
        {
            logService.Log($"Error getting paged games by GameType id={id}. {ex.Message}", "Error");
            return StatusCode(500, ex.Message);
        }
    }


    [Authorize(Roles = "Admin")]
    [HttpPost("[action]")]
    public IActionResult CreateGameType([FromBody] GameTypeCreateDto gameType)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        try
        {
            var trimmedName = gameType.Name.Trim();
            if (context.GameTypes.Any(x => x.Name.Equals(trimmedName)))
            {
                logService.Log($"Game Type with name {trimmedName} already exists", "Error");
                return BadRequest($"Game Type with name {trimmedName} already exists");
            }
            var newGameType = _mapper.Map<GameType>(gameType);

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

    [Authorize(Roles = "Admin")]
    [HttpPut("[action]")]
    public IActionResult UpdateGameType(int id, [FromBody] GameTypeCreateDto updated)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var existing = context.GameTypes.FirstOrDefault(g => g.Id == id);
            if (existing == null)
            {
                logService.Log($"Could not update any GameType with id={id}", "No results");
                return NotFound($"GameType ID {id} not found");
            }
            _mapper.Map(updated, existing);

            context.SaveChanges();
            logService.Log($"Updated GameType with id={id}", "Success");
            return Ok();
        }
        catch (Exception ex)
        {
            logService.Log($"Could not update GameType with id={id}. Error: {ex.Message}", "Error");
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
