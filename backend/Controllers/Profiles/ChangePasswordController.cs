using System;
using Microsoft.AspNetCore.Mvc;
using SutesFozes.Infrastructure;
using SutesFozes.Models;
using Npgsql;

namespace SutesFozes.Controllers;

[ApiController]
[ServiceFilter(typeof(AuthorizationFilter))]

public class ChangePasswordController : ControllerBase
{
        private readonly Argon2 _argon2;
        private readonly Postgresql _postgresql;

        public ChangePasswordController(Argon2 argon2, Postgresql postgresql)
        {
                _argon2 = argon2;
                _postgresql = postgresql;
        }

        [HttpPost("api/profile/change_password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
                var userId = (int)HttpContext.Items["UserId"];
                var tokenVersion = (int)HttpContext.Items["TokenVersion"];

                string password = request.Password;
                string newPassword = request.NewPassword;

                string newSalt = _argon2.GenerateSalt();
                string newHashedPassword = _argon2.HashPassword(newPassword, newSalt);

                await using(var conn = await _postgresql.GetOpenConnectionAsync())
                {
                        await using(var transaction = await conn.BeginTransactionAsync())
                        {
                                try
                                {
                                        await using(var checkPassword = new NpgsqlCommand("SELECT * FROM users WHERE id = @id AND tokenversion = @tokenversion", conn, transaction))
                                        {
                                                checkPassword.Parameters.AddWithValue("id", userId);
                                                checkPassword.Parameters.AddWithValue("tokenversion", tokenVersion);

                                                await using(var reader = await checkPassword.ExecuteReaderAsync())
                                                {
                                                        if(await reader.ReadAsync())
                                                        {
                                                                string salt = reader.GetString(reader.GetOrdinal("salt"));
                                                                string HashedPassword = reader.GetString(reader.GetOrdinal("password"));

                                                                string userPassword = _argon2.HashPassword(password, salt);

                                                                if(userPassword == HashedPassword)
                                                                {
                                                                        
                                                                } else {
                                                                        return Unauthorized(new {error = "Invalid credentials"});
                                                                }

                                                        } else {
                                                                return Unauthorized();
                                                        }
                                                }
                                        }

                                        await using(var updatePassword = new NpgsqlCommand("UPDATE users SET password = @password, salt = @salt, tokenversion = tokenversion + 1 WHERE id = @id AND tokenversion = @tokenversion", conn, transaction))
                                        {
                                                updatePassword.Parameters.AddWithValue("password", newHashedPassword);
                                                updatePassword.Parameters.AddWithValue("salt", newSalt);
                                                updatePassword.Parameters.AddWithValue("id", userId);
                                                updatePassword.Parameters.AddWithValue("tokenversion", tokenVersion);

                                                await updatePassword.ExecuteNonQueryAsync();
                                        }

                                        await transaction.CommitAsync();

                                        return Ok(new {status = 1});
                                }
                                catch(Exception ex)
                                {
                                        Console.WriteLine(ex);
                                        await transaction.RollbackAsync();
                                        return BadRequest(new {error = "Try again later."});
                                }
                        }
                }


        }
}