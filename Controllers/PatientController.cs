using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Alzheimer_Detection.Models;
using Alzheimer_Detection.Services;
using Alzheimer_Detection.Data;
using System;

namespace Alzheimer_Detection.Controllers
{
	[Authorize(Roles = "Patient")]
	public class PatientController : Controller
	{
		private readonly IPdfReportService _pdfService;
		private readonly ApplicationDbContext _context;

		public PatientController(
			IPdfReportService pdfService,
			ApplicationDbContext context)
		{
			_pdfService = pdfService;
			_context = context;
		}

		public IActionResult Dashboard()
		{
			var userId = HttpContext.Session.GetInt32("UserId");
			if (userId == null)
			{
				return RedirectToAction("Login", "Account");
			}

			var user = _context.Users.Find(userId);
			if (user == null)
			{
				return NotFound("Utilisateur non trouvé");
			}

			return View(user);
		}

		public IActionResult DownloadReport()
		{
			var userId = HttpContext.Session.GetInt32("UserId");
			if (userId == null)
			{
				return Unauthorized("Utilisateur non connecté");
			}

			var user = _context.Users.Find(userId);
			if (user == null)
			{
				return NotFound("Utilisateur non trouvé");
			}

			try
			{
				var pdfBytes = _pdfService.GeneratePdfReport(user);
				return File(pdfBytes, "application/pdf", $"Rapport_Alzheimer_{user.LastName}.pdf");
			}
			catch (Exception ex)
			{
				return StatusCode(500, $"Erreur lors de la génération du rapport: {ex.Message}");
			}
		}
	}
}