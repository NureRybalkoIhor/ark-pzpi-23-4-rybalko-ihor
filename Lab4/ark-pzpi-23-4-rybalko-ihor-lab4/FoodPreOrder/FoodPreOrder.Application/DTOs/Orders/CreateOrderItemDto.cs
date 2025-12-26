п»їusing System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodPreOrder.Application.DTOs.Orders
{
    public class CreateOrderItemDto
    {
        [Required(ErrorMessage = "РќРµРѕР±С…С–РґРЅРѕ РІРєР°Р·Р°С‚Рё СЃС‚СЂР°РІСѓ (ID)")]
        public int DishId { get; set; }

        [Required]
        [Range(1, 100, ErrorMessage = "РљС–Р»СЊРєС–СЃС‚СЊ РјР°С” Р±СѓС‚Рё РІС–Рґ 1 РґРѕ 100")]
        public int Quantity { get; set; }
    }
}
