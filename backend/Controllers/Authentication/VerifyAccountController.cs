using System;
using Microsoft.AspNetCore.Mvc;
using SutesFozes.Models;
using SutesFozes.Infrastructure;
using Npgsql;

namespace SutesFozes.Controllers;

[ApiController]
public class VerifyAccountController : ControllerBase
{
        private readonly JsonWebToken _jsonwebtoken;
        private readonly Postgresql _postgresql;

        public VerifyAccountController(JsonWebToken jsonwebtoken, Postgresql postgresql)
        {
                _jsonwebtoken = jsonwebtoken;
                _postgresql = postgresql;
        }

        [HttpPost("api/authentication/verify")]
        public async Task<IActionResult> VerifyAccount([FromBody] VerifyAccountRequest request)
        {
                var token = request.Token;
                bool success = false;

                var informationOfToken = _jsonwebtoken.VerifyToken(token);

                if(informationOfToken != null)
                {
                        int? id = informationOfToken.Value.id;
                        int? tokenVersion = informationOfToken.Value.tokenVersion;

                        if(tokenVersion == 0)
                        {
                                await using(var conn = await _postgresql.GetOpenConnectionAsync())
                                {
                                        await using(var transaction = await conn.BeginTransactionAsync())
                                        {
                                                try
                                                {
                                                        await using(var verifyUser = new NpgsqlCommand("UPDATE users  SET tokenVersion = 1, verify = TRUE  WHERE tokenVersion = 0 AND id = @id RETURNING id", conn, transaction))
                                                        {
                                                                verifyUser.Parameters.AddWithValue("@id", id);

                                                                await using(var reader = await verifyUser.ExecuteReaderAsync())
                                                                {
                                                                        if(await reader.ReadAsync())
                                                                        {
                                                                                success = true;
                                                                        } else {
                                                                                success = false;
                                                                        }
                                                                }
                                                        }

                                                        if(success)
                                                        {
                                                                await transaction.CommitAsync();
                                                                return Ok(new {status = 1});
                                                        } else {
                                                                await transaction.RollbackAsync();
                                                                return Unauthorized(new {error = "The token was expired or invalid."});
                                                        }

                                                }

                                                catch(Exception ex)
                                                {
                                                        Console.WriteLine(ex);
                                                        await transaction.RollbackAsync();
                                                        return BadRequest();
                                                }
                                        }
                                }
                        } else {
                                return Unauthorized(new {error = "Your account has been verified."});
                        }
                } else {
                        return Unauthorized(new {error = "The token was expired or invalid."});
                }

                return BadRequest(new {error = "Try again later."});
        }
}