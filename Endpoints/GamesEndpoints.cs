using GameStore.Api.Dtos;
namespace GameStore.Api.Endpoints;

public static class GamesEndpoints
{
  const string GetGameEndpointName = "GetGameById";


  private static readonly List<GameDto> games = [
    new (
    1,
    "Street Fighter 2",
    "Fighting",
    19.91M,
    new DateOnly(1992, 7 ,15)),
    new (
      2,
      "Super Mario Bros.",
      "Platformer",
      12.99M,
      new DateOnly(1985, 10, 15)),
    new (
      3,
      "The Legend of Zelda: A Link to the Past",
      "Adventure",
      8.99M,
      new DateOnly(1991, 10, 15)),
  ];

  public static void MapGamesEndpoints(this WebApplication app)
  {
    var group = app.MapGroup("/games");

    group.MapGet("/", () => games);

    group.MapGet("/{id}", (int id) =>
    {
      var game = games.Find(g => g.Id == id);
      return game is null ? Results.NotFound() : Results.Ok(game);
    })
    .WithName(GetGameEndpointName);

    group.MapPost("/", (CreateGameDto newGame) =>
    {
      if (string.IsNullOrWhiteSpace(newGame.Name))
      {
        return Results.BadRequest("Game name is required");
      }

      GameDto game = new(
        games.Count + 1,
        newGame.Name,
        newGame.Genre,
        newGame.Price,
        newGame.ReleaseDate
      );

      games.Add(game);
      return Results.CreatedAtRoute(GetGameEndpointName, new { id = game.Id }, game);
    });


    group.MapPut("/{id}", (int id, UpdateGameDto updateGame) =>
    {
      var index = games.FindIndex(g => g.Id == id);

      if (index == -1)
      {
        return Results.NotFound();
      }

      games[index] = new GameDto(
        id,
        updateGame.Name,
        updateGame.Genre,
        updateGame.Price,
        updateGame.ReleaseDate
      );

      return Results.NoContent();
    });

    group.MapDelete("/{id}", (int id) =>
    {
      games.RemoveAll(g => g.Id == id);
      return Results.NoContent();
    });
  }
}