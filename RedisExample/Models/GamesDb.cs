namespace RedisExample.Models;

public static class GamesDb
{
    public static List<Game> GetAllGames() => new List<Game>()
    {
        new Game
        {
            Id = 1,
            Title = "Game 1",
            Genre = "Genre 1",
            Platform = "Platform 1",
            ReleaseYear = 2000
        },
        new Game
        {
            Id = 2,
            Title = "Game 2",
            Genre = "Genre 2",
            Platform = "Platform 2",
            ReleaseYear = 2001
        }
    };
}