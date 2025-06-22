using System.ComponentModel.DataAnnotations;

namespace Alzheimer_Detection.Models
{
    public class User
    {

        public User()
        {
            MRI_Image = Array.Empty<byte>();
            ResultatTest = "";
        }
        public int Id { get; set; }

        [Required(ErrorMessage = "L'email est requis.")]
        [EmailAddress(ErrorMessage = "Format d'email invalide.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Le mot de passe est requis.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Le prénom est requis.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Le nom est requis.")]
        public string LastName { get; set; }

        public int? Age { get; set; }
        public byte[] MRI_Image { get; set; }
        public string ResultatTest { get; set; }
        public string Role { get; set; } = "Patient"; // Valeur par défaut
    }
}