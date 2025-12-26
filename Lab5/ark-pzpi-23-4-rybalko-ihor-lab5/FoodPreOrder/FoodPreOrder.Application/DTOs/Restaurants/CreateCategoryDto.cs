using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodPreOrder.Application.DTOs.Restaurants
{
    public class CreateCategoryDto
    {
        [Required(ErrorMessage = "Category name in Ukrainian is required")]
        [MaxLength(100, ErrorMessage = "Category name in Ukrainian cannot exceed 100 characters")]
        public string NameUA { get; set; } = string.Empty;

        [Required(ErrorMessage = "Category name in English is required")]
        [MaxLength(100, ErrorMessage = "Category name in English cannot exceed 100 characters")]
        public string NameEN { get; set; } = string.Empty;

        [Required(ErrorMessage = "Restaurant ID is required")]
        public int RestaurantId { get; set; }
    }
}