п»їusing System;
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
        [Required(ErrorMessage = "РќР°Р·РІР° СЃС‚СЂР°РІРё С” РѕР±РѕРІ'СЏР·РєРѕРІРѕСЋ")]
        [MaxLength(100, ErrorMessage = "РќР°Р·РІР° РЅРµ РјРѕР¶Рµ РїРµСЂРµРІРёС‰СѓРІР°С‚Рё 100 СЃРёРјРІРѕР»С–РІ")]
        public string NameUA { get; set; } = string.Empty;

        [MaxLength(100)]
        public string NameEN { get; set; } = string.Empty;

        [MaxLength(500, ErrorMessage = "РћРїРёСЃ Р·Р°РЅР°РґС‚Рѕ РґРѕРІРіРёР№ (РјР°РєСЃ 500 СЃРёРјРІРѕР»С–РІ)")]
        public string DescriptionUA { get; set; } = string.Empty;

        [MaxLength(500)]
        public string DescriptionEN { get; set; } = string.Empty;

        [Required]
        [Range(0.01, 100000, ErrorMessage = "Р¦С–РЅР° РјР°С” Р±СѓС‚Рё Р±С–Р»СЊС€Рµ 0")]
        public decimal Price { get; set; }

        public IFormFile? Image { get; set; }

        [Range(1, 480, ErrorMessage = "Р§Р°СЃ РїСЂРёРіРѕС‚СѓРІР°РЅРЅСЏ РјР°С” Р±СѓС‚Рё РІС–Рґ 1 С…РІ РґРѕ 8 РіРѕРґРёРЅ")]
        public int PreparationTimeMinutes { get; set; }

        [Required]
        public int CategoryId { get; set; }
    }
}
