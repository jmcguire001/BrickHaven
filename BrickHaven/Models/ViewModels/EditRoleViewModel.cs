using System.ComponentModel.DataAnnotations;

namespace BrickHaven.Models.ViewModels
{
    public class EditRoleViewModel
    {
        [Required]
        [Display(Name = "Role ID")]
        public string Id { get; set; }
        [Required(ErrorMessage = "Role Name is Required")]
        [Display(Name = "Role")]
        public string RoleName { get; set; }
    }
}