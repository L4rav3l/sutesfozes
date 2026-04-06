using System;
using Microsoft.AspNetCore.Mvc;
using SutesFozes.Models;
using SutesFozes.Infrastructure;
using Npgsql;

namespace SutesFozes.Controllers;

public class SearchDto {
        public required int Id {get;set;}
        public required string Title {get; set;}
}


[ApiController]

public class SearchController : ControllerBase
{
        private readonly Postgresql _postgresql;

        public SearchController(Postgresql postgresql)
        {
                _postgresql = postgresql;
        }

        [HttpPost("api/search")]
        public async Task<IActionResult> Search([FromBody] SearchRequest request)
        {
                string search = request.Search;

                List<SearchDto> recipes = new List<SearchDto>();

                await using(var conn = await _postgresql.GetOpenConnectionAsync())
                {
                        await using(var searchGet = new NpgsqlCommand("SELECT * FROM recipe WHERE title LIKE '%' || @title || '%'", conn))
                        {
                                searchGet.Parameters.AddWithValue("title", search);

                                await using(var reader = await searchGet.ExecuteReaderAsync())
                                {
                                        while(await reader.ReadAsync())
                                        {
                                                var recipe = new SearchDto
                                                {
                                                        Id = reader.GetInt32(reader.GetOrdinal("id")),
                                                        Title = reader.GetString(reader.GetOrdinal("title"))
                                                };

                                                recipes.Add(recipe);
                                        }               
                                }
                        }
                }

                return Ok(recipes);
        }
}