using game_platform_rwa.DTO_generator;
using game_platform_rwa.DTOs;
using game_platform_rwa.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace game_platform_rwa.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameController : ControllerBase
    {

        private readonly GamePlatformRwaContext context;

        public GameController(GamePlatformRwaContext context) 
        {
            this.context = context;
        }

        [HttpGet("[action]")]
        public ActionResult<IEnumerable<GameDto>> GetAllGames()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = context.Games;
                var mappedResult = result.Select(x => GameDTOGenerator.generateGameDto(x));

                return Ok(mappedResult);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("[action]")]
        public ActionResult<IEnumerable<GameDto>> GetGamesWithGenre(int genreId)
        {
            var games = context.GameGenres
                .Include(gg => gg.Game)
                .Where(gg => gg.GenreId == genreId)
                .Select(gg => gg.Game)
                .Distinct()
                .ToList();

            if (!games.Any())
                return NotFound($"No games found with genre ID {genreId}");

            var result = games.Select(g => GameDTOGenerator.generateGameDto(g));
            return Ok(result);
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
                var result =
                    context.Games
                        .FirstOrDefault(x => x.Id == id);

                var mappedResult = GameDTOGenerator.generateGameDto(result);

                return Ok(mappedResult);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("[action]")]
        public ActionResult<IEnumerable<Game>> GetGamesWithGameType(int gameTypeId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var matchingGames = context.Games
                    .Where(game => game.GameTypeId == gameTypeId)
                    .ToList();

                if (!matchingGames.Any())
                {
                    return NotFound($"No games found containing '{gameTypeId}' in the name.");
                }

                var result = matchingGames.Select(x => GameDTOGenerator.generateGameDto(x));

                return Ok(result);
            }
            catch
            {
                return BadRequest($"Could not find any games with the game type ID {gameTypeId}!");
            }
        }

        [HttpGet("[action]")]
        public ActionResult<IEnumerable<Game>> GetGamesWithName(string name)
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
                    return NotFound($"No games found containing '{name}' in the name.");

                var result = matchingGames.Select(x => GameDTOGenerator.generateGameDto(x));

                return Ok(result);
            }
            catch
            {
                return BadRequest($"Something went wrong while searching for games with the name '{name}'.");
            }
        }


        [HttpPost("[action]")]
        public ActionResult<GameDto> AddGame([FromBody] GameCreateDto game)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var newGame = new Game
            {
                Name = game.Name,
                Description = game.Description,
                ReleaseDate = game.ReleaseDate,
                GameUrl = game.GameUrl,
                GameTypeId = game.GameTypeId
            };

            context.Games.Add(newGame);
            context.SaveChanges();

            foreach (var genreId in game.GenreIds)
            {
                context.GameGenres.Add(new GameGenre
                {
                    GameId = newGame.Id,
                    GenreId = genreId
                });
            }
            context.SaveChanges();

            return CreatedAtAction(nameof(GetGamesWithName), new { name = newGame.Name }, new { newGame.Id });
        }

        [HttpPut("[action]")]
        public IActionResult UpdateGame(int id, [FromBody] GameCreateDto game)
        {
            var existing = context.Games.Include(g => g.GameGenres).FirstOrDefault(g => g.Id == id);
            if (existing == null) return NotFound($"Game ID {id} not found");

            existing.Name = game.Name;
            existing.Description = game.Description;
            existing.ReleaseDate = game.ReleaseDate;
            existing.GameUrl = game.GameUrl;
            existing.GameTypeId = game.GameTypeId;

            // Update genres
            context.GameGenres.RemoveRange(existing.GameGenres);
            foreach (var genreId in game.GenreIds)
            {
                context.GameGenres.Add(new GameGenre { GameId = id, GenreId = genreId });
            }

            context.SaveChanges();
            return NoContent();
        }


        [HttpDelete("[action]")]
        public IActionResult DeleteGame(int id)
        {
            var game = context.Games
                .Include(g => g.Reviews)
                .Include(g => g.GameGenres)
                .FirstOrDefault(g => g.Id == id);

            if (game == null)
            {
                return NotFound($"Game with ID {id} not found.");
            }

            // Removal of related many-to-many and one-to-many entries
            context.Reviews.RemoveRange(game.Reviews);
            context.GameGenres.RemoveRange(game.GameGenres);
            context.Games.Remove(game);

            context.SaveChanges();

            return NoContent();
        }
    }
}
