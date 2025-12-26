п»їusing FoodPreOrder.Application.DTOs.Auth;
using FoodPreOrder.Application.DTOs.Restaurants;
using FoodPreOrder.Domain.Entities;
using FoodPreOrder.Domain.Enums;
using FoodPreOrder.Persistence.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FoodPreOrder.Api.Controllers
{
    /// <summary>
    /// РљРѕРЅС‚СЂРѕР»РµСЂ РґР»СЏ СѓРїСЂР°РІР»С–РЅРЅСЏ РїРµСЂСЃРѕРЅР°Р»РѕРј СЂРµСЃС‚РѕСЂР°РЅСѓ (HR РјРѕРґСѓР»СЊ).
    /// Р”РѕР·РІРѕР»СЏС” РІР»Р°СЃРЅРёРєР°Рј СЂРµСЃС‚РѕСЂР°РЅС–РІ РЅР°Р№РјР°С‚Рё С‚Р° Р·РІС–Р»СЊРЅСЏС‚Рё РїСЂР°С†С–РІРЅРёРєС–РІ РєСѓС…РЅС–.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class StaffController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public StaffController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// РџРµСЂРµРІС–СЂСЏС” РєР°РЅРґРёРґР°С‚Р° РїРµСЂРµРґ РЅР°Р№РјРѕРј Р·Р° Email.
        /// </summary>
        /// <remarks>
        /// Р’РёРєРѕРЅСѓС” РїРµСЂРµРІС–СЂРєСѓ, С‡Рё РјРѕР¶РЅР° РЅР°Р№РЅСЏС‚Рё С†СЋ Р»СЋРґРёРЅСѓ:
        /// - РљРѕСЂРёСЃС‚СѓРІР°С‡ РїРѕРІРёРЅРµРЅ С–СЃРЅСѓРІР°С‚Рё.
        /// - РќРµ РјРѕР¶РЅР° РЅР°Р№РЅСЏС‚Рё РђРґРјС–РЅР° Р°Р±Рѕ Р’Р»Р°СЃРЅРёРєР° С–РЅС€РѕРіРѕ СЂРµСЃС‚РѕСЂР°РЅСѓ.
        /// - РќРµ РјРѕР¶РЅР° РЅР°Р№РЅСЏС‚Рё Р»СЋРґРёРЅСѓ, СЏРєР° РІР¶Рµ РїСЂР°С†СЋС” РІ С–РЅС€РѕРјСѓ Р·Р°РєР»Р°РґС–.
        /// </remarks>
        /// <param name="email">Email РєРѕСЂРёСЃС‚СѓРІР°С‡Р°.</param>
        /// <returns>РџСЂРѕС„С–Р»СЊ РєРѕСЂРёСЃС‚СѓРІР°С‡Р°, СЏРєС‰Рѕ РІС–РЅ РїС–РґС…РѕРґРёС‚СЊ.</returns>
        /// <response code="200">РљРѕСЂРёСЃС‚СѓРІР°С‡ РґРѕСЃС‚СѓРїРЅРёР№ РґР»СЏ РЅР°Р№РјСѓ.</response>
        /// <response code="400">РљРѕСЂРёСЃС‚СѓРІР°С‡ Р·Р°Р№РЅСЏС‚РёР№ Р°Р±Рѕ РјР°С” РЅРµРїСЂРёРїСѓСЃС‚РёРјСѓ СЂРѕР»СЊ.</response>
        /// <response code="404">РљРѕСЂРёСЃС‚СѓРІР°С‡Р° Р· С‚Р°РєРёРј Email РЅРµ Р·РЅР°Р№РґРµРЅРѕ.</response>
        [HttpGet("check-user")]
        [Authorize(Roles = "Admin,RestaurantOwner")]
        [ProducesResponseType(typeof(UserProfileDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserProfileDto>> CheckUser([FromQuery] string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null) return NotFound("РљРѕСЂРёСЃС‚СѓРІР°С‡Р° РЅРµ Р·РЅР°Р№РґРµРЅРѕ.");

            if (user.Role == UserRole.Admin || user.Role == UserRole.RestaurantOwner)
                return BadRequest($"РќРµРјРѕР¶Р»РёРІРѕ РЅР°Р№РЅСЏС‚Рё РєРѕСЂРёСЃС‚СѓРІР°С‡Р° Р· СЂРѕР»Р»СЋ {user.Role}.");

            if (user.Role == UserRole.KitchenStaff && user.RestaurantId != null)
            {
                var restaurant = await _context.Restaurants.FindAsync(user.RestaurantId);
                var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

                if (restaurant != null && restaurant.OwnerId == currentUserId)
                {
                    return BadRequest($"Р¦РµР№ РєРѕСЂРёСЃС‚СѓРІР°С‡ РІР¶Рµ РїСЂР°С†СЋС” Сѓ Р’РђРЎ (Р РµСЃС‚РѕСЂР°РЅ: {restaurant.NameUA}).");
                }
                else
                {
                    return BadRequest("Р¦РµР№ РєРѕСЂРёСЃС‚СѓРІР°С‡ РІР¶Рµ Р·Р°Р№РЅСЏС‚РёР№ (РїСЂР°С†СЋС” РІ С–РЅС€РѕРјСѓ СЂРµСЃС‚РѕСЂР°РЅС–).");
                }
            }

            return Ok(new UserProfileDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Phone = user.Phone,
                Role = user.Role.ToString()
            });
        }

        /// <summary>
        /// РќР°Р№РјР°С” РїСЂР°С†С–РІРЅРёРєР° Сѓ РІРєР°Р·Р°РЅРёР№ СЂРµСЃС‚РѕСЂР°РЅ.
        /// </summary>
        /// <remarks>
        /// Р—РјС–РЅСЋС” СЂРѕР»СЊ РєРѕСЂРёСЃС‚СѓРІР°С‡Р° РЅР° `KitchenStaff` С‚Р° РїСЂРёРІ'СЏР·СѓС” Р№РѕРіРѕ РґРѕ `RestaurantId`.
        /// Р’Р»Р°СЃРЅРёРє РјРѕР¶Рµ РЅР°Р№РјР°С‚Рё Р»СЋРґРµР№ С‚С–Р»СЊРєРё Сѓ СЃРІРѕС— СЂРµСЃС‚РѕСЂР°РЅРё.
        /// </remarks>
        /// <param name="dto">Р”Р°РЅС– РґР»СЏ РЅР°Р№РјСѓ (Email С‚Р° ID СЂРµСЃС‚РѕСЂР°РЅСѓ).</param>
        /// <returns>Р РµР·СѓР»СЊС‚Р°С‚ РѕРїРµСЂР°С†С–С—.</returns>
        /// <response code="200">РџСЂР°С†С–РІРЅРёРєР° СѓСЃРїС–С€РЅРѕ РЅР°Р№РЅСЏС‚Рѕ.</response>
        /// <response code="400">РџРѕРјРёР»РєР° РІР°Р»С–РґР°С†С–С— (РєРѕСЂРёСЃС‚СѓРІР°С‡ РІР¶Рµ РїСЂР°С†СЋС” Р°Р±Рѕ РјР°С” С–РЅС€Сѓ СЂРѕР»СЊ).</response>
        /// <response code="403">РЎРїСЂРѕР±Р° РЅР°Р№РЅСЏС‚Рё РїСЂР°С†С–РІРЅРёРєР° Сѓ С‡СѓР¶РёР№ СЂРµСЃС‚РѕСЂР°РЅ.</response>
        /// <response code="404">Р РµСЃС‚РѕСЂР°РЅ Р°Р±Рѕ РєРѕСЂРёСЃС‚СѓРІР°С‡Р° РЅРµ Р·РЅР°Р№РґРµРЅРѕ.</response>
        [HttpPost("hire")]
        [Authorize(Roles = "Admin,RestaurantOwner")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> HireStaff([FromBody] HireStaffDto dto)
        {
            var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var currentUserRole = User.FindFirstValue(ClaimTypes.Role);

            var restaurant = await _context.Restaurants.FindAsync(dto.RestaurantId);
            if (restaurant == null) return NotFound("Р РµСЃС‚РѕСЂР°РЅ РЅРµ Р·РЅР°Р№РґРµРЅРѕ.");

            if (currentUserRole == UserRole.RestaurantOwner.ToString() && restaurant.OwnerId != currentUserId)
            {
                return StatusCode(403, "Р’Рё РЅРµ РјРѕР¶РµС‚Рµ РЅР°Р№РјР°С‚Рё Р»СЋРґРµР№ Сѓ С‡СѓР¶РёР№ СЂРµСЃС‚РѕСЂР°РЅ!");
            }

            var targetUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (targetUser == null) return BadRequest("РљРѕСЂРёСЃС‚СѓРІР°С‡ РЅРµ Р·РЅР°Р№РґРµРЅРёР№.");

            if (targetUser.Role == UserRole.RestaurantOwner || targetUser.Role == UserRole.Admin)
                return BadRequest("РќРµ РјРѕР¶РЅР° РЅР°Р№РЅСЏС‚Рё РІР»Р°СЃРЅРёРєР° Р°Р±Рѕ Р°РґРјС–РЅР°.");

            if (targetUser.Role == UserRole.KitchenStaff)
                return BadRequest("Р¦РµР№ РєРѕСЂРёСЃС‚СѓРІР°С‡ РІР¶Рµ РїСЂР°С†РµРІР»Р°С€С‚РѕРІР°РЅРёР№.");

            targetUser.Role = UserRole.KitchenStaff;
            targetUser.RestaurantId = dto.RestaurantId;

            var notification = new Notification
            {
                UserId = targetUser.Id,
                Message = $"Р’С–С‚Р°С”РјРѕ! Р’Р°СЃ РїСЂРёР№РЅСЏС‚Рѕ РЅР° СЂРѕР±РѕС‚Сѓ РІ СЂРµСЃС‚РѕСЂР°РЅ '{restaurant.NameUA}'.",
                DateSent = DateTime.UtcNow,
                IsRead = false
            };
            _context.Notifications.Add(notification);

            await _context.SaveChangesAsync();

            return Ok(new { message = $"РљРѕСЂРёСЃС‚СѓРІР°С‡Р° {targetUser.FullName} СѓСЃРїС–С€РЅРѕ РЅР°Р№РЅСЏС‚Рѕ." });
        }

        /// <summary>
        /// Р—РІС–Р»СЊРЅСЏС” РїСЂР°С†С–РІРЅРёРєР° Р· СЂРµСЃС‚РѕСЂР°РЅСѓ.
        /// </summary>
        /// <remarks>
        /// Р—РјС–РЅСЋС” СЂРѕР»СЊ РєРѕСЂРёСЃС‚СѓРІР°С‡Р° РЅР°Р·Р°Рґ РЅР° `Customer` С‚Р° РІРёРґР°Р»СЏС” РїСЂРёРІ'СЏР·РєСѓ РґРѕ СЂРµСЃС‚РѕСЂР°РЅСѓ.
        /// </remarks>
        /// <param name="dto">Р”Р°РЅС– РґР»СЏ Р·РІС–Р»СЊРЅРµРЅРЅСЏ (Email С‚Р° ID СЂРµСЃС‚РѕСЂР°РЅСѓ).</param>
        /// <returns>Р РµР·СѓР»СЊС‚Р°С‚ РѕРїРµСЂР°С†С–С—.</returns>
        /// <response code="200">РџСЂР°С†С–РІРЅРёРєР° СѓСЃРїС–С€РЅРѕ Р·РІС–Р»СЊРЅРµРЅРѕ.</response>
        /// <response code="400">РљРѕСЂРёСЃС‚СѓРІР°С‡ РЅРµ С” РїСЂР°С†С–РІРЅРёРєРѕРј С†СЊРѕРіРѕ СЂРµСЃС‚РѕСЂР°РЅСѓ.</response>
        /// <response code="403">РЎРїСЂРѕР±Р° Р·РІС–Р»СЊРЅРёС‚Рё РїСЂР°С†С–РІРЅРёРєР° Р· С‡СѓР¶РѕРіРѕ СЂРµСЃС‚РѕСЂР°РЅСѓ.</response>
        /// <response code="404">РџСЂР°С†С–РІРЅРёРєР° РЅРµ Р·РЅР°Р№РґРµРЅРѕ.</response>
        [HttpPost("fire")]
        [Authorize(Roles = "Admin,RestaurantOwner")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> FireStaff([FromBody] FireStaffDto dto)
        {
            var targetUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (targetUser == null) return NotFound("РџСЂР°С†С–РІРЅРёРєР° Р· С‚Р°РєРёРј Email РЅРµ Р·РЅР°Р№РґРµРЅРѕ.");

            if (targetUser.Role != UserRole.KitchenStaff || targetUser.RestaurantId == null)
            {
                return BadRequest("Р¦РµР№ РєРѕСЂРёСЃС‚СѓРІР°С‡ РЅРµ С” Р°РєС‚РёРІРЅРёРј РїСЂР°С†С–РІРЅРёРєРѕРј РєСѓС…РЅС–.");
            }

            if (targetUser.RestaurantId != dto.RestaurantId)
            {
                return BadRequest("Р¦РµР№ РєРѕСЂРёСЃС‚СѓРІР°С‡ РїСЂР°С†СЋС” РІ С–РЅС€РѕРјСѓ СЂРµСЃС‚РѕСЂР°РЅС–, Р° РЅРµ РІ Р·Р°Р·РЅР°С‡РµРЅРѕРјСѓ.");
            }

            var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var currentUserRole = User.FindFirstValue(ClaimTypes.Role);

            var restaurant = await _context.Restaurants.FindAsync(dto.RestaurantId);
            if (restaurant == null) return NotFound("Р РµСЃС‚РѕСЂР°РЅ РЅРµ Р·РЅР°Р№РґРµРЅРѕ.");

            if (currentUserRole == UserRole.RestaurantOwner.ToString() && restaurant.OwnerId != currentUserId)
            {
                return StatusCode(403, "Р’Рё РЅРµ РјРѕР¶РµС‚Рµ Р·РІС–Р»СЊРЅСЏС‚Рё РїСЂР°С†С–РІРЅРёРєС–РІ Р· С‡СѓР¶РѕРіРѕ СЂРµСЃС‚РѕСЂР°РЅСѓ!");
            }

            targetUser.Role = UserRole.Customer;
            targetUser.RestaurantId = null;

            var notification = new Notification
            {
                UserId = targetUser.Id,
                Message = $"Р’Р°С€Р° СЂРѕР±РѕС‚Р° РІ Р·Р°РєР»Р°РґС– '{restaurant.NameUA}' Р·Р°РІРµСЂС€РµРЅР°. Р’Р°С€Сѓ СЂРѕР»СЊ Р·РјС–РЅРµРЅРѕ РЅР° РљР»С–С”РЅС‚.",
                DateSent = DateTime.UtcNow,
                IsRead = false
            };
            _context.Notifications.Add(notification);

            await _context.SaveChangesAsync();
            return Ok(new { message = $"РџСЂР°С†С–РІРЅРёРєР° {targetUser.FullName} СѓСЃРїС–С€РЅРѕ Р·РІС–Р»СЊРЅРµРЅРѕ." });
        }

        /// <summary>
        /// РћС‚СЂРёРјСѓС” СЃРїРёСЃРѕРє СѓСЃС–С… РїСЂР°С†С–РІРЅРёРєС–РІ РєСѓС…РЅС– РґР»СЏ РєРѕРЅРєСЂРµС‚РЅРѕРіРѕ СЂРµСЃС‚РѕСЂР°РЅСѓ.
        /// </summary>
        /// <param name="restaurantId">ID СЂРµСЃС‚РѕСЂР°РЅСѓ.</param>
        /// <returns>РЎРїРёСЃРѕРє РїСЂРѕС„С–Р»С–РІ РїСЂР°С†С–РІРЅРёРєС–РІ.</returns>
        /// <response code="200">РЎРїРёСЃРѕРє СѓСЃРїС–С€РЅРѕ РѕС‚СЂРёРјР°РЅРѕ.</response>
        /// <response code="403">РЎРїСЂРѕР±Р° РїРµСЂРµРіР»СЏРЅСѓС‚Рё РїРµСЂСЃРѕРЅР°Р» С‡СѓР¶РѕРіРѕ СЂРµСЃС‚РѕСЂР°РЅСѓ.</response>
        [HttpGet("restaurant/{restaurantId}")]
        [Authorize(Roles = "Admin,RestaurantOwner")]
        [ProducesResponseType(typeof(IEnumerable<UserProfileDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<IEnumerable<UserProfileDto>>> GetStaff(int restaurantId)
        {
            var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var currentUserRole = User.FindFirstValue(ClaimTypes.Role);

            if (currentUserRole == UserRole.RestaurantOwner.ToString())
            {
                var restaurant = await _context.Restaurants.FindAsync(restaurantId);
                if (restaurant == null || restaurant.OwnerId != currentUserId)
                    return StatusCode(403, "Р¦Рµ РЅРµ РІР°С€ СЂРµСЃС‚РѕСЂР°РЅ.");
            }

            var staff = await _context.Users
                .Where(u => u.RestaurantId == restaurantId && u.Role == UserRole.KitchenStaff)
                .Select(u => new UserProfileDto
                {
                    Id = u.Id,
                    FullName = u.FullName,
                    Email = u.Email,
                    Phone = u.Phone,
                    Role = u.Role.ToString()
                })
                .ToListAsync();

            return Ok(staff);
        }
    }
}
