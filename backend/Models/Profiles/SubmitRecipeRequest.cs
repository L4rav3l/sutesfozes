using System.Text.Json;

namespace SutesFozes.Models;

public class SubmitRecipeRequest
{
        public required string Title {get;set;}
        public required int PrepTime {get;set;}
        public required int CookTime {get;set;}
        public required int Serve {get;set;}
        public required JsonElement Ingredients { get; set; }
        public required string Instructions {get;set;}
}