using MongoDB.Driver;
using Slot_Machine.Collections;
using Slot_Machine.Interfaces;
using System.Numerics;

namespace Slot_Machine.Services
{
    public class PlayersService:IPlayersService
    {
        private readonly MongoDBContext _context;

        public PlayersService(MongoDBContext context)
        {
            _context = context;
        }

        public async Task<Players> GetPlayerAsync(string playerId)
        {
            return await _context.Players.Find(player => player.PlayerId == playerId).FirstOrDefaultAsync();
        }

        public async Task<List<Players>> GetAllPlayerAsync()
        {
            return await _context.Players.Find(x=>true).ToListAsync();
        }

        public async Task<decimal> UpdateBalanceAsync(string playerId, decimal amount)
        {
            var player = await _context.Players.Find(p => p.PlayerId == playerId).FirstOrDefaultAsync();
            if (player == null)
            {
                throw new Exception("Player not found.");
            }

            // Add the amount to player's current balance
            player.Balance += amount;

            // Use optimistic concurrency control to ensure no other update is in progress
            var updateResult = await _context.Players.ReplaceOneAsync(
                p => p.PlayerId == playerId,
                player,
                new ReplaceOptions { IsUpsert = false }
            );

            if (updateResult.ModifiedCount == 0)
            {
                throw new Exception("Balance update conflict. Please try again.");
            }

            return player.Balance;
        }
    }
}
