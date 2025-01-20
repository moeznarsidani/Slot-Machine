using MongoDB.Driver;
using Slot_Machine.Collections;
using Slot_Machine.Interfaces;

namespace Slot_Machine.Services
{
    public class GameConfigService:IGameConfigService
    {
        private readonly MongoDBContext _context;

        public GameConfigService(MongoDBContext context)
        {
            _context = context;
        }

        public async Task<GameConfig> GetConfigurationAsync(string configId = "default")
        {
            var config = await _context.GameConfigs
                .Find(c => c.ConfigId == configId)
                .FirstOrDefaultAsync();

            if (config == null)
            {
                throw new Exception("Configuration not found.");
            }

            return config;
        }

        // Update the configuration in the database
        public async Task UpdateConfigurationAsync(UpdateConfigurationRequest request)
        {
            var winLines = GenerateWinLines(request.MatrixWidth, request.MatrixHeight);

            //exising
            // Retrieve the existing configuration to get its _id
            var existingConfig = await _context.GameConfigs
                .Find(c => c.ConfigId == "default")
                .FirstOrDefaultAsync();

            if (existingConfig == null)
            {
                throw new Exception("Configuration not found.");
            }

            // Create a new game configuration
            var newConfig = new GameConfig
            {
                Id = existingConfig.Id, 
                ConfigId = "default",
                MatrixWidth = request.MatrixWidth,
                MatrixHeight = request.MatrixHeight,
                WinLines = winLines,
                CreatedAt = DateTime.UtcNow
            };

            // Update or insert the configuration in the database
            var result = await _context.GameConfigs.ReplaceOneAsync(
                c => c.ConfigId == newConfig.ConfigId,
                newConfig,
                new ReplaceOptions { IsUpsert = true }
            );

            if (result.ModifiedCount == 0)
            {
                throw new Exception("Failed to update configuration.");
            }
        }

        private List<WinLine> GenerateWinLines(int width, int height)
        {
            if (width < 1 || height < 1)
            {
                throw new Exception("Invalid Dimension");
            }

            var winLines = new List<WinLine>();

            // Generate horizontal win lines (rows)
            for (int row = 0; row < height; row++)
            {
                var path = new List<List<int>>();
                for (int col = 0; col < width; col++)
                {
                    path.Add(new List<int> { col, row });
                }
                winLines.Add(new WinLine { Name = $"row-{row}", Path = path });
            }

            // Generate diagonal win lines
            // Diagonal down-right
            var diagonalDownPath = new List<List<int>>();
            for (int i = 0; i < Math.Min(width, height); i++) // Loop through the minimum of width and height
            {
                diagonalDownPath.Add(new List<int> { i, i });
            }
            if (diagonalDownPath.Count > 0)
            {
                winLines.Add(new WinLine { Name = "diagonal-down", Path = diagonalDownPath });
            }

            // Diagonal up-right
            var diagonalUpPath = new List<List<int>>();
            for (int i = 0; i < Math.Min(width, height); i++) // Loop through the minimum of width and height
            {
                diagonalUpPath.Add(new List<int> { i, height - 1 - i });
            }
            if (diagonalUpPath.Count > 0)
            {
                winLines.Add(new WinLine { Name = "diagonal-up", Path = diagonalUpPath });
            }

            return winLines;
        }


    }
}
