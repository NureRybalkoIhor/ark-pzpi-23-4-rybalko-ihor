п»їusing System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodPreOrder.Application.DTOs.Auth
{
    public class ForgotPasswordDto
    {
        [Required(ErrorMessage = "Р’РІРµРґС–С‚СЊ Email")]
        [EmailAddress(ErrorMessage = "РќРµРєРѕСЂРµРєС‚РЅРёР№ С„РѕСЂРјР°С‚ Email")]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;
    }
}
