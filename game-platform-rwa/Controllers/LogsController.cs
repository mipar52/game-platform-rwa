using AutoMapper;
using GamePlatformBL.DTOs;
using GamePlatformBL.Models;
using GamePlatformBL.Utilities;
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
        private readonly IMapper _mapper;
        public LogsController(GamePlatformRwaContext context, IMapper mapper)
        {
            this.context = context;
            this._mapper = mapper;
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
                var mappedResult = _mapper.Map<IEnumerable<LogDto>>(result);
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
                var mappedResult = context.LogEntries
                    .OrderByDescending(l => l.Timestamp)
                    .Take(count)
                    .ToList();
                DebugHelper.ApiPrintDebugMessage($"Log count: {mappedResult.Count}");
                var logDtos = _mapper.Map<IEnumerable<LogDto>>(mappedResult);

                return Ok(logDtos);
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
                var mappedResult = _mapper.Map<IEnumerable<LogDto>>(result);
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
