// Protocol/FileSearchController.cs
using Microsoft.AspNetCore.Mvc;
using FileSearchService.Context;
using FileSearchService.Model;
using System.Text.Json;

namespace FileSearchService.Protocol
{
    [ApiController]
    [Route("[controller]")]
    public class SearchController : ControllerBase
    {
        private readonly FileSearchContext _context;
        private const string ResultsFolder = @"C:\search-result";
        private const string ResultsFile = "search-result.json";

        public SearchController(FileSearchContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Search([FromQuery] string q)
        {
            var parts = q.Split(',');
            if (parts.Length != 2) return BadRequest("Invalid query format");
            
            var drive = parts[0].Trim().ToUpper();
            var searchTerm = parts[1].Trim();

            Console.WriteLine($"Search on disk started {drive}: template: '{searchTerm}'");

            var results = _context.SearchFiles(drive, searchTerm).ToList();
            
            SaveResultsToFile(results);

            Console.WriteLine($"Search completed. Found {results.Count} matches");
            
            return results.Any() 
                ? Ok(results) 
                : NotFound(new { Message = "Matches not found" });
        }

        private void SaveResultsToFile(List<FileInfoResponse> results)
        {
            try
            {
                Directory.CreateDirectory(ResultsFolder);
                var path = Path.Combine(ResultsFolder, ResultsFile);
                
                var options = new JsonSerializerOptions 
                { 
                    WriteIndented = true,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                };
                
                System.IO.File.WriteAllText(path, JsonSerializer.Serialize(results, options));
                Console.WriteLine($"Results saved in {path}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Saving error: {ex.Message}");
            }
        }
    }
}

