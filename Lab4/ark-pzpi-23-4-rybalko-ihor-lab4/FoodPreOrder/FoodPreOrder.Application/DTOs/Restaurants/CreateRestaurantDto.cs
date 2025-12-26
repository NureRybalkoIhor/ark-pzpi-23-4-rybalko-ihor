п»їusing System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodPreOrder.Application.DTOs.Restaurants
{
    public class CreateRestaurantDto
    {
        [Required(ErrorMessage = "РќР°Р·РІР° Р·Р°РєР»Р°РґСѓ С” РѕР±РѕРІ'СЏР·РєРѕРІРѕСЋ")]
        [MaxLength(100, ErrorMessage = "РќР°Р·РІР° Р·Р°РЅР°РґС‚Рѕ РґРѕРІРіР°")]
        public string NameUA { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string NameEN { get; set; } = string.Empty;

        [Required(ErrorMessage = "РђРґСЂРµСЃР° С” РѕР±РѕРІ'СЏР·РєРѕРІРѕСЋ")]
        [MaxLength(200, ErrorMessage = "РђРґСЂРµСЃР° Р·Р°РЅР°РґС‚Рѕ РґРѕРІРіР°")]
        public string Address { get; set; } = string.Empty;

        public string? ImageUrl { get; set; }

        [Range(-90, 90, ErrorMessage = "РЁРёСЂРѕС‚Р° РјР°С” Р±СѓС‚Рё РІС–Рґ -90 РґРѕ 90")]
        public double Latitude { get; set; }

        [Range(-180, 180, ErrorMessage = "Р”РѕРІРіРѕС‚Р° РјР°С” Р±СѓС‚Рё РІС–Рґ -180 РґРѕ 180")]
        public double Longitude { get; set; }

        [Required]
        public int OwnerId { get; set; }
    }
}
