using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ViewModels.Registrering
{
    public class KomplettRegistreringViewModel
    {
        // Person/Innmelder data
        [Required(ErrorMessage = "Fornavn er påkrevd")]
        [Display(Name = "Fornavn")]
        [StringLength(50, ErrorMessage = "Fornavn kan ikke være lengre enn 50 tegn")]
        public string Fornavn { get; set; } = null!;

        [Required(ErrorMessage = "Etternavn er påkrevd")]
        [Display(Name = "Etternavn")]
        [StringLength(50, ErrorMessage = "Etternavn kan ikke være lengre enn 50 tegn")]
        public string Etternavn { get; set; } = null!;

        [Required(ErrorMessage = "Telefonnummer er påkrevd")]
        [Display(Name = "Telefonnummer")]
        [StringLength(15, ErrorMessage = "Telefonnummer kan ikke være lengre enn 15 tegn")]
        [RegularExpression(@"^[0-9]+$", ErrorMessage = "Telefonnummer kan kun inneholde tall")]
        public string Telefonnummer { get; set; } = null!;

        [Required(ErrorMessage = "E-post er påkrevd")]
        [EmailAddress(ErrorMessage = "Ugyldig e-postformat")]
        [Display(Name = "E-post")]
        [StringLength(100, ErrorMessage = "E-post kan ikke være lengre enn 100 tegn")]
        public string Email { get; set; } = null!;

        // Identity data
        [Required(ErrorMessage = "Passord er påkrevd")]
        [StringLength(100, ErrorMessage = "Passordet må være minst {2} tegn langt.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Passord")]
        public string Password { get; set; } = null!;

        [DataType(DataType.Password)]
        [Display(Name = "Bekreft passord")]
        [Compare("Password", ErrorMessage = "Passordene matcher ikke.")]
        public string BekreftPassord { get; set; } = null!;

        // Hjelpemetoder for mapping
        public PersonInnmelderViewModel TilPersonInnmelderViewModel()
        {
            return new PersonInnmelderViewModel
            {
                Fornavn = this.Fornavn,
                Etternavn = this.Etternavn,
                Telefonnummer = this.Telefonnummer,
                Epost = this.Email
            };
        }

        public IdentityRegistreringViewModel TilIdentityViewModel()
        {
            return new IdentityRegistreringViewModel
            {
                Email = this.Email,
                Password = this.Password,
                BekreftPassord = this.BekreftPassord
            };
        }
    }
}
