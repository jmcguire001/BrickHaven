using System.ComponentModel.DataAnnotations;

namespace BrickHaven.Models.ViewModels
{
    public class CreateRoleViewModel
    {
        [Required]
        public string RoleName { get; set; }
    }
}