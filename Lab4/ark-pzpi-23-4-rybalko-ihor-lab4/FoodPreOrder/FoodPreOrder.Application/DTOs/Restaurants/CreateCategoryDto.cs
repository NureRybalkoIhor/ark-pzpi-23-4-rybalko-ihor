РїВ»С—using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodPreOrder.Application.DTOs.Restaurants
{
    public class CreateCategoryDto
    {
        [Required(ErrorMessage = "Р СњР В°Р В·Р Р†Р В° РЎС“Р С”РЎР‚Р В°РЎвЂ”Р Р…РЎРѓРЎРЉР С”Р С•РЎР‹ РЎвЂќ Р С•Р В±Р С•Р Р†'РЎРЏР В·Р С”Р С•Р Р†Р С•РЎР‹")]
        [MaxLength(100, ErrorMessage = "Р СњР В°Р В·Р Р†Р В° Р В·Р В°Р Р…Р В°Р Т‘РЎвЂљР С• Р Т‘Р С•Р Р†Р С–Р В°")]
        public string NameUA { get; set; } = string.Empty;

        [Required(ErrorMessage = "Р СњР В°Р В·Р Р†Р В° Р В°Р Р…Р С–Р В»РЎвЂ“Р в„–РЎРѓРЎРЉР С”Р С•РЎР‹ РЎвЂќ Р С•Р В±Р С•Р Р†'РЎРЏР В·Р С”Р С•Р Р†Р С•РЎР‹")]
        [MaxLength(100, ErrorMessage = "Р СњР В°Р В·Р Р†Р В° Р В·Р В°Р Р…Р В°Р Т‘РЎвЂљР С• Р Т‘Р С•Р Р†Р С–Р В°")]
        public string NameEN { get; set; } = string.Empty;

        [Required]
        public int RestaurantId { get; set; }
    }
}
