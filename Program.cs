using GameStore.Api.Dtos;

const string GetGameEndpointName = "GetGameById";


var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

List<GameDto> games = [
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

app.MapGet("/games", () => games);


app.MapGet("/games/{id}", (int id) => games.Find(g => g.Id == id))
.WithName(GetGameEndpointName);

app.MapPost("/games", (CreateGameDto newGame) =>
{
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


app.MapPut("/games/{id}", (int id, UpdateGameDto updateGame) =>
{
  var index = games.FindIndex(g => g.Id == id);
  games[index] = new GameDto(
    id,
    updateGame.Name,
    updateGame.Genre,
    updateGame.Price,
    updateGame.ReleaseDate
  );

  return Results.NoContent();
});

app.MapDelete("/games/{id}", (int id) =>
{
  games.RemoveAll(g => g.Id == id);
  return Results.NoContent();
});


app.Run();
