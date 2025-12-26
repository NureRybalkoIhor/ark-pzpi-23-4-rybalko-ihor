п»їusing System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodPreOrder.Application.DTOs.Orders
{
    public class CreateOrderDto
    {
        [Required(ErrorMessage = "РќРµРѕР±С…С–РґРЅРѕ РІРєР°Р·Р°С‚Рё СЂРµСЃС‚РѕСЂР°РЅ")]
        public int RestaurantId { get; set; }

        [Required(ErrorMessage = "РќРµРѕР±С…С–РґРЅРѕ РІРєР°Р·Р°С‚Рё С‡Р°СЃ РІС–Р·РёС‚Сѓ")]
        public DateTime VisitTime { get; set; }

        [MaxLength(500, ErrorMessage = "РљРѕРјРµРЅС‚Р°СЂ РЅРµ РјРѕР¶Рµ РїРµСЂРµРІРёС‰СѓРІР°С‚Рё 500 СЃРёРјРІРѕР»С–РІ")]
        public string? Comment { get; set; }

        [Required(ErrorMessage = "РЎРїРёСЃРѕРє СЃС‚СЂР°РІ РЅРµ РјРѕР¶Рµ Р±СѓС‚Рё РїРѕСЂРѕР¶РЅС–Рј")]
        public List<CreateOrderItemDto> Items { get; set; } = new List<CreateOrderItemDto>();
    }
}
