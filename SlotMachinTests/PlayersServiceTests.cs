using Microsoft.Extensions.Configuration;
using Slot_Machine;
using Slot_Machine.Collections;
using Slot_Machine.Services;
using System;
using System.Threading.Tasks;
using Xunit;

public class PlayersServiceIntegrationTests
{
    private readonly PlayersService _playersService;
    private readonly MongoDBContext _dbContext;

    public PlayersServiceIntegrationTests()
    {
        // Load the real database configuration from appsettings.Test.json
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.Test.json", optional: false, reloadOnChange: true)
            .Build();

        // Initialize MongoDBContext and PlayersService
        _dbContext = new MongoDBContext(configuration);
        _playersService = new PlayersService(_dbContext);
    }

    [Fact]
    public async Task GetPlayerAsync_ShouldReturnPlayer()
    {
        // Arrange
        var testPlayerId = "1"; // Ensure this player exists in your database

        // Act
        var player = await _playersService.GetPlayerAsync(testPlayerId);

        // Assert
        Assert.NotNull(player); // Ensure a player is returned
        Assert.Equal(testPlayerId, player.PlayerId); // Validate the PlayerId matches
    }

    [Fact]
    public async Task GetAllPlayerAsync_ShouldReturnAllPlayers()
    {
        // Act
        var players = await _playersService.GetAllPlayerAsync();

        // Assert
        Assert.NotNull(players); // Ensure the list is not null
        Assert.NotEmpty(players); // Ensure there are players in the list
    }

    [Fact]
    public async Task UpdateBalanceAsync_ShouldUpdatePlayerBalance()
    {
        // Arrange
        var testPlayerId = "1"; // Ensure this player exists in your database
        var amountToAdd = 100.50m;

        // Get the initial balance
        var initialPlayer = await _playersService.GetPlayerAsync(testPlayerId);
        var initialBalance = initialPlayer.Balance;

        // Act
        var updatedBalance = await _playersService.UpdateBalanceAsync(testPlayerId, amountToAdd);

        // Assert
        Assert.Equal(initialBalance + amountToAdd, updatedBalance); // Ensure the balance is updated correctly

        // Fetch the player again to verify
        var updatedPlayer = await _playersService.GetPlayerAsync(testPlayerId);
        Assert.Equal(updatedBalance, updatedPlayer.Balance);
    }

    [Fact]
    public async Task UpdateBalanceAsync_PlayerNotFound_ShouldThrowException()
    {
        // Arrange
        var nonExistentPlayerId = "nonexistent";

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() =>
            _playersService.UpdateBalanceAsync(nonExistentPlayerId, 50.0m));
    }
}
