using Microsoft.AspNetCore.Mvc;
using Slot_Machine.Collections;
using Slot_Machine.Interfaces;
using Slot_Machine.Services;

namespace Slot_Machine.Controllers
{
    [ApiController]
    [Route("api/slotmachine")]
    public class SlotMachineController : ControllerBase
    {
        private readonly ISlotMachineService _slotMachineService;

        public SlotMachineController(ISlotMachineService slotMachineService)
        {
            _slotMachineService = slotMachineService;
        }

        [HttpPost("spin")]
        public async Task<ActionResult<SpinResult>> Spin([FromQuery] string playerId, [FromQuery] decimal betAmount)
        {
            try
            {
                var result = await _slotMachineService.SpinAsync(playerId, betAmount);
                return Ok(result);
            }
            catch
            {
                return BadRequest("Error in spining");
            }
        }
    }
}
