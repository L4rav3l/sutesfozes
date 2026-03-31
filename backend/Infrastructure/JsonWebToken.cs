using System;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using SutesFozes.Infrastructure;
using Microsoft.IdentityModel.Tokens;
using Npgsql;

namespace SutesFozes.Infrastructure;

public class JsonWebToken
{
        private readonly string _jwtSecret;

        public JsonWebToken()
        {
                _jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET");
        }

        public string GenerateToken(int id, int tokenVersion, int minutes)
        {
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
                
                var claims = new List<Claim>();

                claims.Add(new Claim("id", id.ToString()));
                claims.Add(new Claim("tokenVersion", tokenVersion.ToString()));

                var token = new JwtSecurityToken(
                        issuer: "sutesfozes",
                        audience: "sutesfozes-api",
                        claims: claims,
                        expires: DateTime.UtcNow.AddMinutes(minutes),
                        signingCredentials: credentials
                );

                return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public (int? id, int? tokenVersion)? VerifyToken(string token)
        {
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSecret));

                var validationParameters = new TokenValidationParameters
                {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = securityKey,
                        ValidateIssuer = true,
                        ValidIssuer = "sutesfozes",
                        ValidateAudience = true,
                        ValidAudience = "sutesfozes-api",
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                };

                try
                {
                        var principal = new JwtSecurityTokenHandler().ValidateToken(
                                token,
                                validationParameters,
                                out SecurityToken validatedtoken
                        );

                        int id = int.Parse(principal.FindFirst("id")?.Value);
                        int tokenVersion = int.Parse(principal.FindFirst("tokenVersion")?.Value);

                        return (id, tokenVersion);

                }
                catch(Exception ex)
                {
                        return null;
                }
        }
}