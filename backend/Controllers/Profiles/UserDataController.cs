using System;
using Microsoft.AspNetCore.Mvc;
using SutesFozes.Infrastructure;
using SutesFozes.Models;
using Npgsql;

namespace SutesFozes.Controllers;

[ApiController]
[ServiceFilter(typeof(AuthorizationFilter))]

public class UserDataController : ControllerBase
{
        private readonly Postgresql _postgresql;
        
        public UserDataController(Postgresql postgresql)
        {
                _postgresql = postgresql;
        }

        [HttpGet("api/profile/data")]
        public async Task<IActionResult> GetUserData()
        {
                var id = (int)HttpContext.Items["UserId"];
                var tokenVersion =  (int)HttpContext.Items["TokenVersion"];

                await using(var conn = await _postgresql.GetOpenConnectionAsync())
                {
                        await using(var userData = new NpgsqlCommand("SELECT * FROM users WHERE id = @id AND tokenversion = @tokenversion", conn))
                        {
                                userData.Parameters.AddWithValue("id", id);
                                userData.Parameters.AddWithValue("tokenversion", tokenVersion);

                                await using(var reader = await userData.ExecuteReaderAsync())
                                {
                                        if(await reader.ReadAsync())
                                        {
                                                string username = reader.GetString(reader.GetOrdinal("username"));
                                                string email = reader.GetString(reader.GetOrdinal("email"));

                                                return Ok(new {username, email});
                                        } else {
                                                return Unauthorized(new {error = "Token was expired or invalid."});
                                        }
                                }
                        }
                }

                return BadRequest(new {error = "Try again later."});
        }

}