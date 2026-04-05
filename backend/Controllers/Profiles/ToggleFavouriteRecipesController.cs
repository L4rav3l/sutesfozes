using System;
using Microsoft.AspNetCore.Mvc;
using SutesFozes.Infrastructure;
using SutesFozes.Models;
using Npgsql;

namespace SutesFozes.Controllers;

[ApiController]
[ServiceFilter(typeof(AuthorizationFilter))]

public class ToggleFavouriteRecipesController : ControllerBase
{
        private readonly Postgresql _postgresql;

        public ToggleFavouriteRecipesController(Postgresql postgresql)
        {
                _postgresql = postgresql;
        }

        [HttpPost("api/profile/toggle_favourite")]
        public async Task<IActionResult> ToggleFavourite([FromBody] ToggleFavouriteRequest request)
        {
                int recipeId = request.Id;
                bool status = request.Status;
                
                var userId = (int)HttpContext.Items["UserId"];
                var tokenVersion = (int)HttpContext.Items["TokenVersion"];

                await using(var conn = await _postgresql.GetOpenConnectionAsync())
                {
                        await using(var transaction = await conn.BeginTransactionAsync())
                        {
                                try
                                {
                                        if(status == true)
                                        {
                                                await using(var checkFavourite = new NpgsqlCommand("SELECT * FROM favourite_recipe WHERE userid = @userid AND recipeid = @recipeid", conn))
                                                {
                                                        checkFavourite.Parameters.AddWithValue("userid", userId);
                                                        checkFavourite.Parameters.AddWithValue("recipeid", recipeId);

                                                        await using(var reader = await checkFavourite.ExecuteReaderAsync())
                                                        {
                                                                if(await reader.ReadAsync())
                                                                {
                                                                        return Ok( new {status = 1});
                                                                }
                                                        }
                                                }

                                                await using(var insertFavourite = new NpgsqlCommand("INSERT INTO favourite_recipe (userid, recipeid) VALUES (@userid, @recipeid)", conn, transaction))
                                                {
                                                        insertFavourite.Parameters.AddWithValue("userid", userId);
                                                        insertFavourite.Parameters.AddWithValue("recipeid", recipeId);

                                                        await insertFavourite.ExecuteNonQueryAsync();
                                                }
                                        }

                                        if(status == false)
                                        {
                                                await using(var deleteFavourite = new NpgsqlCommand("DELETE FROM favourite_recipe WHERE userid = @userid AND recipeid = @recipeid", conn, transaction))
                                                {
                                                        deleteFavourite.Parameters.AddWithValue("userid", userId);
                                                        deleteFavourite.Parameters.AddWithValue("recipeid", recipeId);

                                                        await deleteFavourite.ExecuteNonQueryAsync();
                                                }
                                        }

                                        await transaction.CommitAsync();

                                        return Ok(new {status = 1});
                                }

                                catch
                                {
                                        await transaction.RollbackAsync();

                                        return BadRequest(new {error = "Try again later."});
                                }
                        }
                }
        }
}
