п»їusing FoodPreOrder.Application.DTOs.Restaurants;
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
    /// РљРѕРЅС‚СЂРѕР»РµСЂ РґР»СЏ СѓРїСЂР°РІР»С–РЅРЅСЏ РєР°С‚РµРіРѕСЂС–СЏРјРё СЃС‚СЂР°РІ (РјРµРЅСЋ).
    /// Р”РѕР·РІРѕР»СЏС” РїРµСЂРµРіР»СЏРґР°С‚Рё, СЃС‚РІРѕСЂСЋРІР°С‚Рё С‚Р° РІРёРґР°Р»СЏС‚Рё РєР°С‚РµРіРѕСЂС–С—.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// РћС‚СЂРёРјСѓС” СЃРїРёСЃРѕРє РєР°С‚РµРіРѕСЂС–Р№.
        /// </summary>
        /// <remarks>
        /// РњРѕР¶РЅР° РѕС‚СЂРёРјР°С‚Рё РІСЃС– РєР°С‚РµРіРѕСЂС–С— СЃРёСЃС‚РµРјРё Р°Р±Рѕ РІС–РґС„С–Р»СЊС‚СЂСѓРІР°С‚Рё С—С… Р·Р° ID СЂРµСЃС‚РѕСЂР°РЅСѓ.
        /// </remarks>
        /// <param name="restaurantId">РќРµРѕР±РѕРІ'СЏР·РєРѕРІРёР№ ID СЂРµСЃС‚РѕСЂР°РЅСѓ РґР»СЏ С„С–Р»СЊС‚СЂР°С†С–С—.</param>
        /// <returns>РЎРїРёСЃРѕРє РєР°С‚РµРіРѕСЂС–Р№ (DTO).</returns>
        /// <response code="200">РЈСЃРїС–С€РЅРµ РѕС‚СЂРёРјР°РЅРЅСЏ СЃРїРёСЃРєСѓ.</response>
        /// <response code="401">РљРѕСЂРёСЃС‚СѓРІР°С‡ РЅРµ Р°РІС‚РѕСЂРёР·РѕРІР°РЅРёР№.</response>
        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(IEnumerable<CategoryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories([FromQuery] int? restaurantId)
        {
            var query = _context.Categories.AsQueryable();

            if (restaurantId.HasValue)
            {
                query = query.Where(c => c.RestaurantId == restaurantId.Value);
            }

            var categories = await query.ToListAsync();

            var dtos = categories.Select(c => new CategoryDto
            {
                Id = c.Id,
                NameUA = c.NameUA,
                NameEN = c.NameEN,
                RestaurantId = c.RestaurantId
            });

            return Ok(dtos);
        }

        /// <summary>
        /// РЎС‚РІРѕСЂСЋС” РЅРѕРІСѓ РєР°С‚РµРіРѕСЂС–СЋ РІ РјРµРЅСЋ СЂРµСЃС‚РѕСЂР°РЅСѓ.
        /// </summary>
        /// <remarks>
        /// Р”РѕСЃС‚СѓРїРЅРѕ РґР»СЏ СЂРѕР»РµР№ Admin, RestaurantOwner С‚Р° KitchenStaff.
        /// Р’Р»Р°СЃРЅРёРє СЂРµСЃС‚РѕСЂР°РЅСѓ РјРѕР¶Рµ СЃС‚РІРѕСЂСЋРІР°С‚Рё РєР°С‚РµРіРѕСЂС–С— Р»РёС€Рµ Сѓ СЃРІРѕС”РјСѓ Р·Р°РєР»Р°РґС–.
        /// </remarks>
        /// <param name="createDto">Р”Р°РЅС– РґР»СЏ СЃС‚РІРѕСЂРµРЅРЅСЏ РєР°С‚РµРіРѕСЂС–С— (РќР°Р·РІРё С‚Р° ID СЂРµСЃС‚РѕСЂР°РЅСѓ).</param>
        /// <returns>РЎС‚РІРѕСЂРµРЅР° РєР°С‚РµРіРѕСЂС–СЏ.</returns>
        /// <response code="201">РљР°С‚РµРіРѕСЂС–СЋ СѓСЃРїС–С€РЅРѕ СЃС‚РІРѕСЂРµРЅРѕ.</response>
        /// <response code="400">Р РµСЃС‚РѕСЂР°РЅ РЅРµ Р·РЅР°Р№РґРµРЅРѕ Р°Р±Рѕ РЅРµРєРѕСЂРµРєС‚РЅС– РґР°РЅС–.</response>
        /// <response code="401">РљРѕСЂРёСЃС‚СѓРІР°С‡ РЅРµ Р°РІС‚РѕСЂРёР·РѕРІР°РЅРёР№.</response>
        /// <response code="403">РЎРїСЂРѕР±Р° СЃС‚РІРѕСЂРёС‚Рё РєР°С‚РµРіРѕСЂС–СЋ РІ С‡СѓР¶РѕРјСѓ СЂРµСЃС‚РѕСЂР°РЅС–.</response>
        [HttpPost]
        [Authorize(Roles = "Admin,RestaurantOwner,KitchenStaff")]
        [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<CategoryDto>> CreateCategory(CreateCategoryDto createDto)
        {
            var restaurant = await _context.Restaurants.FindAsync(createDto.RestaurantId);

            if (restaurant == null)
            {
                return BadRequest($"Р РµСЃС‚РѕСЂР°РЅ Р· ID {createDto.RestaurantId} РЅРµ Р·РЅР°Р№РґРµРЅРѕ.");
            }

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            if (userRole == UserRole.RestaurantOwner.ToString())
            {
                if (restaurant.OwnerId != userId)
                {
                    return StatusCode(403, "Р’Рё РЅРµ РјРѕР¶РµС‚Рµ СЃС‚РІРѕСЂСЋРІР°С‚Рё РєР°С‚РµРіРѕСЂС–С— РІ С‡СѓР¶РѕРјСѓ СЂРµСЃС‚РѕСЂР°РЅС–!");
                }
            }

            var category = new Category
            {
                NameUA = createDto.NameUA,
                NameEN = createDto.NameEN,
                RestaurantId = createDto.RestaurantId
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            var responseDto = new CategoryDto
            {
                Id = category.Id,
                NameUA = category.NameUA,
                NameEN = category.NameEN,
                RestaurantId = category.RestaurantId
            };

            return CreatedAtAction(nameof(GetCategories), new { id = category.Id }, responseDto);
        }

        /// <summary>
        /// Р’РёРґР°Р»СЏС” С–СЃРЅСѓСЋС‡Сѓ РєР°С‚РµРіРѕСЂС–СЋ.
        /// </summary>
        /// <remarks>
        /// Р”РѕСЃС‚СѓРїРЅРѕ РґР»СЏ СЂРѕР»РµР№ Admin, RestaurantOwner С‚Р° KitchenStaff.
        /// Р’Р»Р°СЃРЅРёРє СЂРµСЃС‚РѕСЂР°РЅСѓ РјРѕР¶Рµ РІРёРґР°Р»СЏС‚Рё РєР°С‚РµРіРѕСЂС–С— Р»РёС€Рµ СЃРІРѕРіРѕ СЂРµСЃС‚РѕСЂР°РЅСѓ.
        /// </remarks>
        /// <param name="id">ID РєР°С‚РµРіРѕСЂС–С—, СЏРєСѓ С‚СЂРµР±Р° РІРёРґР°Р»РёС‚Рё.</param>
        /// <returns>РЎС‚Р°С‚СѓСЃ 204 No Content Сѓ СЂР°Р·С– СѓСЃРїС–С…Сѓ.</returns>
        /// <response code="204">РЈСЃРїС–С€РЅРµ РІРёРґР°Р»РµРЅРЅСЏ.</response>
        /// <response code="401">РљРѕСЂРёСЃС‚СѓРІР°С‡ РЅРµ Р°РІС‚РѕСЂРёР·РѕРІР°РЅРёР№.</response>
        /// <response code="403">РЎРїСЂРѕР±Р° РІРёРґР°Р»РёС‚Рё РєР°С‚РµРіРѕСЂС–СЋ С‡СѓР¶РѕРіРѕ СЂРµСЃС‚РѕСЂР°РЅСѓ.</response>
        /// <response code="404">РљР°С‚РµРіРѕСЂС–СЋ РЅРµ Р·РЅР°Р№РґРµРЅРѕ.</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,RestaurantOwner,KitchenStaff")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories
                .Include(c => c.Restaurant)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                return NotFound();
            }

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            if (userRole == UserRole.RestaurantOwner.ToString())
            {
                if (category.Restaurant.OwnerId != userId)
                {
                    return StatusCode(403, "Р’Рё РЅРµ РјРѕР¶РµС‚Рµ РІРёРґР°Р»СЏС‚Рё РєР°С‚РµРіРѕСЂС–С— С‡СѓР¶РѕРіРѕ СЂРµСЃС‚РѕСЂР°РЅСѓ!");
                }
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
