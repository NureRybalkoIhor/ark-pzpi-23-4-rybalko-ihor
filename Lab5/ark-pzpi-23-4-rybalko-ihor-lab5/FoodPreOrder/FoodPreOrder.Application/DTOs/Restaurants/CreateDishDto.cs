using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace FoodPreOrder.Application.DTOs.Restaurants
{
    public class CreateDishDto
    {
        [Required(ErrorMessage = "Dish name in Ukrainian is required")]
        [MaxLength(100, ErrorMessage = "Dish name in Ukrainian cannot exceed 100 characters")]
        public string NameUA { get; set; } = string.Empty;

        [MaxLength(100, ErrorMessage = "Dish name in English cannot exceed 100 characters")]
        public string NameEN { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "Description in Ukrainian cannot exceed 500 characters")]
        public string DescriptionUA { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "Description in English cannot exceed 500 characters")]
        public string DescriptionEN { get; set; } = string.Empty;

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, 100000, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        public IFormFile? Image { get; set; }

        [Range(1, 480, ErrorMessage = "Preparation time must be between 1 and 480 minutes")]
        public int PreparationTimeMinutes { get; set; }

        [Required(ErrorMessage = "Category ID is required")]
        public int CategoryId { get; set; }
    }
}
