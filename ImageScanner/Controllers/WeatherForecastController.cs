using ImageMagick;
using Microsoft.AspNetCore.Mvc;
using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.IO;
using Tesseract;

namespace ImageScanner.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
             "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
         };

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
            using var engine = new TesseractEngine(@"./tessdata", "eng", EngineMode.Default);
            using var document = PdfReader.Open("Images/pdfToScan.pdf", PdfDocumentOpenMode.Import);
            using var pdfPageStream = new MemoryStream();
            var pdfPage = document.Pages[0];

            var pdfDocument = new PdfDocument();

            pdfDocument.AddPage(pdfPage);

            pdfDocument.Save(pdfPageStream, false);

            using var images = new MagickImageCollection();

            images.Read(pdfPageStream, new MagickReadSettings
            {
                Density = new Density(300, 300),
                FrameIndex = 0,
                FrameCount = 1
            });

            using var image = images[0];
            using var img = new MemoryStream();
            image.Write(img, MagickFormat.Png);
            img.Position = 0;

            using var pix = Pix.LoadFromMemory(img.ToArray());


            // using var img = Pix.LoadFromFile("Images/toScan.png");

            using var page = engine.Process(pix);
            var text = page.GetText();

            Console.WriteLine("Extracted Text:");
            Console.WriteLine(text);
            return ""; 
        }
    }
}
