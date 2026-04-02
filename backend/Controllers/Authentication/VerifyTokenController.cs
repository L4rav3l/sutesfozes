using System;
using Microsoft.AspNetCore.Mvc;
using SutesFozes.Infrastructure;
using SutesFozes.Models;
using Npgsql;

namespace SutesFozes.Controller;

[ApiController]
public class VerifyTokenController : ControllerBase
{
        private readonly Postgresql _postgresql;
        private readonly JsonWebToken _jsonwebtoken;

        public VerifyTokenController(Postgresql postgresql, JsonWebToken jsonwebtoken)
        {
                _postgresql = postgresql;
                _jsonwebtoken = jsonwebtoken;
        }

        [HttpPost("api/authentication/verify_token")]
        public async Task<IActionResult> VerifyToken([FromBody] VerifyTokenRequest request)
        {
                string token = request.Token;

                var informationOfToken = _jsonwebtoken.VerifyToken(token);

                if (!informationOfToken.HasValue)
                {
                        return Unauthorized(new { error = "Invalid token." });
                }

                int? id = informationOfToken.Value.id;
                int? tokenVersion = informationOfToken.Value.tokenVersion;

                await using(var conn = await _postgresql.GetOpenConnectionAsync())
                {
                        await using(var checkToken = new NpgsqlCommand("SELECT 1 FROM users WHERE id = @id AND tokenversion = @tokenversion", conn))
                        {
                                checkToken.Parameters.AddWithValue("id", id);
                                checkToken.Parameters.AddWithValue("tokenversion", tokenVersion);

                                await using(var reader = await checkToken.ExecuteReaderAsync())
                                {
                                        if(await reader.ReadAsync())
                                        {
                                                     return Ok(new {status = 1});
                                        } else {
                                                       return Unauthorized(new {error = "Token was expired or invalid."});
                                        }
                                }
                        }
                }
                
        }
}