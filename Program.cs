using GameStore.Api.Dtos;

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

app.MapGet("/games/{id}", (int id) => games.Find(g => g.Id == id));

app.Run();
