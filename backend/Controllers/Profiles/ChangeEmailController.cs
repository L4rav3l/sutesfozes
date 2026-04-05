using System;
using System.Net.Mail;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using SutesFozes.Infrastructure;
using SutesFozes.Models;
using Npgsql;

namespace SutesFozes.Controllers;

[ApiController]
[ServiceFilter(typeof(AuthorizationFilter))]
public class ChangeEmailController : ControllerBase
{
        private readonly Postgresql _postgresql;
        private readonly Argon2 _argon2;
        private readonly JsonWebToken _jsonwebtoken;
        private readonly Mail _mail;

        public ChangeEmailController(Postgresql postgresql, Argon2 argon2, JsonWebToken jsonwebtoken, Mail mail)
        {
                _postgresql = postgresql;
                _argon2 = argon2;
                _jsonwebtoken = jsonwebtoken;
                _mail = mail;
        }

        [HttpPost("api/profile/change_email")]
        public async Task<IActionResult> ChangeEmail([FromBody] ChangeEmailRequest request)
        {
                string email = request.Email;
                string password = request.Password;

                var userId = (int)HttpContext.Items["UserId"];
                var tokenVersion = (int)HttpContext.Items["TokenVersion"];

                try
                {
                        var address = new MailAddress(email);
                }

                catch
                {
                        return BadRequest(new {error = "Invalid email format."});
                }

                string token = _jsonwebtoken.GenerateToken(userId, tokenVersion, 30);

                await using(var conn = await _postgresql.GetOpenConnectionAsync())
                {
                        await using(var transaction = await conn.BeginTransactionAsync())
                        {
                                try
                                {
                                        await using(var userData = new NpgsqlCommand("SELECT * FROM users WHERE id = @id AND tokenversion = @tokenversion", conn))
                                        {
                                                userData.Parameters.AddWithValue("id", userId);
                                                userData.Parameters.AddWithValue("tokenversion", tokenVersion);

                                                await using(var reader = await userData.ExecuteReaderAsync())
                                                {
                                                        if(await reader.ReadAsync())
                                                        {
                                                                string encryptedPassword = reader.GetString(reader.GetOrdinal("password"));    
                                                                string salt = reader.GetString(reader.GetOrdinal("salt"));

                                                                string userPassword = _argon2.HashPassword(password, salt);

                                                                if(userPassword != encryptedPassword)
                                                                {
                                                                        return Unauthorized(new {error = "The password was wrong."});
                                                                }

                                                        } else {
                                                                return Unauthorized();
                                                        }
                                                }
                                        }

                                        await using(var checkEmail = new NpgsqlCommand("SELECT * FROM users WHERE email = @email", conn))
                                        {
                                                checkEmail.Parameters.AddWithValue("email", email);

                                                await using(var reader = await checkEmail.ExecuteReaderAsync())
                                                {
                                                        if(await reader.ReadAsync())
                                                        {
                                                                return BadRequest(new {error = "Email was taken."});
                                                        }
                                                }
                                        }

                                        await using(var emailChange = new NpgsqlCommand("INSERT INTO emailchange (userid, token, email) VALUES (@id, @token, @email)", conn, transaction))
                                        {
                                                emailChange.Parameters.AddWithValue("id", userId);
                                                emailChange.Parameters.AddWithValue("token", token);
                                                emailChange.Parameters.AddWithValue("email", email);

                                                await emailChange.ExecuteNonQueryAsync();
                                        }

                                        using(var client = _mail.CreateSMTPClient())
                                        {
                                                var message = new MimeMessage();

                                                message.From.Add(new MailboxAddress("SutesFozes", Environment.GetEnvironmentVariable("SMTP_USERNAME")));
                                                message.To.Add(new MailboxAddress("", email));
                                                message.Subject = "Email change";
                                                message.Body = new TextPart("plain")
                                                {
                                                        Text =
                                                        $"Hi username,\n" +
                                                        "This is your email address change confirm link. If you click the button, your email address will change.\n" +
                                                        $"{Environment.GetEnvironmentVariable("PRODUCT_LINK")}/profile/change_address?token={token}\n" +
                                                        "SutesFozes"  
                                                };

                                                client.Send(message);
                                                client.Disconnect(true);
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

                return BadRequest();
        }
}