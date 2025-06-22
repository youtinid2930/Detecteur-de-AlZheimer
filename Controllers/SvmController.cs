using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Alzheimer_Detection.Models;

namespace Alzheimer_Detection.Controllers
{
    public class SvmController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public JsonResult AnalyzeSymptoms([FromBody] Patient input)
        {
            string result = "";
            try
            {
                Console.WriteLine("Received input: " + input.symptoms );
                string appPath = Path.Combine(Directory.GetCurrentDirectory(), "modele", "mod2.py");
                string currentPath = Directory.GetCurrentDirectory();
                var psi = new ProcessStartInfo
                {
                    FileName = @"C:\\Users\\hp\\AppData\\Local\\Programs\\Python\\Python310\\python.exe",
                    Arguments = $"\"{appPath}\" \"{input.symptoms}\" \"{currentPath}\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                Console.WriteLine("Executing Python script with arguments: " + psi.Arguments);


                using (var process = Process.Start(psi))
                {
                    result = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();
                }

                var parts = result.Trim().Split('|');
                Console.WriteLine("Python script output: " + result);
                string prediction = parts[0];
                string confidence = parts.Length > 1 ? parts[1] : "0.00";

                Console.WriteLine($"Prediction: {prediction}, Confidence: {confidence}");

                var details = prediction == "Alzheimer"
                    ? "Les symptômes indiquent un risque élevé."
                    : "Les symptômes ne semblent pas indiquer Alzheimer.";

                var recommandations = prediction == "Alzheimer"
                    ? new[] { "Consulter un spécialiste", "Faire un IRM", "Suivi cognitif recommandé" }
                    : new[] { "Surveillance régulière", "Hygiène de vie", "Consultez en cas de doute" };

                return Json(new
                {
                    prediction,
                    probability = confidence,
                    details,
                    recommendations = recommandations
                });
            }
            catch
            {
                return Json(new
                {
                    prediction = "Erreur",
                    probability = "0",
                    details = "Une erreur s'est produite.",
                    recommendations = new[] { "Réessayez plus tard." }
                });
            }
        }

    }
}
