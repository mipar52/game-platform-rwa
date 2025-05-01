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
            try
            {
                var result = context.Games;
                var mappedResult = result.Select(x => generateGameDto(x));

                return Ok(mappedResult);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("[action]")]
        public ActionResult<GameDto> GetGameById(int id)
        {
            try
            {
                var result =
                    context.Games
                        .FirstOrDefault(x => x.Id == id);

                var mappedResult = generateGameDto(result);

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
            try
            {
                var matchingGames = context.Games
                    .Where(game => game.GameTypeId == gameTypeId)
                    .ToList();

                if (!matchingGames.Any())
                {
                    return NotFound($"No games found containing '{gameTypeId}' in the name.");
                }

                var result = matchingGames.Select(x => generateGameDto(x));

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
            try
            {
                var matchingGames = context.Games
                    .Where(game => game.Name.Contains(name))
                    .ToList();

                if (!matchingGames.Any())
                    return NotFound($"No games found containing '{name}' in the name.");

                var result = matchingGames.Select(x => generateGameDto(x));

                return Ok(result);
            }
            catch
            {
                return BadRequest($"Something went wrong while searching for games with the name '{name}'.");
            }
        }


        [HttpPost]
        public void Post([FromBody] string value)
        {

        }

        // PUT api/<GameController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<GameController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        private GameDto generateGameDto(Game game)
        {
            return new GameDto
            {
                Id = game.Id,
                Name = game.Name,
                Description = game.Description,
                ReleaseDate = game.ReleaseDate,
                GameUrl = game.GameUrl,
                GameTypeId = game.GameTypeId,
                GameType = game.GameType,
                Reviews = game.Reviews,
                Genres = game.Genres
            };
        }
    }
}
