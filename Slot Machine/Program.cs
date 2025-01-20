using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Slot_Machine;
using Slot_Machine.Collections;
using Slot_Machine.Interfaces;
using Slot_Machine.Services;

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
