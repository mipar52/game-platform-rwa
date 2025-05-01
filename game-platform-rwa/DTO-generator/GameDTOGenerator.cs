using game_platform_rwa.DTOs;
using game_platform_rwa.Models;

namespace game_platform_rwa.DTO_generator
{
    public static class GameDTOGenerator
    {
        public static GameDto generateGameDto(Game game)
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
              //  Genres = game.GameGenres
            };
        }
        public static GameTypeDto generateGameTypeDto(GameType gameType)
        {
            return new GameTypeDto
            {
                Id = gameType.Id,
                Name = gameType.Name,
                Games = gameType.Games

            };
        }
    }
}
