using Microsoft.AspNetCore.Mvc;
using RedisExample.Models;
using RedisExample.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOutputCache();
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("RedisConn");
    options.InstanceName = "GameCatalog_";
});

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddSession(options => { options.IdleTimeout = TimeSpan.FromMinutes(3); });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IRedisCacheService, RedisCacheService>();
builder.Services.AddSingleton<IGameService, GameService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseSession();
app.UseOutputCache();
    
var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
    {
        var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
            .ToArray();
        return forecast;
    })
    .WithName("GetWeatherForecast")
    .CacheOutput(x=> x.Expire(TimeSpan.FromMinutes(1)))
    .WithOpenApi();

// تعریف مسیر API
app.MapGet("/api/games", async (HttpContext httpContext, 
        IGameService gameService, 
        IRedisCacheService cacheService) =>
    {
        var instanceId = GetInstanceId(httpContext);
        var cacheKey = $"Games_Cache_{instanceId}";
        var games = cacheService.GetCachedData<List<Game>>(cacheKey);
        bool isFromCache;

        if (games is null)
        {
            games = gameService.LoadGames();
            cacheService.SetCachedData(cacheKey, games, TimeSpan.FromMinutes(3));
            isFromCache = false;
        }
        else
        {
            isFromCache = true;
        }

        return Results.Ok(new { Games = games, IsFromCache = isFromCache });
    })
    .WithName("GetAllGames")
    .WithOpenApi();

string GetInstanceId(HttpContext httpContext)
{
    var instanceId = httpContext.Session.GetString("InstanceId");
    if (string.IsNullOrEmpty(instanceId))
    {
        instanceId = Guid.NewGuid().ToString();
        httpContext.Session.SetString("InstanceId", instanceId);
    }

    return instanceId;
}


app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}