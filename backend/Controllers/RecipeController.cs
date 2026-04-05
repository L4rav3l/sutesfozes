using System;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using SutesFozes.Infrastructure;
using SutesFozes.Models;
using Npgsql;

namespace SutesFozes.Controllers;

[ApiController]
public class RecipeController : ControllerBase
{
        private readonly Postgresql _postgresql;

        public RecipeController(Postgresql postgresql)
        {
                _postgresql = postgresql;
        }

        [HttpGet("api/recipes")]
        public async Task<IActionResult> Recipe([FromQuery] RecipeRequest request)
        {
                int id = request.Id;

                string title = "title";

                int prepTime = 0;
                int cookTime = 0;
                int serve = 0;

                var ingredients = "ingredients";
                string instructions = "instructions";

                List<string> images = new List<string>();

                await using(var conn = await _postgresql.GetOpenConnectionAsync())
                {
                        await using(var get = new NpgsqlCommand("SELECT * FROM recipe WHERE id = @id AND checked = true", conn))
                        {
                                get.Parameters.AddWithValue("id", id);

                                await using(var reader = await get.ExecuteReaderAsync())
                                {
                                        if(await reader.ReadAsync())
                                        {
                                                title = reader.GetString(reader.GetOrdinal("title"));

                                                prepTime = reader.GetInt32(reader.GetOrdinal("preptime"));
                                                cookTime = reader.GetInt32(reader.GetOrdinal("cooktime"));
                                                serve = reader.GetInt32(reader.GetOrdinal("servings"));

                                                ingredients = reader.GetString(reader.GetOrdinal("ingredients"));
                                                instructions = reader.GetString(reader.GetOrdinal("instructions"));

                                        } else {        
                                                return NotFound();
                                        }
                                }
                        }

                        await using(var getImages = new NpgsqlCommand("SELECT * FROM recipe_image WHERE recipe_id = @recipeid", conn))
                        {
                                getImages.Parameters.AddWithValue("recipeid", id);

                                await using(var reader = await getImages.ExecuteReaderAsync())
                                {
                                        while(await reader.ReadAsync())
                                        {
                                                images.Add(reader.GetString(reader.GetOrdinal("image")));
                                        }
                                }
                        }
                }
                
                return Ok(new {title, cookTime, prepTime, serve, ingredients, instructions, images});
        }
}