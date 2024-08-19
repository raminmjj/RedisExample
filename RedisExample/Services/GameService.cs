using RedisExample.Models;

namespace RedisExample.Services;



public class GameService : IGameService
{
    public List<Game> LoadGames() => GamesDb.GetAllGames();

    public Game? GetGameById(int id) => LoadGames().FirstOrDefault(x => x.Id == id);
}