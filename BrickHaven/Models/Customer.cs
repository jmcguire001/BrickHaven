using BrickHaven.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BrickHaven.Models
{
    public class Customer : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? Birthday { get; set; }
        public string? ResidenceCountry { get; set; }
        public string? Gender { get; set; }
        public float? Age { get; set; }
        public int? HighestProduct { get; set; }
        public bool? CookieEnabled { get; set; } = false;
    }
}
