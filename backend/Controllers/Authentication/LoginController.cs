using System;
using System.Net.Mail;
using MimeKit;
using Microsoft.AspNetCore.Mvc;
using SutesFozes.Infrastructure;
using SutesFozes.Models;
using Npgsql;

namespace SutesFozes.Controllers;

[ApiController]
public class LoginController : ControllerBase
{
        private readonly Postgresql _postgresql;
        private readonly Argon2 _argon2;
        private readonly JsonWebToken _jsonwebtoken;
        private readonly Mail _mail;

        public LoginController(Postgresql postgresql, Argon2 argon2, JsonWebToken jsonwebtoken, Mail mail)
        {
                _postgresql = postgresql;
                _argon2 = argon2;
                _jsonwebtoken = jsonwebtoken;
                _mail = mail;
        }

        [HttpPost("api/authentication/login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
                string identification = request.Identification;
                string password = request.Password;

                string loginSQL = "";

                try
                {
                        var addr = new MailAddress(identification);
                        loginSQL = "SELECT * FROM users WHERE email = @identification";
                }
                catch
                {
                        loginSQL = "SELECT * FROM users WHERE username = @identification";
                }
                

                await using(var conn = await _postgresql.GetOpenConnectionAsync())
                {
                        await using(var loginUser = new NpgsqlCommand(loginSQL, conn))
                        {
                                loginUser.Parameters.AddWithValue("identification", identification);

                                await using(var reader = await loginUser.ExecuteReaderAsync())
                                {
                                        if(await reader.ReadAsync())
                                        {
                                                int id = reader.GetInt32(reader.GetOrdinal("id"));
                                                int tokenVersion = reader.GetInt32(reader.GetOrdinal("tokenversion"));
                                                bool isVerify = reader.GetBoolean(reader.GetOrdinal("verify"));
                                                string encryptedPassword = reader.GetString(reader.GetOrdinal("password"));
                                                string salt = reader.GetString(reader.GetOrdinal("salt"));
                                                string username = reader.GetString(reader.GetOrdinal("username"));
                                                string email = reader.GetString(reader.GetOrdinal("email"));

                                                string userPassword = _argon2.HashPassword(password, salt);

                                                if(userPassword == encryptedPassword)
                                                {
                                                        if(tokenVersion > 0 && isVerify)
                                                        {
                                                                string token = _jsonwebtoken.GenerateToken(id, tokenVersion, 365 * 24 * 60);
                                                                        
                                                                return Ok(new {status = 1, token = token});
                                                        } else {

                                                                string token = _jsonwebtoken.GenerateToken(id, tokenVersion, 30);

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
                                                                                $"Please activate your account by clicking this link: {Environment.GetEnvironmentVariable("PRODUCT_LINK")}/profile/activate_account?token={token}\n" +
                                                                                "This link is valid for 30 minutes.\n\n" +
                                                                                "SutesFozes"
                                                                        };
                                                                                
                                                                        client.Send(message);
                                                                        client.Disconnect(true);

                                                                        return Unauthorized(new {error = "Your account hasn't been activated."});
                                                                }

                                                        }
                                                } else {
                                                        return Unauthorized(new {error = "Username or Password was invalid."});
                                                }

                                        } else {
                                                return Unauthorized(new { error = "Invalid credentials" });
                                        }
                                }
                        }
                }
        }
}