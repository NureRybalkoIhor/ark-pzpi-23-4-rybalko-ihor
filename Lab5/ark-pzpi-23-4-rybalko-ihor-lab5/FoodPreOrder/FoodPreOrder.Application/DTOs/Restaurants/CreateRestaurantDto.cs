using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodPreOrder.Application.DTOs.Restaurants
{
    public class CreateRestaurantDto
    {
        [Required(ErrorMessage = "Restaurant name in Ukrainian is required")]
        [MaxLength(100, ErrorMessage = "Restaurant name in Ukrainian cannot exceed 100 characters")]
        public string NameUA { get; set; } = string.Empty;

        [Required(ErrorMessage = "Restaurant name in English is required")]
        [MaxLength(100, ErrorMessage = "Restaurant name in English cannot exceed 100 characters")]
        public string NameEN { get; set; } = string.Empty;

        [Required(ErrorMessage = "Address is required")]
        [MaxLength(200, ErrorMessage = "Address cannot exceed 200 characters")]
        public string Address { get; set; } = string.Empty;

        public string? ImageUrl { get; set; }

        [Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90")]
        public double Latitude { get; set; }

        [Range(-180, 180, ErrorMessage = "Longitude must be between -180 and 180")]
        public double Longitude { get; set; }

        [Required(ErrorMessage = "Owner ID is required")]
        public int OwnerId { get; set; }
    }
}