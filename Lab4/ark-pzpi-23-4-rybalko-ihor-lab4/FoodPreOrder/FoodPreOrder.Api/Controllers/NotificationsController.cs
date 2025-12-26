п»їusing FoodPreOrder.Persistence.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using FoodPreOrder.Application.DTOs.Notifications;

namespace FoodPreOrder.Api.Controllers
{
    /// <summary>
    /// РљРѕРЅС‚СЂРѕР»РµСЂ РґР»СЏ СѓРїСЂР°РІР»С–РЅРЅСЏ РѕСЃРѕР±РёСЃС‚РёРјРё СЃРїРѕРІС–С‰РµРЅРЅСЏРјРё РєРѕСЂРёСЃС‚СѓРІР°С‡Р°.
    /// Р”РѕР·РІРѕР»СЏС” РїРµСЂРµРіР»СЏРґР°С‚Рё С–СЃС‚РѕСЂС–СЋ РїРѕРІС–РґРѕРјР»РµРЅСЊ С‚Р° Р·РјС–РЅСЋРІР°С‚Рё С—С… СЃС‚Р°С‚СѓСЃ.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NotificationsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public NotificationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// РћС‚СЂРёРјСѓС” СЃРїРёСЃРѕРє СѓСЃС–С… СЃРїРѕРІС–С‰РµРЅСЊ РїРѕС‚РѕС‡РЅРѕРіРѕ РєРѕСЂРёСЃС‚СѓРІР°С‡Р°.
        /// </summary>
        /// <remarks>
        /// РџРѕРІРµСЂС‚Р°С” СЃРїРѕРІС–С‰РµРЅРЅСЏ РІС–РґСЃРѕСЂС‚РѕРІР°РЅС– Р·Р° РґР°С‚РѕСЋ (СЃРїРѕС‡Р°С‚РєСѓ РЅРѕРІС–).
        /// Р†РґРµРЅС‚РёС„С–РєР°С‚РѕСЂ РєРѕСЂРёСЃС‚СѓРІР°С‡Р° Р±РµСЂРµС‚СЊСЃСЏ Р°РІС‚РѕРјР°С‚РёС‡РЅРѕ Р· С‚РѕРєРµРЅР° Р°РІС‚РѕСЂРёР·Р°С†С–С—.
        /// </remarks>
        /// <returns>РЎРїРёСЃРѕРє СЃРїРѕРІС–С‰РµРЅСЊ (DTO).</returns>
        /// <response code="200">РЈСЃРїС–С€РЅРµ РѕС‚СЂРёРјР°РЅРЅСЏ СЃРїРёСЃРєСѓ.</response>
        /// <response code="401">РљРѕСЂРёСЃС‚СѓРІР°С‡ РЅРµ Р°РІС‚РѕСЂРёР·РѕРІР°РЅРёР№.</response>
        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(IEnumerable<NotificationDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<NotificationDto>>> GetMyNotifications()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var notifications = await _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.DateSent)
                .ToListAsync();

            var dtos = notifications.Select(n => new NotificationDto
            {
                Id = n.Id,
                Message = n.Message,
                DateSent = n.DateSent,
                IsRead = n.IsRead
            });

            return Ok(dtos);
        }

        /// <summary>
        /// РџРѕР·РЅР°С‡Р°С” РєРѕРЅРєСЂРµС‚РЅРµ СЃРїРѕРІС–С‰РµРЅРЅСЏ СЏРє РїСЂРѕС‡РёС‚Р°РЅРµ.
        /// </summary>
        /// <remarks>
        /// РљРѕСЂРёСЃС‚СѓРІР°С‡ РјРѕР¶Рµ РїРѕР·РЅР°С‡РёС‚Рё С‚С–Р»СЊРєРё РІР»Р°СЃРЅС– СЃРїРѕРІС–С‰РµРЅРЅСЏ.
        /// РЇРєС‰Рѕ СЃРїРѕРІС–С‰РµРЅРЅСЏ РЅР°Р»РµР¶РёС‚СЊ С–РЅС€РѕРјСѓ РєРѕСЂРёСЃС‚СѓРІР°С‡РµРІС–, РїРѕРІРµСЂРЅРµС‚СЊСЃСЏ РїРѕРјРёР»РєР° 404.
        /// </remarks>
        /// <param name="id">ID СЃРїРѕРІС–С‰РµРЅРЅСЏ.</param>
        /// <returns>РЎС‚Р°С‚СѓСЃ 200 OK.</returns>
        /// <response code="200">РЎРїРѕРІС–С‰РµРЅРЅСЏ СѓСЃРїС–С€РЅРѕ РѕРЅРѕРІР»РµРЅРѕ.</response>
        /// <response code="401">РљРѕСЂРёСЃС‚СѓРІР°С‡ РЅРµ Р°РІС‚РѕСЂРёР·РѕРІР°РЅРёР№.</response>
        /// <response code="404">РЎРїРѕРІС–С‰РµРЅРЅСЏ РЅРµ Р·РЅР°Р№РґРµРЅРѕ Р°Р±Рѕ РІРѕРЅРѕ РЅР°Р»РµР¶РёС‚СЊ С–РЅС€РѕРјСѓ РєРѕСЂРёСЃС‚СѓРІР°С‡РµРІС–.</response>
        [HttpPost("read/{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);

            if (notification == null) return NotFound();

            notification.IsRead = true;
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
