using System;
using Microsoft.AspNetCore.Mvc;
using SutesFozes.Infrastructure;
using Npgsql;

namespace SutesFozes.Controllers;

public class FavouriteRecipeDto {
        public required int Id {get;set;}
        public required string Title {get; set;}
        public required int Carma {get;set;}
        public required string Author {get;set;}
}

[ApiController]
[ServiceFilter(typeof(AuthorizationFilter))]

public class MyFavouriteRecipesController : ControllerBase
{
        private readonly Postgresql _postgresql;

        public MyFavouriteRecipesController(Postgresql postgresql)
        {
                _postgresql = postgresql;
        }

        [HttpGet("api/profile/my_favourite_recipes")]
        public async Task<IActionResult> MyFavouriteRecipes()
        {
                var userId = (int)HttpContext.Items["UserId"];
                var tokenVersion = (int)HttpContext.Items["TokenVersion"];

                await using(var conn = await _postgresql.GetOpenConnectionAsync())
                {
                        await using(var recipes = new NpgsqlCommand("SELECT fr.*, r.*, u.username FROM favourite_recipe fr JOIN recipe r ON fr.recipeid = r.id JOIN users u ON r.userid = u.id WHERE fr.userid = @userid", conn))
                        {
                                recipes.Parameters.AddWithValue("userid", userId);

                                await using(var reader = await recipes.ExecuteReaderAsync())
                                {
                                        List<FavouriteRecipeDto> recipelist = new List<FavouriteRecipeDto>();

                                        while(await reader.ReadAsync())
                                        {
                                                var recipe = new FavouriteRecipeDto
                                                {
                                                        Id = reader.GetInt32(reader.GetOrdinal("recipeid")),
                                                        Title = reader.GetString(reader.GetOrdinal("title")),
                                                        Carma = reader.IsDBNull(reader.GetOrdinal("rating")) ? 0 : reader.GetInt32(reader.GetOrdinal("rating")),
                                                        Author = reader.GetString(reader.GetOrdinal("username"))
                                                };

                                                recipelist.Add(recipe);
                                        }

                                        return Ok(recipelist);
                                }
                        }
                }
        }
}