using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace BrickHaven.Models
{
    public class Customer : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}
