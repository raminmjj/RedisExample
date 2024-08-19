using RedisExample.Models;

namespace RedisExample.Services;

public interface IGameService
{
    List<Game> LoadGames();
    Game? GetGameById(int id);
}