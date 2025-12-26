using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodPreOrder.Application.DTOs.Orders
{
    public class CreateOrderDto
    {
        [Required(ErrorMessage = "Restaurant ID is required")]
        public int RestaurantId { get; set; }

        [Required(ErrorMessage = "Visit time is required")]
        public DateTime VisitTime { get; set; }

        [MaxLength(500, ErrorMessage = "Comment cannot exceed 500 characters")]
        public string? Comment { get; set; }

        [Required(ErrorMessage = "Order items list cannot be empty")]
        public List<CreateOrderItemDto> Items { get; set; } = new List<CreateOrderItemDto>();
    }
}
