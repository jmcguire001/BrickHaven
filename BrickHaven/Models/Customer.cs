using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace BrickHaven.Models
{
    public class Customer : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? CountryOfResidence { get; set; }
        public string? Gender { get; set;}
        public int? Age { get; set; }
    }
}
