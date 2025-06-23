using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Alzheimer_Detection.Models;
using Alzheimer_Detection.Data;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Alzheimer_Detection.Controllers
{
    public class DoctorController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DoctorController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Doctor/ManagePatients
        public async Task<IActionResult> ManagePatients()
        {
            // Vérifier si l'utilisateur est un médecin
            var role = HttpContext.Session.GetString("UserRole");
            if (role != "Medecin")
            {
                return RedirectToAction("AccessDenied", "Home");
            }

            // Récupérer tous les patients
            var patients = await _context.Users
                .Where(u => u.Role == "Patient")
                .ToListAsync();

            // Calculer les statistiques
            int totalPatients = patients.Count;
            int alzheimerPatients = patients.Count(p => p.ResultatTest == "alzheimer-maladi");
            int healthyPatients = patients.Count(p => p.ResultatTest == "non-maladia-alzheimer");

            ViewBag.TotalPatients = totalPatients;
            ViewBag.AlzheimerPatients = alzheimerPatients;
            ViewBag.HealthyPatients = healthyPatients;

            return View(patients);
        }

        // POST: /Doctor/UpdatePatient
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePatient(int id, string resultatTest)
        {
            var patient = await _context.Users.FindAsync(id);
            if (patient == null)
            {
                return NotFound();
            }

            patient.ResultatTest = resultatTest;
            _context.Update(patient);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(ManagePatients));
        }

        // POST: /Doctor/UploadImage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadImage(int id, IFormFile file)
        {
            var patient = await _context.Users.FindAsync(id);
            if (patient == null)
            {
                return NotFound();
            }

            if (file != null && file.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);
                    patient.MRI_Image = memoryStream.ToArray();
                }
                _context.Update(patient);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(ManagePatients));
        }

        [HttpPost]
        public async Task<IActionResult> Analyze(IFormFile file)
        {

            Console.WriteLine("file : ", file);
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
            Console.WriteLine("class : " + classe + " confiance : " + confiance);

            return Json(new { success = true, predictClass = classe, predictConfiance = confiance });
        }

        // Exemple simple de fonction de prédiction
        private string RunPrediction(string modelPath, string imagePath)
        {
            string appPath = Path.Combine(Directory.GetCurrentDirectory(), "modele", "app.py");
            string classPath = Path.Combine(Directory.GetCurrentDirectory(), "modele", "classes.json");
            var psi = new ProcessStartInfo
            {
                FileName = @"C:\Users\hp\AppData\Local\Programs\Python\Python310\\python.exe",
                Arguments = $"\"{appPath}\" \"{modelPath}\" \"{classPath}\" \"{imagePath}\"",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            Console.WriteLine("psi argument:" + psi.Arguments);
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