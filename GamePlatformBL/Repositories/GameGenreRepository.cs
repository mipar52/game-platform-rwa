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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace GamePlatformBL.Repositories
{
    public class GameGenreRepository : IGameRepository<Genre, GenreCreateDto>
    {
        private readonly GamePlatformRwaContext _context;
        private readonly LogService _logService;
        public GameGenreRepository(GamePlatformRwaContext context, LogService logservice)
        {
            _context = context;
            _logService = logservice;
        }
        public Genre Add(Genre entity, GenreCreateDto vm)
        {
            return new Genre();
        }

        public Genre? Get(int id)
        {
            return _context.Genres.FirstOrDefault(x => x.Id == id);
        }

        public IEnumerable<Genre> GetAll()
        {
            return _context.Genres;
        }

        public void Remove(int id)
        {
            var genre = _context.Genres.Include(g => g.GameGenres).FirstOrDefault(g => g.Id == id);
            if (genre == null)
            {
                _logService.Log($"Failed to delete genre with id={id}.", "No results");
                throw new Exception($"Genre with ID {id} not found.");
            }

            //var gameGenres = context.GameGenres.Where(gg => gg.GenreId == id);
            //context.GameGenres.RemoveRange(gameGenres);

            if (genre.GameGenres.Any())
            {
                _logService.Log($"Failed to delete genre with id={id}. Cannot delete genre assigned to games. ", "Error");
                throw new Exception("Cannot delete genre assigned to games.");
            }

            _context.Genres.Remove(genre);
            _context.SaveChanges();
        }

        public Genre Update(int id, GenreCreateDto model)
        {
            var existing = _context.Genres.Include(g => g.GameGenres).FirstOrDefault(g => g.Id == id);
            if (existing == null) throw new Exception($"Genre ID {id} not found");
            existing.Name = model.Name;
            /*
                             context.GameGenres.RemoveRange(existing.GameGenres);
            foreach (var genre in existing.GameGenres)
            {
                context.GameGenres.Add(new GameGenre { GameId = id, GenreId = context.Genres.FirstOrDefault(g => g.Id == existing.Id)?.Id ?? 0 });
            }
             */
            _context.SaveChanges();
            return existing;
        }
    }
}
