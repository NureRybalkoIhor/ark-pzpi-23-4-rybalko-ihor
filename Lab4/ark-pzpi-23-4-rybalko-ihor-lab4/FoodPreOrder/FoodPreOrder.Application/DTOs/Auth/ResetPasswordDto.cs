п»їusing System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodPreOrder.Application.DTOs.Auth
{
    public class ResetPasswordDto
    {
        [Required]
        public string Token { get; set; } = string.Empty;

        [Required(ErrorMessage = "РќРѕРІРёР№ РїР°СЂРѕР»СЊ С” РѕР±РѕРІ'СЏР·РєРѕРІРёРј")]
        [MinLength(6, ErrorMessage = "РџР°СЂРѕР»СЊ РјР°С” Р±СѓС‚Рё РјС–РЅС–РјСѓРј 6 СЃРёРјРІРѕР»С–РІ")]
        [MaxLength(50, ErrorMessage = "РџР°СЂРѕР»СЊ Р·Р°РЅР°РґС‚Рѕ РґРѕРІРіРёР№")]
        public string NewPassword { get; set; } = string.Empty;
    }
}
