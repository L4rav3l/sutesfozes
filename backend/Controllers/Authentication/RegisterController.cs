using System;
using System.Net.Mail;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using SutesFozes.Infrastructure;
using SutesFozes.Models;
using Npgsql;

namespace SutesFozes.Controllers;

[ApiController]
public class RegisterController : ControllerBase
{
        private readonly Argon2 _argon2;
        private readonly JsonWebToken _jsonwebtoken;
        private readonly Mail _mail;
        private readonly Postgresql _postgresql;

        public RegisterController(Argon2 argon2, JsonWebToken jsonwebtoken, Mail mail, Postgresql postgresql)
        {
                _argon2 = argon2;
                _jsonwebtoken = jsonwebtoken;
                _mail = mail;
                _postgresql = postgresql;
        }

        [HttpPost("api/authentication/register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
                string username = request.Username;
                string email = request.Email;
                string salt = _argon2.GenerateSalt();
                string password = _argon2.HashPassword(request.Password, salt);

                try
                {
                        var addr = new MailAddress(email);
                }
                catch 
                {
                        return BadRequest(new {error = "Invalid email format."});
                }

                using(var conn = await _postgresql.GetOpenConnectionAsync())
                {
                        await using(var transaction = await conn.BeginTransactionAsync())
                        {
                                try
                                {
                                        await using(var userInsert = new NpgsqlCommand("INSERT INTO users (email, username, salt, password) VALUES (@email, @username, @salt, @password) RETURNING id", conn, transaction))
                                        {
                                                userInsert.Parameters.AddWithValue("email", email);
                                                userInsert.Parameters.AddWithValue("username", username);
                                                userInsert.Parameters.AddWithValue("password", password);
                                                userInsert.Parameters.AddWithValue("salt", salt);

                                                await using(var reader = await userInsert.ExecuteReaderAsync())
                                                {
                                                        if(await reader.ReadAsync())
                                                        {
                                                                int id = reader.GetInt32(reader.GetOrdinal("id"));

                                                                string token = _jsonwebtoken.GenerateToken(id, 0, 30);

                                                                using(var client = _mail.CreateSMTPClient())
                                                                {
                                                                        var message = new MimeMessage();

                                                                        message.From.Add(new MailboxAddress("SutesFozes", Environment.GetEnvironmentVariable("SMTP_USERNAME")));
                                                                        message.To.Add(new MailboxAddress(username, email));
                                                                        message.Subject = "Sutesfozes registration";
                                                                        message.Body = new TextPart("plain")
                                                                        {
                                                                                Text = 
                                                                                $"Hi {username}\n\n" +
                                                                                $"Please activate your account by clicking this link: {Environment.GetEnvironmentVariable("PRODUCT_LINK")}/users/verify?token={token}\n" +
                                                                                "This link is valid for 30 minutes.\n\n" +
                                                                                "SutesFozes"
                                                                        };
                                                                        
                                                                        client.Send(message);
                                                                        client.Disconnect(true);
                                                                }
                                                        }
                                                }
                                        }

                                        await transaction.CommitAsync();

                                        return Ok();
                                }
                                catch(PostgresException ex) when (ex.SqlState == "23505")
                                {
                                        await transaction.RollbackAsync();
                                        return BadRequest(new {error = "Email or Username is taken."});
                                }
                                catch(Exception ex)
                                {
                                        Console.WriteLine(ex);
                                        await transaction.RollbackAsync();
                                        return BadRequest(new {error = "Try again later."});
                                }
                        }
                }

                return BadRequest(new {error = "Try again later."});
        }
}