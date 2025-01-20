using Slot_Machine.Collections;

namespace Slot_Machine.Interfaces
{
    public interface ISlotMachineService
    {
        Task<SpinResult> SpinAsync(string playerId, decimal betAmount);
    }
}
