using Slot_Machine.Collections;

namespace Slot_Machine.Interfaces
{
    public interface IPlayersService
    {
        Task<Players> GetPlayerAsync(string playerId);
        Task<List<Players>> GetAllPlayerAsync();
        Task<decimal> UpdateBalanceAsync(string playerId, decimal amount);
    }
}
