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
    public class GameTypeRepository : IGameRepository<GameType, GameTypeCreateDto>
    {
        private readonly GamePlatformRwaContext _context;
        private readonly LogService _logService;
        public GameTypeRepository(GamePlatformRwaContext context, LogService logservice)
        {
            _context = context;
            _logService = logservice;
        }

        public GameType Add(GameType entity, GameTypeCreateDto vm)
        {
            return new GameType();
        }

        public GameType? Get(int id)
        {
            var result = _context.GameTypes.FirstOrDefault(x => x.Id == id);

            if (result == null)
            {
                _logService.Log($"Could not find game with ID {id}", "No results");
                throw new Exception($"Could not find game with ID {id}");
            }
            return result;
        }

        public IEnumerable<GameType> GetAll()
        {
            return _context.GameTypes;
        }

        public void Remove(int id)
        {
            var type = _context.GameTypes.Include(t => t.Games).FirstOrDefault(t => t.Id == id);
            if (type == null)
            {

                _logService.Log($"Could not delete any GameType with id={id}", "No results");
                throw new Exception($"GameType ID {id} not found");
            }
            if (type.Games.Any())
            {
                _logService.Log("Cannot delete type with games assigned.", "Error");
                throw new Exception("Cannot delete type with games assigned.");
            }

            _context.GameTypes.Remove(type);
            _context.SaveChanges();
            _logService.Log($"Deleted GameType with id={id}.", "Success");
        }

        public GameType Update(int id, GameTypeCreateDto model)
        {
            return new GameType();
        }
    }
}
