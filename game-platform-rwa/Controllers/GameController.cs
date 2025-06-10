using AutoMapper;
using GamePlatformBL.DTOs;
using GamePlatformBL.Logger;
using GamePlatformBL.Models;
using GamePlatformBL.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace game_platform_rwa.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class GameController : Controller
    {

        private readonly GamePlatformRwaContext context;
        private readonly LogService logService;
        private readonly IMapper _mapper;
        public GameController(GamePlatformRwaContext context, LogService logService, IMapper mapper) 
        {
            this.context = context;
            this.logService = logService;
            this._mapper = mapper;
        }

        [HttpGet("[action]")]
        public ActionResult<IEnumerable<SimpleGameDto>> GetAllGames()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = context.Games
                    .Include(g => g.GameType)
                    .Include(g => g.GameGenres).ThenInclude(gg => gg.Genre);
                //  .Include(g => g.Reviews);
                
                var mappedResult = _mapper.Map<IEnumerable<SimpleGameDto>>(result);

                if (!mappedResult.Any()) 
                {
                  logService.Log($"GetAllGames requested. No games found.", "No results");
                    return NotFound("No games found.");
                }

                logService.Log($"GetAllGames requested. Found {mappedResult.Count()} games.", "Success");
                return Ok(mappedResult);
            }
            catch (Exception ex)
            {
                DebugHelper.ApiPrintDebugMessage("Error mapping games: " + ex.Message);
                logService.Log($"Failed to get all games. Error: {ex.Message}", "Error");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("[action]")]
        public ActionResult<GameDto> GetGameById(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = context.Games
                    .Include(g => g.GameType)
                    .Include(g => g.GameGenres).ThenInclude(gg => gg.Genre)
                    .Include(g => g.Reviews)
                    .FirstOrDefault(x => x.Id == id);

                if (result == null)
                {
                    logService.Log($"Failed to get game with id={id}.", "No results");
                    return NotFound($"Could not find game with ID {id}");
                }

                var mappedResult = _mapper.Map<GameDto>(result);

                logService.Log($"Game '{mappedResult.Name}' with id={id} found.");
                return Ok(mappedResult);
            }
            catch (Exception ex)
            {
                logService.Log($"Failed to get game by Id. Error: {ex.Message}", "Error");
                return StatusCode(500, ex.Message);
            }
        }
        
         
        [HttpGet("[action]")]
        public ActionResult<IEnumerable<GameDto>> GetGamesWithName(string name)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var matchingGames = context.Games
                    .Where(game => game.Name.Contains(name))
                    .ToList();

                if (!matchingGames.Any())
                {
                    logService.Log($"Failed to get games with name={name}.", "No results");
                    return NotFound($"No games found containing '{name}' in the name.");
                }

                var mappedResult = _mapper.Map<IEnumerable<GameDto>>(matchingGames);
                logService.Log($"Found '{matchingGames.Count}' games with name={name}.");
                return Ok(mappedResult);
            }
            catch (Exception ex)
            {
                logService.Log($"Failed to GetGamesWithName with name={name}. Error: {ex.Message}", "Error");
                return BadRequest($"Something went wrong while searching for games with the name '{name}'.");
            }
        }

        [HttpGet("[action]")]
        public ActionResult<IEnumerable<SimpleGameDto>> GetGamesByTypeAndGenres(int gameTypeId, [FromQuery] List<int> genreIds)
        {
            if (!ModelState.IsValid || genreIds == null || !genreIds.Any())
            {
                return BadRequest("You must provide at least one genre ID.");
            }

            try
            {
                // Get game IDs that have at least one of the specified genres
                var matchingGameIds = context.GameGenres
                    .Where(gg => genreIds.Contains(gg.GenreId))
                    .Select(gg => gg.GameId)
                    .Distinct()
                    .ToList();

                // Get games that match the GameType AND one of the specified genres
                var games = context.Games
                    .Where(g => g.GameTypeId == gameTypeId && matchingGameIds.Contains(g.Id))
                    .Include(g => g.GameType)
                    .Include(g => g.GameGenres).ThenInclude(gg => gg.Genre);

                if (!games.Any())
                {
                    logService.Log($"No games found with GameTypeId={gameTypeId} and Genres={string.Join(",", genreIds)}.", "No Results");
                    return NotFound("No games matched the selected filters.");
                }

                var mappedResult = _mapper.Map<IEnumerable<SimpleGameDto>>(games);

                logService.Log($"Found {mappedResult.Count()} games for GameTypeId={gameTypeId} and Genres={string.Join(",", genreIds)}.", "Success");

                return Ok(mappedResult);
            }
            catch (Exception ex)
            {
                logService.Log($"Error getting games by type and genres: {ex.Message}", "Error");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }


        [Authorize(Roles = "Admin")]
        [HttpPost("[action]")]
        public ActionResult<GameCreateDto> CreateGame([FromBody] GameCreateDto game)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var trimmedName = game.Name.Trim();
                if (context.Games.Any(x => x.Name.Equals(trimmedName)))
                {
                    logService.Log($"Game with name {trimmedName} already exists", "Error");
                    return BadRequest($"Game with name {trimmedName} already exists");
                }
                var newGame = _mapper.Map<Game>(game);

                foreach (var genre in game.GenreIds)
                {
                    context.GameGenres.Add(new GameGenre
                    {
                        GameId = newGame.Id,
                        Game = newGame,
                        GenreId = genre,
                        Genre = context.Genres.FirstOrDefault(g => g.Id == genre) ?? new Genre { Id = genre, Name = "Unknown" }
                    });

                    logService.Log($"Added new Genre with id={genre} when creating the game.");
                }

                var gameTypes = context.GameTypes.FirstOrDefault(x => x.Id == newGame.GameTypeId);
                if (gameTypes != null)
                {
                    gameTypes.Games.Add(newGame);
                }

               context.SaveChanges();
                logService.Log($"Game '{newGame.Name}' with id={newGame.Id} created.");

                return CreatedAtAction(nameof(GetGameById), new { name = newGame.Name }, new { newGame.Id });
            }
            catch (Exception ex)
            {
                logService.Log($"Failed to add game. Error: {ex.Message}", "Error");
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("[action]")]
        public ActionResult<GameCreateDto> UpdateGame(int id, [FromBody] GameCreateDto game)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var existing = context.Games
                    .Include(g => g.GameGenres)
                    .FirstOrDefault(g => g.Id == id);

                if (existing == null)
                    return NotFound($"Game ID {id} not found");

                // Map scalar properties only
                _mapper.Map(game, existing);

                // Update GameGenres safely
                // 1. Remove all existing
                context.GameGenres.RemoveRange(existing.GameGenres);

                // 2. Add new, avoiding nulls
                foreach (var genreId in game.GenreIds.Distinct())
                {
                    if (context.Genres.Any(g => g.Id == genreId)) // Validate genre exists
                    {
                        context.GameGenres.Add(new GameGenre
                        {
                            GameId = id,
                            GenreId = genreId
                        });
                    }
                }

                context.SaveChanges();

                logService.Log($"Game '{existing.Name}' with id={existing.Id} updated.");
                return Ok(existing);
            }
            catch (Exception ex)
            {
                logService.Log($"Failed to update game with id={id}. Error: {ex.Message}", "Error");
                return StatusCode(500, ex.Message);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("[action]")]
        public IActionResult DeleteGame(int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var game = context.Games
                    .Include(g => g.Reviews)
                    .Include(g => g.GameGenres)
                    .FirstOrDefault(g => g.Id == id);

                if (game == null)
                {
                    logService.Log($"Failed to delete game with id={id}.", "No results");
                    return NotFound($"Game with ID {id} not found.");
                }

                // Removal of related many-to-many and one-to-many entries
                context.Reviews.RemoveRange(game.Reviews);
                context.GameGenres.RemoveRange(game.GameGenres);
                var gameTypes = context.GameTypes.FirstOrDefault(x => x.Games.Contains(game));
                if (gameTypes != null)
                {
                    gameTypes.Games.Remove(game);
                }
                context.Games.Remove(game);

                context.SaveChanges();
                logService.Log($"Game '{game.Name}' with id={game.Id} deleted.");

                return NoContent(); 
            }
            catch (Exception ex)
            {
                logService.Log($"Failed to delete game with id={id}. Error: {ex.Message}", "Error");
                return StatusCode(500, ex.Message);
            }
        }
    }
}
