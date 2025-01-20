using MongoDB.Driver;
using Slot_Machine.Collections;
using Slot_Machine.Interfaces;
using System.Numerics;

namespace Slot_Machine.Services
{
    public class SlotMachineService:ISlotMachineService
    {
        private readonly MongoDBContext _context;
        private readonly IGameConfigService _configService;

        public SlotMachineService(MongoDBContext context, IGameConfigService configService)
        {
            _context = context;
            _configService = configService;
        }

        public async Task<SpinResult> SpinAsync(string playerId, decimal betAmount)
        {
            var player = await _context.Players.Find(p => p.PlayerId == playerId).FirstOrDefaultAsync();
            if (player == null || player.Balance < betAmount)
            {
                throw new Exception("Insufficient balance.");
            }

            // Deduct the bet amount and apply concurrency control
            player.Balance -= betAmount;
            var updateResult = await _context.Players.ReplaceOneAsync(
                p => p.PlayerId == playerId && p.Balance == (player.Balance + betAmount),
                player
            );

            if (updateResult.MatchedCount == 0)
            {
                throw new Exception("Balance update conflict.");
            }

            // Fetch the current configuration
            var config = await _configService.GetConfigurationAsync();
            var resultMatrix = GenerateResultMatrix(config.MatrixWidth, config.MatrixHeight);

            // Calculate wins based on the win lines
            decimal totalWin = 0;
            foreach (var line in config.WinLines)
            {
                totalWin += CalculateLineWin(resultMatrix, line.Path, betAmount);
            }

            // Add win to player balance
            player.Balance += totalWin;
            await _context.Players.ReplaceOneAsync(
                p => p.PlayerId == playerId && p.Balance == (player.Balance - totalWin),
                player
            );

            return new SpinResult
            {
                ResultMatrix = resultMatrix,
                WinAmount = totalWin,
                PlayerBalance = player.Balance
            };
        }

        // Generate the random result matrix based on matrix size
        private int[,] GenerateResultMatrix(int width, int height)
        {
            var random = new Random();
            var matrix = new int[width, height];

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    matrix[i, j] = random.Next(0, 10);  // Random number between 0-9
                }
            }

            return matrix;
        }

        // Calculate the win for a single win line path
        private decimal CalculateLineWin(int[,] matrix, List<List<int>> path, decimal betAmount)
        {
            decimal lineWin = 0;
            int consecutiveCount = 1;

            for (int i = 1; i < path.Count; i++)
            {
                var previous = matrix[path[i - 1][0], path[i - 1][1]];
                var current = matrix[path[i][0], path[i][1]];

                if (previous == current)
                {
                    consecutiveCount++;
                }
                else
                {
                    if (consecutiveCount >= 3)
                    {
                        lineWin += consecutiveCount * betAmount;
                    }
                    consecutiveCount = 1;  // Reset for the next series
                }
            }

            // Account for the last series if it was a win
            if (consecutiveCount >= 3)
            {
                lineWin += consecutiveCount * betAmount;
            }

            return lineWin;
        }
    }
}
