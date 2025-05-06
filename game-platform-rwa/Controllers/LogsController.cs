using game_platform_rwa.DTO_generator;
using game_platform_rwa.DTOs;
using game_platform_rwa.Logger;
using game_platform_rwa.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace game_platform_rwa.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class LogsController : ControllerBase
    {
        private readonly GamePlatformRwaContext context;

        public LogsController(GamePlatformRwaContext context)
        {
            this.context = context;
        }

        [HttpGet("[action]")]
        public ActionResult<IEnumerable<LogDto>> GetAllLogs()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = context.LogEntries;
                var mappedResult = result.Select(x => LogDtoGenerator.generateLogDto(x));
                return Ok(mappedResult);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("[action]")]
        public ActionResult<IEnumerable<LogDto>> GetLastLogs(int count)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = context.LogEntries;
                var mappedResult = 
                    result
                    .OrderByDescending(l => l.Timestamp)
                    .Take(count)
                    .Select(x => LogDtoGenerator.generateLogDto(x));

                return Ok(mappedResult);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("[action]")]
        public ActionResult<int> GetLogCount()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = context.LogEntries;
                var mappedResult = result.Select(x => LogDtoGenerator.generateLogDto(x));
                if (!mappedResult.Any())
                    return NotFound("Could not find any logs.");
                
                return Ok(mappedResult.Count());
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
