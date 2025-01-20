using Microsoft.AspNetCore.Mvc;
using Slot_Machine.Collections;
using Slot_Machine.Interfaces;

namespace Slot_Machine.Controllers
{
    public class PlayersController : Controller
    {
        private readonly IPlayersService _playersService;

        public PlayersController(IPlayersService playersService)
        {
            _playersService = playersService;
        }

        [HttpGet("Players")]
        public async Task<IActionResult> GetAllPlayers()
        {
            var players = await _playersService.GetAllPlayerAsync();

            if (players is null || players.Count() < 1)
            {
                return NotFound("Players not found.");
            }

            return Ok(players);
        }

        [HttpGet("{playerId}")]
        public async Task<IActionResult> GetPlayer(string playerId)
        {
            var player = await _playersService.GetPlayerAsync(playerId);

            if (player == null)
            {
                return NotFound("Player not found.");
            }

            return Ok(player);
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateBalance([FromBody] UpdateBalanceRequest request)
        {
            try
            {
                var newBalance = await _playersService.UpdateBalanceAsync(request.PlayerId, request.Amount);
                return Ok(new { Balance = newBalance });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
