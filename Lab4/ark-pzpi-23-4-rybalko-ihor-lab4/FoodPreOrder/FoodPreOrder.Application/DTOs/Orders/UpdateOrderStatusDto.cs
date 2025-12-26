п»їusing FoodPreOrder.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodPreOrder.Application.DTOs.Orders
{
    public class UpdateOrderStatusDto
    {
        [Required(ErrorMessage = "РќРѕРІРёР№ СЃС‚Р°С‚СѓСЃ С” РѕР±РѕРІ'СЏР·РєРѕРІРёРј")]
        public OrderStatus Status { get; set; }
    }
}
