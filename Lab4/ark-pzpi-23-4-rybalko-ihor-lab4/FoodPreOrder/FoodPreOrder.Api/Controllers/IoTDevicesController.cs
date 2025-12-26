п»їusing FoodPreOrder.Api.Services;
using FoodPreOrder.Application.DTOs.IoT;
using FoodPreOrder.Domain.Enums;
using FoodPreOrder.Persistence.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace FoodPreOrder.Api.Controllers
{
    /// <summary>
    /// РљРѕРЅС‚СЂРѕР»РµСЂ РґР»СЏ СѓРїСЂР°РІР»С–РЅРЅСЏ С„С–Р·РёС‡РЅРёРјРё IoT-РїСЂРёСЃС‚СЂРѕСЏРјРё СЂРµСЃС‚РѕСЂР°РЅСѓ.
    /// Р—Р°Р±РµР·РїРµС‡СѓС” СЂРµС”СЃС‚СЂР°С†С–СЋ РѕР±Р»Р°РґРЅР°РЅРЅСЏ С‚Р° РјРѕРЅС–С‚РѕСЂРёРЅРі Р№РѕРіРѕ СЃС‚Р°РЅСѓ С‡РµСЂРµР· РјРµС…Р°РЅС–Р·Рј "Ping".
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class IoTDevicesController : ControllerBase
    {
        private readonly IoTService _iotService;
        private readonly ApplicationDbContext _context;

        public IoTDevicesController(IoTService iotService, ApplicationDbContext context)
        {
            _iotService = iotService;
            _context = context;
        }

        /// <summary>
        /// Р РµС”СЃС‚СЂСѓС” РЅРѕРІРёР№ IoT-РїСЂРёСЃС‚СЂС–Р№ Сѓ СЃРёСЃС‚РµРјС–.
        /// </summary>
        /// <remarks>
        /// Р”РѕСЃС‚СѓРїРЅРѕ РґР»СЏ Admin С‚Р° RestaurantOwner. 
        /// Р’Р»Р°СЃРЅРёРє РјРѕР¶Рµ РґРѕРґР°РІР°С‚Рё РїСЂРёСЃС‚СЂРѕС— Р»РёС€Рµ РґРѕ СЃРІРѕС—С… СЂРµСЃС‚РѕСЂР°РЅС–РІ.
        /// РџСЂРёСЃС‚СЂС–Р№ РѕС‚СЂРёРјСѓС” СЃС‚Р°С‚СѓСЃ "New" С– СЃС‚Р°С” Р°РєС‚РёРІРЅРёРј РїС–СЃР»СЏ РїРµСЂС€РѕРіРѕ РїС–РЅРіСѓ.
        /// </remarks>
        /// <param name="dto">Р”Р°РЅС– РґР»СЏ СЂРµС”СЃС‚СЂР°С†С–С— (РЎРµСЂС–Р№РЅРёР№ РЅРѕРјРµСЂ, РќР°Р·РІР° Р»РѕРєР°С†С–С—, ID СЂРµСЃС‚РѕСЂР°РЅСѓ).</param>
        /// <returns>РЎС‚РІРѕСЂРµРЅРёР№ РѕР±'С”РєС‚ РїСЂРёСЃС‚СЂРѕСЋ.</returns>
        /// <response code="200">РџСЂРёСЃС‚СЂС–Р№ СѓСЃРїС–С€РЅРѕ Р·Р°СЂРµС”СЃС‚СЂРѕРІР°РЅРѕ.</response>
        /// <response code="400">РџРѕРјРёР»РєР° РІР°Р»С–РґР°С†С–С— Р°Р±Рѕ РїСЂРёСЃС‚СЂС–Р№ Р· С‚Р°РєРёРј СЃРµСЂС–Р№РЅРёРј РЅРѕРјРµСЂРѕРј РІР¶Рµ С–СЃРЅСѓС”.</response>
        /// <response code="401">РљРѕСЂРёСЃС‚СѓРІР°С‡ РЅРµ Р°РІС‚РѕСЂРёР·РѕРІР°РЅРёР№.</response>
        /// <response code="403">РЎРїСЂРѕР±Р° РґРѕРґР°С‚Рё РїСЂРёСЃС‚СЂС–Р№ Сѓ С‡СѓР¶РёР№ СЂРµСЃС‚РѕСЂР°РЅ.</response>
        [HttpPost]
        [Authorize(Roles = "Admin,RestaurantOwner")]
        [ProducesResponseType(typeof(IoTDeviceDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<IoTDeviceDto>> CreateDevice([FromBody] CreateIoTDeviceDto dto)
        {
            if (!await CheckAccess(dto.RestaurantId))
                return StatusCode(403, "Р’Рё РЅРµ РјРѕР¶РµС‚Рµ РґРѕРґР°РІР°С‚Рё РѕР±Р»Р°РґРЅР°РЅРЅСЏ РІ С‡СѓР¶РёР№ СЂРµСЃС‚РѕСЂР°РЅ!");

            try
            {
                var device = await _iotService.RegisterDeviceAsync(dto);

                return Ok(new IoTDeviceDto
                {
                    Id = device.Id,
                    SerialNumber = device.SerialNumber,
                    LocationName = device.LocationName,
                    IsActive = device.IsActive,
                    LastPing = device.LastPing,
                    Status = "New"
                });
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// РћС‚СЂРёРјСѓС” СЃРїРёСЃРѕРє СѓСЃС–С… РїСЂРёСЃС‚СЂРѕС—РІ РєРѕРЅРєСЂРµС‚РЅРѕРіРѕ СЂРµСЃС‚РѕСЂР°РЅСѓ.
        /// </summary>
        /// <remarks>
        /// Р’РёРєРѕСЂРёСЃС‚РѕРІСѓС”С‚СЊСЃСЏ РґР»СЏ РјРѕРЅС–С‚РѕСЂРёРЅРіСѓ: РґРѕР·РІРѕР»СЏС” РїРѕР±Р°С‡РёС‚Рё, СЏРєС– РїСЂРёСЃС‚СЂРѕС— РѕРЅР»Р°Р№РЅ (РІС–РґРїСЂР°РІР»СЏР»Рё РїС–РЅРі РЅРµС‰РѕРґР°РІРЅРѕ).
        /// </remarks>
        /// <param name="restaurantId">ID СЂРµСЃС‚РѕСЂР°РЅСѓ.</param>
        /// <returns>РЎРїРёСЃРѕРє РїСЂРёСЃС‚СЂРѕС—РІ.</returns>
        /// <response code="200">РЎРїРёСЃРѕРє СѓСЃРїС–С€РЅРѕ РѕС‚СЂРёРјР°РЅРѕ.</response>
        /// <response code="401">РљРѕСЂРёСЃС‚СѓРІР°С‡ РЅРµ Р°РІС‚РѕСЂРёР·РѕРІР°РЅРёР№.</response>
        /// <response code="403">РЎРїСЂРѕР±Р° РїРµСЂРµРіР»СЏРЅСѓС‚Рё РїСЂРёСЃС‚СЂРѕС— С‡СѓР¶РѕРіРѕ СЂРµСЃС‚РѕСЂР°РЅСѓ.</response>
        [HttpGet("restaurant/{restaurantId}")]
        [Authorize(Roles = "Admin,RestaurantOwner")]
        [ProducesResponseType(typeof(IEnumerable<IoTDeviceDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<IEnumerable<IoTDeviceDto>>> GetDevices(int restaurantId)
        {
            if (!await CheckAccess(restaurantId))
                return StatusCode(403, "Р¦Рµ РЅРµ РІР°С€ СЂРµСЃС‚РѕСЂР°РЅ.");

            var devices = await _iotService.GetRestaurantDevicesAsync(restaurantId);
            return Ok(devices);
        }

        /// <summary>
        /// РџСЂРёР№РјР°С” СЃРёРіРЅР°Р» "Heartbeat" (Ping) РІС–Рґ С„С–Р·РёС‡РЅРѕРіРѕ РїСЂРёСЃС‚СЂРѕСЋ.
        /// </summary>
        /// <remarks>
        /// Р¦РµР№ РјРµС‚РѕРґ РІРёРєР»РёРєР°С”С‚СЊСЃСЏ Р°РїР°СЂР°С‚РЅРёРј Р·Р°Р±РµР·РїРµС‡РµРЅРЅСЏРј (РјС–РєСЂРѕРєРѕРЅС‚СЂРѕР»РµСЂРѕРј Р°Р±Рѕ СЃРєСЂРёРїС‚РѕРј РЅР° С‚РµСЂРјС–РЅР°Р»С–) РєРѕР¶РЅС– N СЃРµРєСѓРЅРґ.
        /// РќРµ РІРёРјР°РіР°С” Р°РІС‚РѕСЂРёР·Р°С†С–С— РєРѕСЂРёСЃС‚СѓРІР°С‡Р° (JWT), С–РґРµРЅС‚РёС„С–РєР°С†С–СЏ РІС–РґР±СѓРІР°С”С‚СЊСЃСЏ Р·Р° СЃРµСЂС–Р№РЅРёРј РЅРѕРјРµСЂРѕРј.
        /// РћРЅРѕРІР»СЋС” РїРѕР»Рµ LastPing Сѓ Р±Р°Р·С– РґР°РЅРёС….
        /// </remarks>
        /// <param name="serialNumber">РЈРЅС–РєР°Р»СЊРЅРёР№ СЃРµСЂС–Р№РЅРёР№ РЅРѕРјРµСЂ РїСЂРёСЃС‚СЂРѕСЋ (СЂСЏРґРѕРє).</param>
        /// <returns>РџРѕС‚РѕС‡РЅРёР№ С‡Р°СЃ СЃРµСЂРІРµСЂР° (Pong).</returns>
        /// <response code="200">Ping СѓСЃРїС–С€РЅРёР№, РїСЂРёСЃС‚СЂС–Р№ СЂРѕР·РїС–Р·РЅР°РЅРѕ.</response>
        /// <response code="404">РџСЂРёСЃС‚СЂС–Р№ Р· С‚Р°РєРёРј СЃРµСЂС–Р№РЅРёРј РЅРѕРјРµСЂРѕРј РЅРµ Р·Р°СЂРµС”СЃС‚СЂРѕРІР°РЅРѕ РІ СЃРёСЃС‚РµРјС–.</response>
        [HttpPost("ping")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Ping([FromBody] string serialNumber)
        {
            var result = await _iotService.PingDeviceAsync(serialNumber);

            if (!result) return NotFound("Unknown device");

            return Ok(new { message = "Pong", serverTime = System.DateTime.UtcNow });
        }

        /// <summary>
        /// Р’РёРґР°Р»СЏС” РїСЂРёСЃС‚СЂС–Р№ С–Р· СЃРёСЃС‚РµРјРё.
        /// </summary>
        /// <remarks>
        /// Р”РѕСЃС‚СѓРїРЅРѕ Admin С‚Р° RestaurantOwner (С‚С–Р»СЊРєРё РґР»СЏ СЃРІРѕС—С… РїСЂРёСЃС‚СЂРѕС—РІ).
        /// </remarks>
        /// <param name="id">ID РїСЂРёСЃС‚СЂРѕСЋ.</param>
        /// <returns>РџРѕРІС–РґРѕРјР»РµРЅРЅСЏ РїСЂРѕ СѓСЃРїС–С€РЅРµ РІРёРґР°Р»РµРЅРЅСЏ.</returns>
        /// <response code="200">РџСЂРёСЃС‚СЂС–Р№ РІРёРґР°Р»РµРЅРѕ.</response>
        /// <response code="401">РљРѕСЂРёСЃС‚СѓРІР°С‡ РЅРµ Р°РІС‚РѕСЂРёР·РѕРІР°РЅРёР№.</response>
        /// <response code="403">РЎРїСЂРѕР±Р° РІРёРґР°Р»РёС‚Рё РїСЂРёСЃС‚СЂС–Р№ С‡СѓР¶РѕРіРѕ СЂРµСЃС‚РѕСЂР°РЅСѓ.</response>
        /// <response code="404">РџСЂРёСЃС‚СЂС–Р№ РЅРµ Р·РЅР°Р№РґРµРЅРѕ.</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,RestaurantOwner")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteDevice(int id)
        {
            var device = await _iotService.GetDeviceByIdAsync(id);
            if (device == null) return NotFound("РџСЂРёСЃС‚СЂС–Р№ РЅРµ Р·РЅР°Р№РґРµРЅРѕ.");

            if (!await CheckAccess(device.RestaurantId))
                return StatusCode(403, "Р’Рё РЅРµ РјРѕР¶РµС‚Рµ РІРёРґР°Р»СЏС‚Рё РїСЂРёСЃС‚СЂРѕС— Р· С‡СѓР¶РѕРіРѕ СЂРµСЃС‚РѕСЂР°РЅСѓ.");

            await _iotService.DeleteDeviceAsync(id);

            return Ok(new { message = "РџСЂРёСЃС‚СЂС–Р№ РІРёРґР°Р»РµРЅРѕ." });
        }

        private async Task<bool> CheckAccess(int restaurantId)
        {
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            if (userRole == UserRole.Admin.ToString()) return true;

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var restaurant = await _context.Restaurants.FindAsync(restaurantId);

            return restaurant != null && restaurant.OwnerId == userId;
        }
    }
}
