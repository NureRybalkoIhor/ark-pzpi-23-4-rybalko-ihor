п»їusing FoodPreOrder.Api.Services;
using FoodPreOrder.Application.DTOs;
using FoodPreOrder.Application.DTOs.Restaurants;
using FoodPreOrder.Domain.Entities;
using FoodPreOrder.Domain.Enums;
using FoodPreOrder.Persistence.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FoodPreOrder.Api.Controllers
{
    /// <summary>
    /// РљРѕРЅС‚СЂРѕР»РµСЂ РґР»СЏ СѓРїСЂР°РІР»С–РЅРЅСЏ Р·Р°РєР»Р°РґР°РјРё С…Р°СЂС‡СѓРІР°РЅРЅСЏ (СЂРµСЃС‚РѕСЂР°РЅР°РјРё).
    /// Р”РѕР·РІРѕР»СЏС” С€СѓРєР°С‚Рё Р·Р°РєР»Р°РґРё (РІ С‚РѕРјСѓ С‡РёСЃР»С– Р·Р° РіРµРѕР»РѕРєР°С†С–С”СЋ), СЃС‚РІРѕСЂСЋРІР°С‚Рё РЅРѕРІС– С‚Р° РІРёРґР°Р»СЏС‚Рё С–СЃРЅСѓСЋС‡С–.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class RestaurantsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ICalculationService _calcService;

        public RestaurantsController(ApplicationDbContext context, ICalculationService calcService)
        {
            _context = context;
            _calcService = calcService;
        }

        /// <summary>
        /// РћС‚СЂРёРјСѓС” СЃРїРёСЃРѕРє СЂРµСЃС‚РѕСЂР°РЅС–РІ.
        /// </summary>
        /// <remarks>
        /// РџС–РґС‚СЂРёРјСѓС” СЃРѕСЂС‚СѓРІР°РЅРЅСЏ Р·Р° РІС–РґСЃС‚Р°РЅРЅСЋ РґРѕ РєРѕСЂРёСЃС‚СѓРІР°С‡Р°.
        /// - РЇРєС‰Рѕ РїРµСЂРµРґР°С‚Рё `userLat` С‚Р° `userLon`, СЃРїРёСЃРѕРє Р±СѓРґРµ РІС–РґСЃРѕСЂС‚РѕРІР°РЅРѕ РІС–Рґ РЅР°Р№Р±Р»РёР¶С‡РѕРіРѕ РґРѕ РЅР°Р№РґР°Р»СЊС€РѕРіРѕ.
        /// - РЇРєС‰Рѕ РєРѕРѕСЂРґРёРЅР°С‚Рё РЅРµ РїРµСЂРµРґР°РЅРѕ, РїРѕРІРµСЂС‚Р°С”С‚СЊСЃСЏ Р·РІРёС‡Р°Р№РЅРёР№ СЃРїРёСЃРѕРє.
        /// </remarks>
        /// <param name="userLat">РЁРёСЂРѕС‚Р° РєРѕСЂРёСЃС‚СѓРІР°С‡Р° (Latitude).</param>
        /// <param name="userLon">Р”РѕРІРіРѕС‚Р° РєРѕСЂРёСЃС‚СѓРІР°С‡Р° (Longitude).</param>
        /// <returns>РЎРїРёСЃРѕРє СЂРµСЃС‚РѕСЂР°РЅС–РІ (DTO).</returns>
        /// <response code="200">РЈСЃРїС–С€РЅРµ РѕС‚СЂРёРјР°РЅРЅСЏ СЃРїРёСЃРєСѓ.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<RestaurantDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<object>>> GetRestaurants([FromQuery] double? userLat, [FromQuery] double? userLon)
        {
            var restaurants = await _context.Restaurants
                                            .Include(r => r.Owner)
                                            .ToListAsync();

            if (!userLat.HasValue || !userLon.HasValue)
            {
                var dtos = restaurants.Select(r => new RestaurantDto
                {
                    Id = r.Id,
                    NameUA = r.NameUA,
                    NameEN = r.NameEN,
                    Address = r.Address,
                    ImageUrl = r.ImageUrl,
                    Latitude = r.Latitude,
                    Longitude = r.Longitude,
                    IsActive = r.IsActive,
                    PaidUntil = r.PaidUntil,
                    OwnerId = r.OwnerId,
                    Owner = r.Owner == null ? null : new OwnerDto
                    {
                        Id = r.Owner.Id,
                        FullName = r.Owner.FullName,
                        Email = r.Owner.Email
                    }
                });

                return Ok(dtos);
            }

            var sortedRestaurants = restaurants
                .Select(r => new
                {
                    Restaurant = r,
                    DistanceKm = _calcService.CalculateDistance(userLat.Value, userLon.Value, r.Latitude, r.Longitude)
                })
                .OrderBy(x => x.DistanceKm)
                .Select(x => new RestaurantDto
                {
                    Id = x.Restaurant.Id,
                    NameUA = x.Restaurant.NameUA,
                    NameEN = x.Restaurant.NameEN,
                    Address = x.Restaurant.Address,
                    ImageUrl = x.Restaurant.ImageUrl,
                    Latitude = x.Restaurant.Latitude,
                    Longitude = x.Restaurant.Longitude,
                    IsActive = x.Restaurant.IsActive,
                    PaidUntil = x.Restaurant.PaidUntil,
                    OwnerId = x.Restaurant.OwnerId,
                    Owner = x.Restaurant.Owner == null ? null : new OwnerDto
                    {
                        Id = x.Restaurant.Owner.Id,
                        FullName = x.Restaurant.Owner.FullName,
                        Email = x.Restaurant.Owner.Email
                    }
                });

            return Ok(sortedRestaurants);
        }

        /// <summary>
        /// РћС‚СЂРёРјСѓС” РґРµС‚Р°Р»СЊРЅСѓ С–РЅС„РѕСЂРјР°С†С–СЋ РїСЂРѕ СЂРµСЃС‚РѕСЂР°РЅ Р·Р° Р№РѕРіРѕ ID.
        /// </summary>
        /// <param name="id">ID СЂРµСЃС‚РѕСЂР°РЅСѓ.</param>
        /// <returns>DTO СЂРµСЃС‚РѕСЂР°РЅСѓ.</returns>
        /// <response code="200">Р РµСЃС‚РѕСЂР°РЅ Р·РЅР°Р№РґРµРЅРѕ.</response>
        /// <response code="404">Р—Р°РєР»Р°Рґ РЅРµ Р·РЅР°Р№РґРµРЅРѕ.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(RestaurantDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<RestaurantDto>> GetRestaurant(int id)
        {
            var r = await _context.Restaurants
                .Include(rest => rest.Owner)
                .FirstOrDefaultAsync(rest => rest.Id == id);

            if (r == null)
            {
                return NotFound("Р—Р°РєР»Р°Рґ РЅРµ Р·РЅР°Р№РґРµРЅРѕ");
            }

            var restaurantDto = new RestaurantDto
            {
                Id = r.Id,
                NameUA = r.NameUA,
                NameEN = r.NameEN,
                Address = r.Address,
                ImageUrl = r.ImageUrl,
                Latitude = r.Latitude,
                Longitude = r.Longitude,
                IsActive = r.IsActive,
                OwnerId = r.OwnerId,
                Owner = r.Owner == null ? null : new OwnerDto
                {
                    Id = r.Owner.Id,
                    FullName = r.Owner.FullName,
                    Email = r.Owner.Email
                }
            };

            return Ok(restaurantDto);
        }

        /// <summary>
        /// Р РµС”СЃС‚СЂСѓС” РЅРѕРІРёР№ СЂРµСЃС‚РѕСЂР°РЅ Сѓ СЃРёСЃС‚РµРјС–.
        /// </summary>
        /// <remarks>
        /// Р”РѕСЃС‚СѓРїРЅРѕ РґР»СЏ Admin С‚Р° RestaurantOwner.
        /// - РЇРєС‰Рѕ СЃС‚РІРѕСЂСЋС” **RestaurantOwner**: РІС–РЅ Р°РІС‚РѕРјР°С‚РёС‡РЅРѕ РїСЂРёРІ'СЏР·СѓС”С‚СЊСЃСЏ СЏРє РІР»Р°СЃРЅРёРє.
        /// - РЇРєС‰Рѕ СЃС‚РІРѕСЂСЋС” **Admin**: РІС–РЅ РјРѕР¶Рµ РІРєР°Р·Р°С‚Рё ID Р±СѓРґСЊ-СЏРєРѕРіРѕ РєРѕСЂРёСЃС‚СѓРІР°С‡Р° СЏРє РІР»Р°СЃРЅРёРєР° (С‡РµСЂРµР· `createDto.OwnerId`).
        /// </remarks>
        /// <param name="createDto">Р”Р°РЅС– РґР»СЏ СЃС‚РІРѕСЂРµРЅРЅСЏ (РќР°Р·РІР°, РђРґСЂРµСЃР°, РљРѕРѕСЂРґРёРЅР°С‚Рё).</param>
        /// <returns>РЎС‚РІРѕСЂРµРЅРёР№ СЂРµСЃС‚РѕСЂР°РЅ.</returns>
        /// <response code="201">Р РµСЃС‚РѕСЂР°РЅ СѓСЃРїС–С€РЅРѕ СЃС‚РІРѕСЂРµРЅРѕ.</response>
        /// <response code="400">Р’РєР°Р·Р°РЅРѕРіРѕ РІР»Р°СЃРЅРёРєР° РЅРµ С–СЃРЅСѓС” (РґР»СЏ РђРґРјС–РЅР°).</response>
        /// <response code="401">РљРѕСЂРёСЃС‚СѓРІР°С‡ РЅРµ Р°РІС‚РѕСЂРёР·РѕРІР°РЅРёР№.</response>
        [HttpPost]
        [Authorize(Roles = "Admin,RestaurantOwner")]
        [ProducesResponseType(typeof(RestaurantDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<RestaurantDto>> CreateRestaurant(CreateRestaurantDto createDto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var userRoleStr = User.FindFirstValue(ClaimTypes.Role);

            int realOwnerId;

            if (userRoleStr == UserRole.RestaurantOwner.ToString())
            {
                realOwnerId = userId;
            }
            else
            {
                realOwnerId = createDto.OwnerId;
            }

            var owner = await _context.Users.FindAsync(realOwnerId);
            if (owner == null) return BadRequest($"РљРѕСЂРёСЃС‚СѓРІР°С‡Р° Р· ID {realOwnerId} РЅРµ С–СЃРЅСѓС”.");

            var restaurant = new Restaurant
            {
                NameUA = createDto.NameUA,
                NameEN = createDto.NameEN,
                Address = createDto.Address,
                ImageUrl = createDto.ImageUrl,
                Latitude = createDto.Latitude,
                Longitude = createDto.Longitude,
                OwnerId = realOwnerId,
                IsActive = true
            };

            _context.Restaurants.Add(restaurant);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetRestaurant), new { id = restaurant.Id }, new RestaurantDto { Id = restaurant.Id, NameUA = restaurant.NameUA });
        }

        /// <summary>
        /// Р’РёРґР°Р»СЏС” СЂРµСЃС‚РѕСЂР°РЅ С–Р· СЃРёСЃС‚РµРјРё.
        /// </summary>
        /// <remarks>
        /// - **Admin**: РњРѕР¶Рµ РІРёРґР°Р»РёС‚Рё Р±СѓРґСЊ-СЏРєРёР№ СЂРµСЃС‚РѕСЂР°РЅ.
        /// - **RestaurantOwner**: РњРѕР¶Рµ РІРёРґР°Р»РёС‚Рё С‚С–Р»СЊРєРё РІР»Р°СЃРЅРёР№ СЂРµСЃС‚РѕСЂР°РЅ.
        /// </remarks>
        /// <param name="id">ID СЂРµСЃС‚РѕСЂР°РЅСѓ.</param>
        /// <returns>Р РµР·СѓР»СЊС‚Р°С‚ РѕРїРµСЂР°С†С–С—.</returns>
        /// <response code="200">Р РµСЃС‚РѕСЂР°РЅ РІРёРґР°Р»РµРЅРѕ.</response>
        /// <response code="401">РљРѕСЂРёСЃС‚СѓРІР°С‡ РЅРµ Р°РІС‚РѕСЂРёР·РѕРІР°РЅРёР№.</response>
        /// <response code="403">РЎРїСЂРѕР±Р° РІРёРґР°Р»РёС‚Рё С‡СѓР¶РёР№ СЂРµСЃС‚РѕСЂР°РЅ.</response>
        /// <response code="404">Р РµСЃС‚РѕСЂР°РЅ РЅРµ Р·РЅР°Р№РґРµРЅРѕ.</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,RestaurantOwner")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteRestaurant(int id)
        {
            var restaurant = await _context.Restaurants.FindAsync(id);
            if (restaurant == null) return NotFound("Р—Р°РєР»Р°Рґ РЅРµ Р·РЅР°Р№РґРµРЅРѕ");

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var userRoleStr = User.FindFirstValue(ClaimTypes.Role);

            if (userRoleStr == UserRole.RestaurantOwner.ToString())
            {
                if (restaurant.OwnerId != userId)
                {
                    return StatusCode(403, "Р’Рё РЅРµ РјР°С”С‚Рµ РїСЂР°РІР° РІРёРґР°Р»СЏС‚Рё С‡СѓР¶РёР№ СЂРµСЃС‚РѕСЂР°РЅ!");
                }
            }

            _context.Restaurants.Remove(restaurant);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Р РµСЃС‚РѕСЂР°РЅ СѓСЃРїС–С€РЅРѕ РІРёРґР°Р»РµРЅРѕ." });
        }
    }
}
