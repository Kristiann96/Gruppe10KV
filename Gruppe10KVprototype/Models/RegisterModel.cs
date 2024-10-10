using System.ComponentModel.DataAnnotations;

namespace Gruppe10KVprototype.Models
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "Brukernavn er påkrevd.")]
        [StringLength(100)]
        public string Username { get; set; }

        [Required(ErrorMessage = "E-post er påkrevd.")]
        [EmailAddress(ErrorMessage = "Ugyldig e-postadresse.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Passord er påkrevd.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passordene samsvarer ikke.")]
        public string ConfirmPassword { get; set; } // Added ConfirmPassword property for validation
    }
}

