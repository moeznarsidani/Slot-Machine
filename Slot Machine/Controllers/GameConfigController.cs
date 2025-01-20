using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using Slot_Machine.Collections;
using Slot_Machine.Interfaces;
using Slot_Machine.Services;

namespace Slot_Machine.Controllers
{
    [ApiController]
    [Route("api/gameconfig")]
    public class GameConfigController : ControllerBase
    {
        private readonly IGameConfigService _gameConfigService;

        public GameConfigController(IGameConfigService gameConfigService)
        {
            _gameConfigService = gameConfigService;
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateConfig(UpdateConfigurationRequest request)
        {
            try
            {
                // Validate input
                if (request.MatrixWidth <= 0 || request.MatrixHeight <= 0)
                {
                    return BadRequest("Matrix width and height must be greater than 0.");
                }

                await _gameConfigService.UpdateConfigurationAsync(request);
                return Ok("Configuration updated.");
            }
            catch
            {
                return BadRequest("Error updating the configs");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetConfig()
        {
            var config = await _gameConfigService.GetConfigurationAsync();
            return Ok(config);
        }

       


    }
}
