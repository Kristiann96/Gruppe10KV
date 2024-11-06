using System.ComponentModel.DataAnnotations;

namespace ViewModels;
public class BrukerregistreringViewModel // Fjernet internal
{
    [Required(ErrorMessage = "E-post er påkrevd")]
    [EmailAddress(ErrorMessage = "Ugyldig e-postformat")]
    [Display(Name = "E-post")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Passord er påkrevd")]
    [StringLength(100, ErrorMessage = "Passordet må være minst {2} tegn langt.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "Passord")]
    public string Password { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "Bekreft passord")]
    [Compare("Password", ErrorMessage = "Passordene matcher ikke.")]
    public string BekreftPassord { get; set; }
}