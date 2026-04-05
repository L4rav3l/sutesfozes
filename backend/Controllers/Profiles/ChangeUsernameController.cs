using System;
using Microsoft.AspNetCore.Mvc;
using SutesFozes.Infrastructure;
using SutesFozes.Models;
using Npgsql;

namespace SutesFozes.Controllers;

[ApiController]
[ServiceFilter(typeof(AuthorizationFilter))]

public class ChangeUsernameController : ControllerBase
{
        private readonly Postgresql _postgresql;
        private readonly Argon2 _argon2;

        public ChangeUsernameController(Postgresql postgresql, Argon2 argon2)
        {
                _postgresql = postgresql;
                _argon2 = argon2;
        }

        [HttpPost("api/profile/change_username")]
        public async Task<IActionResult> ChangeUsername([FromBody] ChangeUsernameRequest request)
        {
                var id = (int)HttpContext.Items["UserId"];
                var tokenVersion = (int)HttpContext.Items["TokenVersion"];

                string username = request.Username;
                string password = request.Password;

                await using(var conn = await _postgresql.GetOpenConnectionAsync())
                {
                        await using(var transaction = await conn.BeginTransactionAsync())
                        {
                                try
                                {
                                        await using(var checkData = new NpgsqlCommand("SELECT * FROM users WHERE id = @id AND tokenversion = @tokenversion", conn))
                                        {
                                                checkData.Parameters.AddWithValue("id", id);
                                                checkData.Parameters.AddWithValue("tokenversion", tokenVersion);

                                                await using(var reader = await checkData.ExecuteReaderAsync())
                                                {
                                                        if(await reader.ReadAsync())
                                                        {
                                                                string salt = reader.GetString(reader.GetOrdinal("salt"));
                                                                string hashedPassword = reader.GetString(reader.GetOrdinal("password"));
                                                                
                                                                string userPassword = _argon2.HashPassword(password, salt);

                                                                if(userPassword == hashedPassword)
                                                                {

                                                                } else {
                                                                        return Unauthorized(new {error = "Invalid credentials."});
                                                                }
                                                        } else {
                                                                return Unauthorized(new {error = "Token was expired or invalid"});
                                                        }
                                                }
                                        }

                                        await using(var updateUser = new NpgsqlCommand("UPDATE users SET username = @username WHERE id = @id AND tokenversion = @tokenversion", conn, transaction))
                                        {
                                                updateUser.Parameters.AddWithValue("username", username);
                                                updateUser.Parameters.AddWithValue("id", id);
                                                updateUser.Parameters.AddWithValue("tokenversion", tokenVersion);

                                                await updateUser.ExecuteNonQueryAsync();
                                        }

                                        await transaction.CommitAsync();
                                        return Ok(new {status = 1});
                                }
                                catch(PostgresException ex) when (ex.SqlState == "23505")
                                {
                                        await transaction.RollbackAsync();
                                        return BadRequest(new {error = "Username is taken."});
                                }
                                catch
                                {
                                        return BadRequest(new {error = "Try again later."});
                                }
                        }
                }
        }
}