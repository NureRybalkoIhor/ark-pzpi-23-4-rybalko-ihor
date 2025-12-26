п»їusing FoodPreOrder.Application.DTOs.Admin;
using FoodPreOrder.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FoodPreOrder.Api.Controllers
{
    /// <summary>
    /// РљРѕРЅС‚СЂРѕР»РµСЂ РґР»СЏ РІРµСЂРёС„С–РєР°С†С–С— Р±С–Р·РЅРµСЃ-РєРѕСЂРёСЃС‚СѓРІР°С‡С–РІ.
    /// Р”РѕР·РІРѕР»СЏС” РєРѕСЂРёСЃС‚СѓРІР°С‡Р°Рј РїРѕРґР°РІР°С‚Рё РґРѕРєСѓРјРµРЅС‚Рё РґР»СЏ РѕС‚СЂРёРјР°РЅРЅСЏ СЃС‚Р°С‚СѓСЃСѓ "RestaurantOwner",
    /// Р° Р°РґРјС–РЅС–СЃС‚СЂР°С‚РѕСЂР°Рј вЂ” РїРµСЂРµРіР»СЏРґР°С‚Рё С‚Р° Р·Р°С‚РІРµСЂРґР¶СѓРІР°С‚Рё С†С– Р·Р°СЏРІРєРё.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class VerificationController : ControllerBase
    {
        private readonly IVerificationService _verificationService;

        public VerificationController(IVerificationService verificationService)
        {
            _verificationService = verificationService;
        }

        /// <summary>
        /// РџРѕРґР°С” Р·Р°СЏРІРєСѓ РЅР° РІРµСЂРёС„С–РєР°С†С–СЋ РѕР±Р»С–РєРѕРІРѕРіРѕ Р·Р°РїРёСЃСѓ.
        /// </summary>
        /// <remarks>
        /// РџСЂРёР№РјР°С” С„Р°Р№Р» (СЃРєР°РЅ РґРѕРєСѓРјРµРЅС‚С–РІ) Сѓ С„РѕСЂРјР°С‚С– `multipart/form-data`.
        /// РЇРєС‰Рѕ Р·Р°СЏРІРєР° СЃС…РІР°Р»РµРЅР°, РєРѕСЂРёСЃС‚СѓРІР°С‡ РѕС‚СЂРёРјР°С” СЂРѕР»СЊ `RestaurantOwner`.
        /// </remarks>
        /// <param name="dto">Р”Р°РЅС– Р·Р°СЏРІРєРё (Р¤Р°Р№Р» РґРѕРєСѓРјРµРЅС‚Сѓ).</param>
        /// <returns>Р РµР·СѓР»СЊС‚Р°С‚ РїРѕРґР°С‡С– Р·Р°СЏРІРєРё.</returns>
        /// <response code="200">Р”РѕРєСѓРјРµРЅС‚Рё СѓСЃРїС–С€РЅРѕ Р·Р°РІР°РЅС‚Р°Р¶РµРЅРѕ.</response>
        /// <response code="400">Р¤Р°Р№Р» РЅРµ РѕР±СЂР°РЅРѕ Р°Р±Рѕ РІС–РЅ РїРѕСЂРѕР¶РЅС–Р№.</response>
        /// <response code="401">РљРѕСЂРёСЃС‚СѓРІР°С‡ РЅРµ Р°РІС‚РѕСЂРёР·РѕРІР°РЅРёР№.</response>
        /// <response code="500">Р’РЅСѓС‚СЂС–С€РЅСЏ РїРѕРјРёР»РєР° СЃРµСЂРІРµСЂР° РїСЂРё Р·Р±РµСЂРµР¶РµРЅРЅС– С„Р°Р№Р»Сѓ.</response>
        [HttpPost("submit")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Submit([FromForm] SubmitVerificationDto dto)
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

                if (dto.Document == null || dto.Document.Length == 0)
                {
                    return BadRequest("Р‘СѓРґСЊ Р»Р°СЃРєР°, Р·Р°РІР°РЅС‚Р°Р¶С‚Рµ С„Р°Р№Р».");
                }

                await _verificationService.SubmitRequestAsync(userId, dto.Document);

                return Ok(new { message = "Р”РѕРєСѓРјРµРЅС‚Рё СѓСЃРїС–С€РЅРѕ РІС–РґРїСЂР°РІР»РµРЅРѕ РЅР° РїРµСЂРµРІС–СЂРєСѓ." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "РџРѕРјРёР»РєР° СЃРµСЂРІРµСЂР°: " + ex.Message });
            }
        }

        /// <summary>
        /// РћС‚СЂРёРјСѓС” СЃРїРёСЃРѕРє Р·Р°СЏРІРѕРє, С‰Рѕ РѕС‡С–РєСѓСЋС‚СЊ РЅР° СЂРѕР·РіР»СЏРґ.
        /// </summary>
        /// <remarks>
        /// Р”РѕСЃС‚СѓРїРЅРѕ С‚С–Р»СЊРєРё РґР»СЏ РђРґРјС–РЅС–СЃС‚СЂР°С‚РѕСЂС–РІ.
        /// </remarks>
        /// <returns>РЎРїРёСЃРѕРє Р·Р°СЏРІРѕРє Р·С– СЃС‚Р°С‚СѓСЃРѕРј Pending.</returns>
        /// <response code="200">РЈСЃРїС–С€РЅРµ РѕС‚СЂРёРјР°РЅРЅСЏ СЃРїРёСЃРєСѓ.</response>
        /// <response code="403">Р”РѕСЃС‚СѓРї Р·Р°Р±РѕСЂРѕРЅРµРЅРѕ (С‚С–Р»СЊРєРё Admin).</response>
        [HttpGet("pending")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetPending()
        {
            var requests = await _verificationService.GetPendingRequestsAsync();
            return Ok(requests);
        }

        /// <summary>
        /// РћР±СЂРѕР±Р»СЏС” Р·Р°СЏРІРєСѓ РЅР° РІРµСЂРёС„С–РєР°С†С–СЋ (РЎС…РІР°Р»РµРЅРЅСЏ Р°Р±Рѕ РІС–РґС…РёР»РµРЅРЅСЏ).
        /// </summary>
        /// <remarks>
        /// РђРґРјС–РЅС–СЃС‚СЂР°С‚РѕСЂ РІРёСЂС–С€СѓС” РґРѕР»СЋ Р·Р°СЏРІРєРё.
        /// - РЈ СЂР°Р·С– СЃС…РІР°Р»РµРЅРЅСЏ (`IsApproved = true`), РєРѕСЂРёСЃС‚СѓРІР°С‡ РѕС‚СЂРёРјСѓС” СЂРѕР»СЊ RestaurantOwner.
        /// - РЈ СЂР°Р·С– РІС–РґС…РёР»РµРЅРЅСЏ, Р·Р°СЏРІРєР° РїРѕР·РЅР°С‡Р°С”С‚СЊСЃСЏ СЏРє Rejected, РјРѕР¶РЅР° РІРєР°Р·Р°С‚Рё РїСЂРёС‡РёРЅСѓ.
        /// </remarks>
        /// <param name="dto">Р С–С€РµРЅРЅСЏ Р°РґРјС–РЅС–СЃС‚СЂР°С‚РѕСЂР° (ID Р·Р°СЏРІРєРё, СЃС‚Р°С‚СѓСЃ, РїСЂРёС‡РёРЅР° РІС–РґРјРѕРІРё).</param>
        /// <returns>Р РµР·СѓР»СЊС‚Р°С‚ РѕР±СЂРѕР±РєРё.</returns>
        /// <response code="200">Р—Р°СЏРІРєСѓ СѓСЃРїС–С€РЅРѕ РѕР±СЂРѕР±Р»РµРЅРѕ.</response>
        /// <response code="400">Р—Р°СЏРІРєСѓ РЅРµ Р·РЅР°Р№РґРµРЅРѕ Р°Р±Рѕ РІРѕРЅР° РІР¶Рµ РѕР±СЂРѕР±Р»РµРЅР°.</response>
        /// <response code="403">Р”РѕСЃС‚СѓРї Р·Р°Р±РѕСЂРѕРЅРµРЅРѕ.</response>
        [HttpPost("process")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Process([FromBody] ProcessVerificationDto dto)
        {
            var adminId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var result = await _verificationService.ProcessRequestAsync(adminId, dto);

            if (!result)
                return BadRequest("Р—Р°СЏРІРєР° РЅРµ Р·РЅР°Р№РґРµРЅР° Р°Р±Рѕ РІР¶Рµ Р±СѓР»Р° РѕР±СЂРѕР±Р»РµРЅР°.");

            return Ok(new { message = dto.IsApproved ? "Р’Р»Р°СЃРЅРёРєР° Р·Р°С‚РІРµСЂРґР¶РµРЅРѕ СѓСЃРїС–С€РЅРѕ!" : "Р—Р°СЏРІРєСѓ РІС–РґС…РёР»РµРЅРѕ." });
        }
    }
}
