using game_platform_rwa.DTOs;
using game_platform_rwa.Models;
using Microsoft.Extensions.Configuration.UserSecrets;

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
                GameType = new GameTypeDto
                {
                    Id = game.GameType.Id,
                    Name = game.GameType.Name
                },
                MetacriticScore = game.MetacriticScore,
                WonGameOfTheYear = game.WonGameOfTheYear ?? false,
                ImagePath = game.ImagePath,
                ImageUrl = game.ImageUrl,
                Genres = game.GameGenres?
                    .Select(gg => generateGenreDto(gg.Genre))
                    .ToList() ?? new List<GenreDto>(),

                Reviews = game.Reviews?
                    .Select(r => new GameReviewDto
                    {
                        Id = r.Id,
                        UserId = r.UserId,
                        Rating = r.Rating,
                        ReviewText = r.ReviewText,
                        Approved = r.Approved,
                        CreatedAt = r.CreatedAt
                    })
                    .ToList() ?? new List<GameReviewDto>()
            };
        }
        

        public static GameTypeDto generateGameTypeDto(GameType gameType)
        {
            return new GameTypeDto
            {
                Id = gameType.Id,
                Name = gameType.Name,
            };
        }

        public static GenreDto generateGenreDto(Genre genre)
        {
            return new GenreDto
            {
                Id = genre.Id,
                Name = genre.Name,
            };
        }

        public static GameReviewDto generateGameReviewDto(Review review)
        {
            return new GameReviewDto
            {
                Id = review.Id,
                UserId = review.UserId,
                GameId = review.GameId,
                Rating = review.Rating,
                ReviewText = review.ReviewText,
                Approved = review.Approved,
                CreatedAt = review.CreatedAt,
                Game = review.Game,
                User = review.User
            };
        }

        public static UserGameReviewDto generateUserGameReviewDto(Review review)
        {
            return new UserGameReviewDto
            {
                Id = review.Id,
                UserId = review.UserId,
                GameName = review.Game.Name,
                GameId = review.GameId,
                Rating = review.Rating,
                ReviewText = review.ReviewText,
                Approved = review.Approved,
                CreatedAt = review.CreatedAt,
            };
        }
    }
}
