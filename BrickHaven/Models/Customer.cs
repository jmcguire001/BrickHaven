using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.ComponentModel.DataAnnotations;

namespace BrickHaven.Models
{
    public class Customer : IdentityUser
    {
        public int? CustomerId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? Birthday { get; set; }
        public string? ResidenceCountry { get; set; }
        public string? Gender { get; set; }
        public float Age { get; set; }
    }
}
