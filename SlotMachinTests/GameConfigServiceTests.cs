using Microsoft.Extensions.Configuration;
using Slot_Machine;
using Slot_Machine.Collections;
using Slot_Machine.Services;
using System;
using System.Threading.Tasks;
using Xunit;

public class GameConfigServiceIntegrationTests
{
    private readonly GameConfigService _gameConfigService;

    public GameConfigServiceIntegrationTests()
    {
        // Load the real database configuration from appsettings.Test.json
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory()) // Test project's base directory
            .AddJsonFile("appsettings.Test.json", optional: false, reloadOnChange: true)
            .Build();

        // Initialize MongoDBContext and GameConfigService
        var dbContext = new MongoDBContext(configuration);
        _gameConfigService = new GameConfigService(dbContext);
    }

    [Fact]
    public async Task GetConfigurationAsync_ShouldReturnConfig()
    {
        // Arrange
        var configId = "default";

        // Act
        var config = await _gameConfigService.GetConfigurationAsync(configId);

        // Assert
        Assert.NotNull(config); // Ensure a configuration is returned
        Assert.Equal(configId, config.ConfigId); // Validate the ConfigId matches
        Assert.True(config.MatrixWidth > 0); // Check that MatrixWidth is valid
        Assert.True(config.MatrixHeight > 0); // Check that MatrixHeight is valid
        Assert.NotEmpty(config.WinLines); // Ensure there are win lines
    }

    [Fact]
    public async Task UpdateConfigurationAsync_ShouldUpdateConfig()
    {
        // Arrange
        var updateRequest = new UpdateConfigurationRequest
        {
            MatrixWidth = 5,
            MatrixHeight = 3
        };

        // Act
        await _gameConfigService.UpdateConfigurationAsync(updateRequest);

        // Fetch the updated configuration
        var updatedConfig = await _gameConfigService.GetConfigurationAsync("default");

        // Assert
        Assert.NotNull(updatedConfig); // Ensure the configuration exists
        Assert.Equal("default", updatedConfig.ConfigId); // Check that the ConfigId matches
        Assert.Equal(updateRequest.MatrixWidth, updatedConfig.MatrixWidth); // Validate MatrixWidth
        Assert.Equal(updateRequest.MatrixHeight, updatedConfig.MatrixHeight); // Validate MatrixHeight
        Assert.NotEmpty(updatedConfig.WinLines); // Ensure win lines are generated
    }

    [Fact]
    public async Task UpdateConfigurationAsync_InvalidDimensions_ShouldThrowException()
    {
        // Arrange
        var invalidRequest = new UpdateConfigurationRequest
        {
            MatrixWidth = 0, // Invalid width
            MatrixHeight = 3
        };

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() =>
            _gameConfigService.UpdateConfigurationAsync(invalidRequest));
    }
}
