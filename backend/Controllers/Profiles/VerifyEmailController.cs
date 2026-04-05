using System;
using Microsoft.AspNetCore.Mvc;
using SutesFozes.Infrastructure;
using SutesFozes.Models;
using Npgsql;

[ApiController]
public class VerifyEmailController : ControllerBase
{
        private readonly Postgresql _postgresql;
        private readonly JsonWebToken _jsonwebtoken;

        public VerifyEmailController(Postgresql postgresql, JsonWebToken jsonwebtoken)
        {
                _postgresql = postgresql;
                _jsonwebtoken = jsonwebtoken;
        }

        [HttpPost("api/profile/verify_email")]
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailRequest request)
        {
                string token = request.Token;

                var data = _jsonwebtoken.VerifyToken(token);

                if(!data.HasValue)
                {
                        return Unauthorized(new {error = "The token was expired or invalid."});
                }

                int? id = data.Value.id;
                int? tokenVersion = data.Value.tokenVersion;
                string email = null;

                await using(var conn = await _postgresql.GetOpenConnectionAsync())
                {
                        await using(var transaction = await conn.BeginTransactionAsync())
                        {
                                try
                                {
                                        await using(var getChange = new NpgsqlCommand("SELECT * FROM emailchange WHERE token = @token", conn))
                                        {
                                                getChange.Parameters.AddWithValue("token", token);

                                                await using(var reader = await getChange.ExecuteReaderAsync())
                                                {
                                                        if(await reader.ReadAsync())
                                                        {
                                                                email = reader.GetString(reader.GetOrdinal("email"));
                                                        } else {
                                                                return NotFound(new {error = "The token was not found."});
                                                        }
                                                }
                                        }

                                        await using(var updateEmail = new NpgsqlCommand("UPDATE users SET email = @email, tokenversion = tokenversion + 1 WHERE id = @id AND tokenversion = @tokenversion", conn, transaction))
                                        {
                                                updateEmail.Parameters.AddWithValue("email", email);
                                                updateEmail.Parameters.AddWithValue("id", id);
                                                updateEmail.Parameters.AddWithValue("tokenversion", tokenVersion);

                                                await updateEmail.ExecuteNonQueryAsync();
                                        }

                                        await transaction.CommitAsync();
                                        return Ok(new {status = 1});
                                }
                                catch(PostgresException ex) when (ex.SqlState == "23505")
                                {
                                        await transaction.RollbackAsync();
                                        return BadRequest(new {error = "Email was taken."});
                                }
                                catch(Exception ex)
                                {
                                        Console.WriteLine(ex);
                                        return BadRequest(new {error = "Try again later."});
                                }
                        }
                }
        }
}