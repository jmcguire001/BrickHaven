﻿using System.ComponentModel.DataAnnotations;

namespace BrickHaven.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }
        public string? Name { get; set; }
        public int? Year { get; set; }
        public int? NumParts { get; set; }
        public float? Price { get; set; }
        public string? ImgLink { get; set; }
        public string? PrimaryColor { get; set; }
        public string? SecondaryColor { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
        public int? Recommendation1 { get; set; }
        public int? Recommendation2 { get; set; }
        public int? Recommendation3 { get; set; }
        public int? Recommendation4 { get; set; }
        public int? Recommendation5 { get; set; }
    }
}
