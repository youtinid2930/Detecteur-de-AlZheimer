using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Alzheimer_Detection.Models;
using Alzheimer_Detection.Data;

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
    }
}