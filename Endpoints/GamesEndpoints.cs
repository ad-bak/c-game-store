using GameStore.Api.Data;
using GameStore.Api.Dtos;
using GameStore.Api.Models;
using Microsoft.EntityFrameworkCore;
namespace GameStore.Api.Endpoints;

public static class GamesEndpoints
{
  const string GetGameEndpointName = "GetGameById";


  private static readonly List<GameSummaryDto> games = [
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

    group.MapGet("/", async (GameStoreContext dbContext)
      => await dbContext.Games
                        .Include(game => game.Genre)
                        .Select(game => new GameSummaryDto(
                          game.Id,
                          game.Name,
                          game.Genre!.Name,
                          game.Price,
                          game.ReleaseDate
                        ))
                        .AsNoTracking()
                        .ToListAsync());

    group.MapGet("/{id}", async (int id, GameStoreContext dbContext) =>
    {
      var game = await dbContext.Games.FindAsync(id);
      return game is null ? Results.NotFound() : Results.Ok(game);
    })
    .WithName(GetGameEndpointName);

    group.MapPost("/", async (CreateGameDto newGame, GameStoreContext dbContext) =>
    {
      if (string.IsNullOrWhiteSpace(newGame.Name))
      {
        return Results.BadRequest("Game name is required");
      }

      Game game = new()
      {
        Name = newGame.Name,
        GenreId = newGame.GenreId,
        Price = newGame.Price,
        ReleaseDate = newGame.ReleaseDate
      };

      dbContext.Games.Add(game);
      await dbContext.SaveChangesAsync();

      GameDetailsDto gameDto = new(
        game.Id,
        game.Name,
        game.GenreId,
        game.Price,
        game.ReleaseDate
      );

      return Results.CreatedAtRoute(GetGameEndpointName, new { id = gameDto.Id }, gameDto);
    });


    group.MapPut("/{id}", async (int id, UpdateGameDto updateGame, GameStoreContext dbContext) =>
    {
      var existingGame = await dbContext.Games.FindAsync(id);

      if (existingGame is null)
      {
        return Results.NotFound();
      }

      existingGame.Name = updateGame.Name;
      existingGame.GenreId = updateGame.GenreId;
      existingGame.Price = updateGame.Price;
      existingGame.ReleaseDate = updateGame.ReleaseDate;

      await dbContext.SaveChangesAsync();

      return Results.NoContent();
    });

    group.MapDelete("/{id}", (int id) =>
    {
      games.RemoveAll(g => g.Id == id);
      return Results.NoContent();
    });
  }
}