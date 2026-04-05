using System;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using SutesFozes.Models;
using SutesFozes.Infrastructure;
using Npgsql;

namespace SutesFozes.Controllers;

[ApiController]
[ServiceFilter(typeof(AuthorizationFilter))]

public class SubmitRecipesController : ControllerBase
{
        private readonly Postgresql _postgresql;

        public SubmitRecipesController(Postgresql postgresql)
        {
                _postgresql = postgresql;
        }

        [HttpPost("api/profile/submit_recipe")]
        public async Task<IActionResult> SubmitRecipe([FromBody] SubmitRecipeRequest request)
        {
                string title = request.Title;
                int prepTime = request.PrepTime;
                int cookTime = request.CookTime;
                int serve = request.Serve;
                JsonElement ingredients = request.Ingredients;
                string instructions = request.Instructions;

                var userId = (int)HttpContext.Items["UserId"];
                var tokenVersion = (int)HttpContext.Items["TokenVersion"];

                int recipeid = 0;

                await using(var conn = await _postgresql.GetOpenConnectionAsync())
                {
                        await using(var transaction = await conn.BeginTransactionAsync())
                        {
                                try
                                {
                                        await using(var insertRecipe = new NpgsqlCommand("INSERT INTO recipe (userid, title, preptime, cooktime, servings, ingredients, instructions) VALUES (@userid, @title, @preptime, @cooktime, @servings, @ingredients, @instructions) RETURNING id", conn, transaction))
                                        {
                                                insertRecipe.Parameters.AddWithValue("userid", userId);
                                                insertRecipe.Parameters.AddWithValue("title", title);
                                                insertRecipe.Parameters.AddWithValue("preptime", prepTime);
                                                insertRecipe.Parameters.AddWithValue("cooktime", cookTime);
                                                insertRecipe.Parameters.AddWithValue("servings", serve);
                                                insertRecipe.Parameters.AddWithValue("ingredients", ingredients);
                                                insertRecipe.Parameters.AddWithValue("instructions", instructions);

                                                await using(var reader = await insertRecipe.ExecuteReaderAsync())
                                                {
                                                        if(await reader.ReadAsync())
                                                        {
                                                                recipeid = reader.GetInt32(reader.GetOrdinal("id"));
                                                        }
                                                }
                                        }

                                        await transaction.CommitAsync();
                                        return Ok(new {status = 1, id = recipeid});
                                }

                                catch(Exception ex)
                                {
                                        Console.WriteLine(ex);
                                        await transaction.RollbackAsync();
                                        return BadRequest(new {error = "try again later."});
                                }
                        }
                }

                return BadRequest(new {error = "try again later."});
        }
}