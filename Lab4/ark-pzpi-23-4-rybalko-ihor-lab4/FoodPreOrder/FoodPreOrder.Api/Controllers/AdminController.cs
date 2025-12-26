п»їusing FoodPreOrder.Application.DTOs.Admin;
using FoodPreOrder.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FoodPreOrder.Api.Controllers
{
    /// <summary>
    /// РљРѕРЅС‚СЂРѕР»РµСЂ РґР»СЏ РІРёРєРѕРЅР°РЅРЅСЏ Р°РґРјС–РЅС–СЃС‚СЂР°С‚РёРІРЅРёС… С„СѓРЅРєС†С–Р№.
    /// Р”РѕСЃС‚СѓРї РґРѕР·РІРѕР»РµРЅРѕ Р»РёС€Рµ РєРѕСЂРёСЃС‚СѓРІР°С‡Р°Рј Р· СЂРѕР»Р»СЋ "Admin".
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        /// <summary>
        /// РћС‚СЂРёРјСѓС” СЃРїРёСЃРѕРє СѓСЃС–С… Р·Р°СЂРµС”СЃС‚СЂРѕРІР°РЅРёС… РєРѕСЂРёСЃС‚СѓРІР°С‡С–РІ Сѓ СЃРёСЃС‚РµРјС–.
        /// </summary>
        /// <returns>РЎРїРёСЃРѕРє DTO РєРѕСЂРёСЃС‚СѓРІР°С‡С–РІ.</returns>
        /// <response code="200">РЈСЃРїС–С€РЅРµ РѕС‚СЂРёРјР°РЅРЅСЏ СЃРїРёСЃРєСѓ.</response>
        /// <response code="401">РљРѕСЂРёСЃС‚СѓРІР°С‡ РЅРµ Р°РІС‚РѕСЂРёР·РѕРІР°РЅРёР№.</response>
        /// <response code="403">РќРµРґРѕСЃС‚Р°С‚РЅСЊРѕ РїСЂР°РІ (РїРѕС‚СЂС–Р±РЅР° СЂРѕР»СЊ Admin).</response>
        [HttpGet("users")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _adminService.GetAllUsersAsync();
            return Ok(users);
        }

        /// <summary>
        /// Р‘Р»РѕРєСѓС” РґРѕСЃС‚СѓРї РєРѕСЂРёСЃС‚СѓРІР°С‡Р° РґРѕ СЃРёСЃС‚РµРјРё.
        /// </summary>
        /// <param name="dto">РћР±'С”РєС‚ РґР°РЅРёС… РґР»СЏ Р±Р»РѕРєСѓРІР°РЅРЅСЏ (ID РєРѕСЂРёСЃС‚СѓРІР°С‡Р° С‚Р° РїСЂРёС‡РёРЅР°).</param>
        /// <returns>РџРѕРІС–РґРѕРјР»РµРЅРЅСЏ РїСЂРѕ СѓСЃРїС–С€РЅРµ Р±Р»РѕРєСѓРІР°РЅРЅСЏ.</returns>
        /// <response code="200">РљРѕСЂРёСЃС‚СѓРІР°С‡Р° СѓСЃРїС–С€РЅРѕ Р·Р°Р±Р»РѕРєРѕРІР°РЅРѕ.</response>
        /// <response code="404">РљРѕСЂРёСЃС‚СѓРІР°С‡Р° РЅРµ Р·РЅР°Р№РґРµРЅРѕ.</response>
        [HttpPost("block")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> BlockUser([FromBody] BlockUserDto dto)
        {
            try
            {
                var adminId = GetCurrentUserId();

                var result = await _adminService.BlockUserAsync(adminId, dto);

                if (!result)
                    return NotFound("РљРѕСЂРёСЃС‚СѓРІР°С‡Р° РЅРµ Р·РЅР°Р№РґРµРЅРѕ.");

                return Ok(new { message = "РљРѕСЂРёСЃС‚СѓРІР°С‡Р° СѓСЃРїС–С€РЅРѕ Р·Р°Р±Р»РѕРєРѕРІР°РЅРѕ." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Р’С–РґРЅРѕРІР»СЋС” РґРѕСЃС‚СѓРї Р·Р°Р±Р»РѕРєРѕРІР°РЅРѕРіРѕ РєРѕСЂРёСЃС‚СѓРІР°С‡Р° РґРѕ СЃРёСЃС‚РµРјРё.
        /// </summary>
        /// <param name="userId">РЈРЅС–РєР°Р»СЊРЅРёР№ С–РґРµРЅС‚РёС„С–РєР°С‚РѕСЂ РєРѕСЂРёСЃС‚СѓРІР°С‡Р°.</param>
        /// <returns>РџРѕРІС–РґРѕРјР»РµРЅРЅСЏ РїСЂРѕ СЂРѕР·Р±Р»РѕРєСѓРІР°РЅРЅСЏ.</returns>
        /// <response code="200">РљРѕСЂРёСЃС‚СѓРІР°С‡Р° СѓСЃРїС–С€РЅРѕ СЂРѕР·Р±Р»РѕРєРѕРІР°РЅРѕ.</response>
        /// <response code="404">РљРѕСЂРёСЃС‚СѓРІР°С‡Р° РЅРµ Р·РЅР°Р№РґРµРЅРѕ.</response>
        [HttpPost("unblock/{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UnblockUser(int userId)
        {
            var adminId = GetCurrentUserId();
            var result = await _adminService.UnblockUserAsync(adminId, userId);

            if (!result)
                return NotFound("РљРѕСЂРёСЃС‚СѓРІР°С‡Р° РЅРµ Р·РЅР°Р№РґРµРЅРѕ.");

            return Ok(new { message = "РљРѕСЂРёСЃС‚СѓРІР°С‡Р° СЂРѕР·Р±Р»РѕРєРѕРІР°РЅРѕ." });
        }

        /// <summary>
        /// РћС‚СЂРёРјСѓС” Р¶СѓСЂРЅР°Р» Р°РєС‚РёРІРЅРѕСЃС‚С– Р°РґРјС–РЅС–СЃС‚СЂР°С‚РѕСЂС–РІ С‚Р° СЃРёСЃС‚РµРјРЅРёС… РїРѕРґС–Р№ (Audit Logs).
        /// </summary>
        /// <returns>РЎРїРёСЃРѕРє РѕСЃС‚Р°РЅРЅС–С… Р»РѕРіС–РІ.</returns>
        [HttpGet("logs")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetActivityLogs()
        {
            var logs = await _adminService.GetRecentLogsAsync();
            return Ok(logs);
        }

        private int GetCurrentUserId()
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            return claim != null ? int.Parse(claim.Value) : 0;
        }

        /// <summary>
        /// Р—РјС–РЅСЋС” СЂРѕР»СЊ РєРѕСЂРёСЃС‚СѓРІР°С‡Р° РІ СЃРёСЃС‚РµРјС– (РЅР°РїСЂРёРєР»Р°Рґ, Р· Customer РЅР° Manager).
        /// </summary>
        /// <param name="dto">Р”Р°РЅС– РґР»СЏ Р·РјС–РЅРё СЂРѕР»С– (ID РєРѕСЂРёСЃС‚СѓРІР°С‡Р° С‚Р° РЅРѕРІР° СЂРѕР»СЊ).</param>
        /// <returns>Р РµР·СѓР»СЊС‚Р°С‚ РѕРїРµСЂР°С†С–С—.</returns>
        /// <response code="200">Р РѕР»СЊ СѓСЃРїС–С€РЅРѕ Р·РјС–РЅРµРЅРѕ.</response>
        /// <response code="404">РљРѕСЂРёСЃС‚СѓРІР°С‡Р° РЅРµ Р·РЅР°Р№РґРµРЅРѕ.</response>
        [HttpPut("users/role")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ChangeRole([FromBody] ChangeRoleDto dto)
        {
            var adminId = GetCurrentUserId();
            var result = await _adminService.ChangeUserRoleAsync(adminId, dto);

            if (!result) return NotFound("РљРѕСЂРёСЃС‚СѓРІР°С‡Р° РЅРµ Р·РЅР°Р№РґРµРЅРѕ");

            return Ok(new { message = "Р РѕР»СЊ РєРѕСЂРёСЃС‚СѓРІР°С‡Р° СѓСЃРїС–С€РЅРѕ Р·РјС–РЅРµРЅРѕ." });
        }

        /// <summary>
        /// Р—РјС–РЅСЋС” СЃС‚Р°С‚СѓСЃ Р°РєС‚РёРІРЅРѕСЃС‚С– СЂРµСЃС‚РѕСЂР°РЅСѓ (Р±Р»РѕРєСѓРІР°РЅРЅСЏ Р°Р±Рѕ Р°РєС‚РёРІР°С†С–СЏ).
        /// </summary>
        /// <param name="id">ID СЂРµСЃС‚РѕСЂР°РЅСѓ.</param>
        /// <param name="isActive">РќРѕРІРёР№ СЃС‚Р°С‚СѓСЃ (true - Р°РєС‚РёРІРЅРёР№, false - Р·Р°Р±Р»РѕРєРѕРІР°РЅРёР№).</param>
        /// <returns>РџРѕРІС–РґРѕРјР»РµРЅРЅСЏ РїСЂРѕ Р·РјС–РЅСѓ СЃС‚Р°С‚СѓСЃСѓ.</returns>
        /// <response code="200">РЎС‚Р°С‚СѓСЃ СѓСЃРїС–С€РЅРѕ РѕРЅРѕРІР»РµРЅРѕ.</response>
        /// <response code="404">Р РµСЃС‚РѕСЂР°РЅ РЅРµ Р·РЅР°Р№РґРµРЅРѕ.</response>
        [HttpPatch("restaurants/{id}/status")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ToggleRestaurantStatus(int id, [FromQuery] bool isActive)
        {
            var adminId = GetCurrentUserId();
            var result = await _adminService.ToggleRestaurantStatusAsync(adminId, id, isActive);

            if (!result) return NotFound("Р РµСЃС‚РѕСЂР°РЅ РЅРµ Р·РЅР°Р№РґРµРЅРѕ");

            return Ok(new { message = $"РЎС‚Р°С‚СѓСЃ СЂРµСЃС‚РѕСЂР°РЅСѓ Р·РјС–РЅРµРЅРѕ РЅР° {(isActive ? "РђРєС‚РёРІРЅРёР№" : "Р—Р°Р±Р»РѕРєРѕРІР°РЅРёР№")}" });
        }

    }
}
