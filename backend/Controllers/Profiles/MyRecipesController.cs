using System;
using Microsoft.AspNetCore.Mvc;
using SutesFozes.Infrastructure;
using Npgsql;

namespace SutesFozes.Controllers;

public class RecipeDto {
        public required int Id {get;set;}
        public required string Title {get; set;}
        public required int Carma {get;set;}
        public required bool Checked {get;set;}
}

[ApiController]
[ServiceFilter(typeof(AuthorizationFilter))]

public class MyRecipesController : ControllerBase
{
        private readonly Postgresql _postgresql;

        public MyRecipesController(Postgresql postgresql)
        {
                _postgresql = postgresql;
        }

        [HttpGet("api/profile/my_recipes")]
        public async Task<IActionResult> MyRecipes()
        {
                var userId = (int)HttpContext.Items["UserId"];
                var tokenVersion = (int)HttpContext.Items["TokenVersion"];

                await using(var conn = await _postgresql.GetOpenConnectionAsync())
                {
                        await using(var recipes = new NpgsqlCommand("SELECT * FROM recipe WHERE userid = @userid", conn))
                        {
                                recipes.Parameters.AddWithValue("userid", userId);

                                await using(var reader = await recipes.ExecuteReaderAsync())
                                {
                                        List<RecipeDto> recipelist = new List<RecipeDto>();

                                        while(await reader.ReadAsync())
                                        {
                                                var recipe = new RecipeDto
                                                {
                                                        Id = reader.GetInt32(reader.GetOrdinal("id")),
                                                        Title = reader.GetString(reader.GetOrdinal("title")),
                                                        Carma = reader.IsDBNull(reader.GetOrdinal("rating")) ? 0 : reader.GetInt32(reader.GetOrdinal("rating")),
                                                        Checked = reader.GetBoolean(reader.GetOrdinal("checked"))
                                                };

                                                recipelist.Add(recipe);
                                        }

                                        return Ok(recipelist);
                                }
                        }
                }
        }
}