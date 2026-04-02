using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SutesFozes.Infrastructure;
using Npgsql;

namespace SutesFozes.Infrastructure;

public class AuthorizationFilter : IAsyncAuthorizationFilter
{
        private readonly Postgresql _postgresql;
        private readonly JsonWebToken _jsonwebtoken;

        public AuthorizationFilter(Postgresql postgresql, JsonWebToken jsonwebtoken)
        {
                _postgresql = postgresql;
                _jsonwebtoken = jsonwebtoken;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
                var request = context.HttpContext.Request;

                if(!request.Headers.ContainsKey("Authorization"))
                {
                        context.Result = new UnauthorizedResult();
                        return;
                }

                var token = request.Headers["Authorization"].ToString().Replace("Bearer ", "");
                
                var informationOfToken = _jsonwebtoken.VerifyToken(token);

                if(!informationOfToken.HasValue)
                {
                        context.Result = new UnauthorizedResult();
                        return;
                } else {
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
                                                        context.HttpContext.Items["UserId"] = id;
                                                        context.HttpContext.Items["TokenVersion"] = tokenVersion;
                                                } else {
                                                        context.Result = new UnauthorizedResult();
                                                        return;
                                                }
                                        }
                                }
                        }
                }
        }
}

