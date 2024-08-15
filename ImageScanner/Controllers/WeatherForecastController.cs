using Microsoft.AspNetCore.Mvc;
using Tesseract;

namespace ImageScanner.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        // private static readonly string[] Summaries = new[]
        // {
        //     "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        // };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet("/Scan")]
        public string ScanImage()
        {
            // Initialize the Tesseract engine
            using var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default);

            // Load the image from file
            using var img = Pix.LoadFromFile("Images/toScan.png");

            // Perform OCR on the image
            using var page = engine.Process(img);
            var text = page.GetText();

            // Output the extracted text
            Console.WriteLine("Extracted Text:");
            Console.WriteLine(text);
            return ""; 
        }
    }
}
