п»їusing System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodPreOrder.Application.DTOs.Auth
{
    public class RegisterUserDto
    {
        [Required(ErrorMessage = "РџР†Р‘ С” РѕР±РѕРІ'СЏР·РєРѕРІРёРј")]
        [StringLength(100, ErrorMessage = "РџР†Р‘ РЅРµ РјРѕР¶Рµ РїРµСЂРµРІРёС‰СѓРІР°С‚Рё 100 СЃРёРјРІРѕР»С–РІ")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email С” РѕР±РѕРІ'СЏР·РєРѕРІРёРј")]
        [EmailAddress(ErrorMessage = "РќРµРєРѕСЂРµРєС‚РЅРёР№ С„РѕСЂРјР°С‚ Email")]
        [StringLength(100, ErrorMessage = "Email Р·Р°РЅР°РґС‚Рѕ РґРѕРІРіРёР№")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "РџР°СЂРѕР»СЊ С” РѕР±РѕРІ'СЏР·РєРѕРІРёРј")]
        [MinLength(6, ErrorMessage = "РџР°СЂРѕР»СЊ РјР°С” Р±СѓС‚Рё РјС–РЅС–РјСѓРј 6 СЃРёРјРІРѕР»С–РІ")]
        [MaxLength(50, ErrorMessage = "РџР°СЂРѕР»СЊ Р·Р°РЅР°РґС‚Рѕ РґРѕРІРіРёР№")]
        public string Password { get; set; } = string.Empty;

        [Phone(ErrorMessage = "РќРµРєРѕСЂРµРєС‚РЅРёР№ С„РѕСЂРјР°С‚ С‚РµР»РµС„РѕРЅСѓ")]
        [StringLength(20, ErrorMessage = "РўРµР»РµС„РѕРЅ Р·Р°РЅР°РґС‚Рѕ РґРѕРІРіРёР№")]
        public string Phone { get; set; } = string.Empty;
    }
}
