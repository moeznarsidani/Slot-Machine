using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Slot_Machine.Collections;
using Slot_Machine.Services;
using Slot_Machine.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace Slot_Machine.Tests
{
    public class SlotMachineServiceIntegrationTests
    {
        private readonly SlotMachineService _slotMachineService;
        private readonly PlayersService _playersService;
        private readonly MongoDBContext _dbContext;

        public SlotMachineServiceIntegrationTests()
        {
            // Load the real database configuration from appsettings.Test.json
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.Test.json", optional: false, reloadOnChange: true)
                .Build();

            // Initialize MongoDBContext and PlayersService
            _dbContext = new MongoDBContext(configuration);
            _playersService = new PlayersService(_dbContext);
            _slotMachineService = new SlotMachineService(_dbContext, new GameConfigService(_dbContext));
        }

        [Fact]
        public async Task SpinAsync_ShouldUpdateBalance_WhenPlayerWins()
        {
            // Arrange: Create a test player in the real database
            var playerId = "1";
            var betAmount = 10m;

            // Act: Perform the spin
            var player = await _playersService.GetPlayerAsync(playerId);
            var result = await _slotMachineService.SpinAsync(playerId, betAmount);

            // Assert: Check if the balance has been updated correctly
            var updatedPlayer = await _dbContext.Players.Find(p => p.PlayerId == playerId).FirstOrDefaultAsync();
            Assert.NotNull(updatedPlayer);
            Assert.Equal(player.Balance - betAmount + result.WinAmount, updatedPlayer.Balance);
        }

        [Fact]
        public async Task SpinAsync_ShouldThrowException_WhenInsufficientBalance()
        {
            // Arrange: Create a test player with insufficient balance
            var playerId = "5";
            var initialBalance = 5m; // Not enough for the bet
            var betAmount = 10m;

            var player = new Players
            {
                PlayerId = playerId,
                Balance = initialBalance
            };

            await _dbContext.Players.InsertOneAsync(player);

            // Act & Assert: Ensure the method throws an exception for insufficient balance
            await Assert.ThrowsAsync<Exception>(() => _slotMachineService.SpinAsync(playerId, betAmount));
        }

        [Fact]
        public async Task SpinAsync_ShouldReturnCorrectResultMatrix()
        {
            // Arrange: Create a test player
            var playerId = "1";
            var initialBalance = 100m;
            var betAmount = 10m;

            // Act: Perform the spin
            var result = await _slotMachineService.SpinAsync(playerId, betAmount);

            // Assert: Check if the result matrix is correctly generated (random values)
            Assert.Equal(5, result.ResultMatrix.GetLength(0)); // Matrix width
            Assert.Equal(3, result.ResultMatrix.GetLength(1)); // Matrix height
        }
    }
}
