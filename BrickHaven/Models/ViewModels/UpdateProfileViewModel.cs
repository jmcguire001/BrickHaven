using System.ComponentModel.DataAnnotations;

namespace BrickHaven.Models.ViewModels
{
    public class UpdateProfileViewModel
    {
        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Display(Name = "First Name")]
        public string? FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string? LastName { get; set; }

        [Display(Name = "Date of Birth")]
        public DateTime? Birthday { get; set; }

        [Display(Name = "Country of Residence")]
        public string? ResidenceCountry { get; set; }

        [Display(Name = "Gender")]
        public string? Gender { get; set; }
    }
}