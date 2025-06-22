using System.ComponentModel.DataAnnotations;

namespace Alzheimer_Detection.Models
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int? Age{ get; set; }
        public string Role { get; set; }
        public string? RésultatTest { get; set; }

        // Propriété pour stocker l'image IRM
        public byte[]? MRI_Image { get; set; }
    }
}
