using GamePlatformBL.DTOs;
using GamePlatformBL.Interfaces;
using GamePlatformBL.Logger;
using GamePlatformBL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamePlatformBL.Repositories
{
    public class GameRepository : IGameRepository<Game, GameCreateDto>
    {
        private readonly GamePlatformRwaContext _context;
        private readonly LogService _logService;
        public GameRepository(GamePlatformRwaContext context, LogService logservice) 
        { 
            _context = context;
            _logService = logservice;
        }

        public Game Add(Game entity, GameCreateDto game)
        {
            foreach (var genre in game.GenreIds)
            {
                _context.GameGenres.Add(new GameGenre
                {
                    GameId = entity.Id,
                    Game = entity,
                    GenreId = genre,
                    Genre = _context.Genres.FirstOrDefault(g => g.Id == genre) ?? new Genre { Id = genre, Name = "Unknown" }
                });

                _logService.Log($"Added new Genre with id={genre} when creating the game.");
            }

            var gameTypes = _context.GameTypes.FirstOrDefault(x => x.Id == entity.GameTypeId);
            if (gameTypes != null)
            {
                gameTypes.Games.Add(entity);
            }

            _context.SaveChanges();
            _logService.Log($"Game '{entity.Name}' with id={entity.Id} created.");
            return entity;
        }

        public Game? Get(int id)
        {
            var result = _context.Games
            .Include(g => g.GameType)
            .Include(g => g.GameGenres).ThenInclude(gg => gg.Genre)
            .Include(g => g.Reviews)
            .FirstOrDefault(x => x.Id == id);

            return result;
        }

        public IEnumerable<Game> GetAll()
        {
            var result = _context.Games
            .Include(g => g.GameType)
            .Include(g => g.GameGenres).ThenInclude(gg => gg.Genre);
            return result.ToList();
        }

        public Game Update(int id, GameCreateDto model)
        {
            var existing = _context.Games
                .Include(g => g.GameGenres)
                .FirstOrDefault(g => g.Id == id);

            if (existing == null)
                throw new Exception($"Game with id={id} not found.");

            _context.GameGenres.RemoveRange(existing.GameGenres);

            foreach (var genreId in model.GenreIds.Distinct())
            {
                if (_context.Genres.Any(g => g.Id == genreId)) // Validate genre exists
                {
                    _context.GameGenres.Add(new GameGenre
                    {
                        GameId = id,
                        GenreId = genreId
                    });
                }
            }

            return existing;
        }

        public void Remove(int id)
        {
            var game = _context.Games
                .Include(g => g.Reviews)
                .Include(g => g.GameGenres)
                .FirstOrDefault(g => g.Id == id);

            if (game == null)
            {
                _logService.Log($"Failed to delete game with id={id}.", "No results");
                throw new Exception($"Game with id={id} not found.");
            }

            _context.Reviews.RemoveRange(game.Reviews);
            _context.GameGenres.RemoveRange(game.GameGenres);
            var gameTypes = _context.GameTypes.FirstOrDefault(x => x.Games.Contains(game));
            if (gameTypes != null)
            {
                gameTypes.Games.Remove(game);
            }
            _context.Games.Remove(game);

            _context.SaveChanges();
            _logService.Log($"Game '{game.Name}' with id={game.Id} deleted.");
        }
    }
}
