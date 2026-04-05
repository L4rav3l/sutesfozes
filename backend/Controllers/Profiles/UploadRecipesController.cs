using System;
using Microsoft.AspNetCore.Mvc;
using SutesFozes.Infrastructure;
using SutesFozes.Models;
using Npgsql;

namespace SutesFozes.Controllers;

[ApiController]
[ServiceFilter(typeof(AuthorizationFilter))]

public class UploadRecipesController : ControllerBase
{
        private readonly Postgresql _postgresql;
        private readonly Cloudflare _cloudflare;

        public UploadRecipesController(Postgresql postgresql, Cloudflare cloudflare)
        {
                _postgresql = postgresql;
                _cloudflare = cloudflare;
        }

        [HttpPost("api/profile/upload_recipe")]
        public async Task<IActionResult> UploadImage([FromForm] UploadImageRequest request)
        {

                IFormFile file = request.File;
                int recipeId = request.Id;

                string link = "link";

                if(file == null || file.Length == 0)
                {
                        return BadRequest(new {error = "Try again"});
                }

                await using(var conn = await _postgresql.GetOpenConnectionAsync())
                {
                        await using(var checkRecipe = new NpgsqlCommand("SELECT * FROM recipe WHERE id = @id AND userid = @userid", conn))
                        {
                                checkRecipe.Parameters.AddWithValue("id", recipeId);
                                checkRecipe.Parameters.AddWithValue("userid", (int)HttpContext.Items["UserId"]);

                                await using(var reader = await checkRecipe.ExecuteReaderAsync())
                                {
                                        if(await reader.ReadAsync())
                                        {
                                                link = await _cloudflare.UploadFile(file);
                                        } else {
                                                return Unauthorized(new {error = "This isn't your recipe."});
                                        }
                                }
                        }

                        await using(var transaction = await conn.BeginTransactionAsync())
                        {
                                try
                                {
                                        await using(var insertImages = new NpgsqlCommand("INSERT INTO recipe_image (recipe_id, image) VALUES (@recipeid, @image)", conn, transaction))
                                        {
                                                insertImages.Parameters.AddWithValue("recipeid", recipeId);
                                                insertImages.Parameters.AddWithValue("image", link);
                                                
                                                await insertImages.ExecuteNonQueryAsync();
                                        }

                                        await transaction.CommitAsync();
                                        return Ok();
                                }

                                catch
                                {
                                        await transaction.RollbackAsync();
                                        return BadRequest(new {error = "try again later."});
                                }
                        }
                }


        }
}