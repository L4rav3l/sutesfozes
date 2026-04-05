using System;
using Microsoft.AspNetCore.Mvc;
using SutesFozes.Infrastructure;
using SutesFozes.Models;
using Npgsql;

namespace SutesFozes.Controllers;

[ApiController]
[ServiceFilter(typeof(AuthorizationFilter))]

public class FavouriteRecipe : ControllerBase
{
        private readonly Postgresql _postgresql;

        public FavouriteRecipe(Postgresql postgresql)
        {
                _postgresql = postgresql;
        }

        [HttpGet("api/profile/favourite")]
        public async Task<IActionResult> Favourite([FromQuery] RecipeRequest request)
        {
                var userid = (int)HttpContext.Items["UserId"];

                await using(var conn = await _postgresql.GetOpenConnectionAsync())
                {
                        await using(var favouritecheck = new NpgsqlCommand("SELECT * FROM favourite_recipe WHERE userid = @userid AND recipeid = @recipeid", conn))
                        {
                                favouritecheck.Parameters.AddWithValue("userid", userid);
                                favouritecheck.Parameters.AddWithValue("recipeid", request.Id);

                                await using(var reader = await favouritecheck.ExecuteReaderAsync())
                                {
                                        if(await reader.ReadAsync())
                                        {
                                                return Ok(new {status = true});
                                        } else {
                                                return Ok(new {status = false});
                                        }
                                }
                        }
                }
        }
}