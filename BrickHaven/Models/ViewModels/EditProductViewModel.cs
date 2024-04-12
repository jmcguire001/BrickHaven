using System.ComponentModel.DataAnnotations;

namespace BrickHaven.Models.ViewModels
{
    public class EditProductViewModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Product Name")]
        public string Name { get; set; }

        [Display(Name = "Year")]
        public int? Year { get; set; }

        [Display(Name = "Number of Parts")]
        public int? NumParts { get; set; }

        [Required]
        [Display(Name = "Price")]
        public float? Price { get; set; }

        [Display(Name = "URL to Image")]
        public string? ImgLink { get; set; }

        [Display(Name = "Primary Color")]
        public string? PrimaryColor { get; set; }

        [Display(Name = "Secondary Color")]
        public string? SecondaryColor { get; set; }

        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Required]
        [Display(Name = "Category")]
        public string Category { get; set; }
    }
}