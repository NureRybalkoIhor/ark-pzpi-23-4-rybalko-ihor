п»їusing System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodPreOrder.Application.DTOs.Reviews
{
    public class CreateReviewDto
    {
        [Required(ErrorMessage = "РќРµРѕР±С…С–РґРЅРѕ РІРєР°Р·Р°С‚Рё СЃС‚СЂР°РІСѓ (ID)")]
        public int DishId { get; set; }

        [Required(ErrorMessage = "РћС†С–РЅРєР° С” РѕР±РѕРІ'СЏР·РєРѕРІРѕСЋ")]
        [Range(1, 5, ErrorMessage = "РћС†С–РЅРєР° РјР°С” Р±СѓС‚Рё РІС–Рґ 1 РґРѕ 5 Р·С–СЂРѕРє")]
        public int Rating { get; set; }

        [MaxLength(500, ErrorMessage = "РљРѕРјРµРЅС‚Р°СЂ Р·Р°РЅР°РґС‚Рѕ РґРѕРІРіРёР№ (РјР°РєСЃ 500 СЃРёРјРІРѕР»С–РІ)")]
        public string Comment { get; set; } = string.Empty;
    }
}
