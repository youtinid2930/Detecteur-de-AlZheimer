using Microsoft.AspNetCore.Mvc;
using Alzheimer_Detection.Models;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Alzheimer_Detection.Controllers
{
    public class DetectionController : Controller
    {
        private readonly ILogger<DetectionController> _logger;

        public DetectionController(ILogger<DetectionController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Analyze(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                TempData["Error"] = "Aucun fichier fourni.";
                return RedirectToAction("Index", "Home");
            }

            var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
            Directory.CreateDirectory(uploads);
            var filePath = Path.Combine(uploads, file.FileName);
            Console.WriteLine(filePath);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            Console.WriteLine(filePath);
            string modelPath = Path.Combine(Directory.GetCurrentDirectory(), "modele", "mymodel.keras");
            string predictionString = RunPrediction(modelPath, filePath);



            var match = Regex.Match(predictionString, @"Classe:\s*(.+?)\s+Confiance:\s*([\d.]+%)");


            var classe = match.Groups[1].Value;
            var confiance = match.Groups[2].Value;

            System.IO.File.Delete(filePath); // facultatif

            // 🔁 envoyer vers Home/Index avec le résultat
            
            Console.WriteLine("after the result: ");
            Console.WriteLine("class : "+classe+" confiance : "+ confiance);
            
            return Json(new { success = true, predictClass = classe, predictConfiance = confiance });
        }

        // Exemple simple de fonction de prédiction
        private string RunPrediction(string modelPath, string imagePath)
        {
            string appPath = Path.Combine(Directory.GetCurrentDirectory(), "modele", "app.py");
            string classPath = Path.Combine(Directory.GetCurrentDirectory(), "modele", "classes.json");
            var psi = new ProcessStartInfo
            {
                FileName = @"C:\Users\yasst\AppData\Local\Programs\Python\Python310\\python.exe",
                Arguments = $"\"{appPath}\" \"{modelPath}\" \"{classPath}\" \"{imagePath}\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            Console.WriteLine("psi argument:"+psi.Arguments);
            using (var process = Process.Start(psi))
            {
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                Console.WriteLine("output: " + output);
                return output;
            }
        }

    }
}
