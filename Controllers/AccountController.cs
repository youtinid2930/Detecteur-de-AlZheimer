using Microsoft.AspNetCore.Mvc;
using Alzheimer_Detection.Models;
using Alzheimer_Detection.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace Alzheimer_Detection.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Account/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(User user)
        {
            Console.WriteLine($"user Role 1: {user?.Role}"); // par exemple
            Console.WriteLine($"ModelState valid: {ModelState.IsValid}");

            if (!ModelState.IsValid)
            {
                foreach (var state in ModelState)
                {
                    var key = state.Key;
                    var errors = state.Value.Errors;
                    foreach (var error in errors)
                    {
                        Console.WriteLine($"Erreur sur {key} : {error.ErrorMessage}");
                    }
                }
                return View(user);
            }


            if (ModelState.IsValid)
            {
                Console.WriteLine("I am here in the beginig");
                // Vérifier si l'email existe déjà
                if (await _context.Users.AnyAsync(u => u.Email == user.Email))
                {
                    ModelState.AddModelError("Email", "Cet email est déjà utilisé");
                    return View(user);
                }
                Console.WriteLine($"user Role 2: {user?.Role}"); // par exemple
                // Hasher le mot de passe
                user.Password = HashPassword(user.Password);

                // Valeurs par défaut pour les nouveaux utilisateurs
                user.ResultatTest = "non-defini";
                

                _context.Add(user);
                await _context.SaveChangesAsync();
                Console.WriteLine($"user Role 3: {user?.Role}"); // par exemple
                // Créer l'identité de l'utilisateur
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.GivenName, user.FirstName),
                    new Claim(ClaimTypes.Surname, user.LastName),
                    new Claim(ClaimTypes.Role, user.Role)
                };
                Console.WriteLine($"user Role 4: {user?.Role}"); // par exemple
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                Console.WriteLine("User Role 5: ", user.Role);

                // Stocker les informations dans la session
                HttpContext.Session.SetInt32("UserId", user.Id);
                HttpContext.Session.SetString("UserRole", user.Role);
                HttpContext.Session.SetString("UserFirstName", user.FirstName);

                // Redirection basée sur le rôle
                if (user.Role == "Medecin")
                {
                    return RedirectToAction("ManagePatients", "Doctor");
                }
                else
                {
                    return RedirectToAction("Dashboard", "Patient");
                }
            }
            return View(user);
        }

        // GET: /Account/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user != null && VerifyPassword(password, user.Password))
            {
                // Créer l'identité de l'utilisateur
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.GivenName, user.FirstName),
                    new Claim(ClaimTypes.Surname, user.LastName),
                    new Claim(ClaimTypes.Role, user.Role)
                };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                // Stocker les informations dans la session
                HttpContext.Session.SetInt32("UserId", user.Id);
                HttpContext.Session.SetString("UserRole", user.Role);
                HttpContext.Session.SetString("UserFirstName", user.FirstName);

                // Redirection basée sur le rôle
                if (user.Role == "Medecin")
                {
                    return RedirectToAction("ManagePatients", "Doctor");
                }
                else
                {
                    return RedirectToAction("Dashboard", "Patient");
                }
            }

            ModelState.AddModelError(string.Empty, "Identifiants invalides");
            return View();
        }

        // GET: /Account/Logout
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }

        private bool VerifyPassword(string password, string storedHash)
        {
            return HashPassword(password) == storedHash;
        }
    }
}