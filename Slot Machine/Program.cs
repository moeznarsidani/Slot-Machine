using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using Slot_Machine;
using Slot_Machine.Collections;
using Slot_Machine.Interfaces;
using Slot_Machine.Services;
using System.Numerics;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Slot Machine API", Version = "v1" });

});
builder.Services.AddControllers()
        .AddNewtonsoftJson();  // Use Newtonsoft for complex types
builder.Services.AddSingleton<MongoDBContext>();
// Register the services
builder.Services.AddScoped<IGameConfigService, GameConfigService>();
builder.Services.AddScoped<IPlayersService, PlayersService>();
builder.Services.AddScoped<ISlotMachineService, SlotMachineService>();

var app = builder.Build();

// Seed the database
await SeedDatabaseAsync(app.Services);

// Seed database function
async Task SeedDatabaseAsync(IServiceProvider services)
{
    var database = services.GetRequiredService<IMongoDatabase>();
    var gameConfigCollection = database.GetCollection<GameConfig>("game-config");
    var playersCollection = database.GetCollection<Players>("players");

    // Seed Game Config if not exists
    var gameConfigExists = await gameConfigCollection.Find(c => c.ConfigId == "default").AnyAsync();
    if (!gameConfigExists)
    {
        var defaultConfig = new GameConfig
        {
            ConfigId = "default",
            MatrixWidth = 5,
            MatrixHeight = 3,
            WinLines = new List<WinLine>
            {
                new WinLine { Name = "top-row", Path = new List<List<int>> { new() { 0, 0 }, new() { 1, 0 }, new() { 2, 0 }, new() { 3, 0 }, new() { 4, 0 } } },
                new WinLine { Name = "middle-row", Path = new List<List<int>> { new() { 0, 1 }, new() { 1, 1 }, new() { 2, 1 }, new() { 3, 1 }, new() { 4, 1 } } },
                new WinLine { Name = "bottom-row", Path = new List<List<int>> { new() { 0, 2 }, new() { 1, 2 }, new() { 2, 2 }, new() { 3, 2 }, new() { 4, 2 } } },
                new WinLine { Name = "diagonal-top-left", Path = new List<List<int>> { new() { 0, 0 }, new() { 1, 1 }, new() { 2, 2 }, new() { 3, 1 }, new() { 4, 0 } } },
                new WinLine { Name = "diagonal-middle", Path = new List<List<int>> { new() { 0, 1 }, new() { 1, 2 }, new() { 2, 1 }, new() { 3, 0 }, new() { 4, 1 } } }
            },
            CreatedAt = DateTime.UtcNow
        };
        await gameConfigCollection.InsertOneAsync(defaultConfig);
    }

    // Seed Players if not exists
    var players = new List<Players>
    {
        new Players { PlayerId = "1", Name = "Test Player", Balance = 1090m, CreatedAt = DateTime.UtcNow },
        new Players { PlayerId = "2", Name = "Test Player 2", Balance = 500m, CreatedAt = DateTime.UtcNow },
        new Players { PlayerId = "3", Name = "Test Player 3", Balance = 700m, CreatedAt = DateTime.UtcNow }
    };

    foreach (var player in players)
    {
        var existingPlayer = await playersCollection.Find(p => p.PlayerId == player.PlayerId).FirstOrDefaultAsync();
        if (existingPlayer == null)
        {
            await playersCollection.InsertOneAsync(player);
        }
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
