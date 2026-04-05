namespace SutesFozes.Models;

public class UploadImageRequest
{
        public required IFormFile File {get;set;}
        public required int Id {get;set;}
}