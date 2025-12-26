п»їusing Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using FoodPreOrder.Domain.Entities;
using FoodPreOrder.Persistence.Data;
using FoodPreOrder.Application.DTOs.Restaurants;
using FoodPreOrder.Api.Services;
using Microsoft.AspNetCore.Authorization;
using FoodPreOrder.Domain.Enums;
using System.Security.Claims;

namespace FoodPreOrder.Api.Controllers
{
    /// <summary>
    /// РљРѕРЅС‚СЂРѕР»РµСЂ РґР»СЏ СѓРїСЂР°РІР»С–РЅРЅСЏ СЃС‚СЂР°РІР°РјРё (РїРѕР·РёС†С–СЏРјРё РјРµРЅСЋ).
    /// Р—Р°Р±РµР·РїРµС‡СѓС” РґРѕРґР°РІР°РЅРЅСЏ, РїРµСЂРµРіР»СЏРґ С‚Р° РІРёРґР°Р»РµРЅРЅСЏ СЃС‚СЂР°РІ, РІРєР»СЋС‡Р°СЋС‡Рё Р·Р°РІР°РЅС‚Р°Р¶РµРЅРЅСЏ Р·РѕР±СЂР°Р¶РµРЅСЊ.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class DishesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IFileStorageService _fileService;

        public DishesController(ApplicationDbContext context, IFileStorageService fileService)
        {
            _context = context;
            _fileService = fileService;
        }

        /// <summary>
        /// РћС‚СЂРёРјСѓС” СЃРїРёСЃРѕРє СЃС‚СЂР°РІ.
        /// </summary>
        /// <remarks>
        /// Р”РѕР·РІРѕР»СЏС” РѕС‚СЂРёРјР°С‚Рё РІСЃС– СЃС‚СЂР°РІРё Р°Р±Рѕ РІС–РґС„С–Р»СЊС‚СЂСѓРІР°С‚Рё С—С… Р·Р° РєР°С‚РµРіРѕСЂС–С”СЋ.
        /// </remarks>
        /// <param name="categoryId">РќРµРѕР±РѕРІ'СЏР·РєРѕРІРёР№ ID РєР°С‚РµРіРѕСЂС–С— РґР»СЏ С„С–Р»СЊС‚СЂР°С†С–С— СЃС‚СЂР°РІ РєРѕРЅРєСЂРµС‚РЅРѕРіРѕ СЂРѕР·РґС–Р»Сѓ РјРµРЅСЋ.</param>
        /// <returns>РЎРїРёСЃРѕРє СЃС‚СЂР°РІ (DTO).</returns>
        /// <response code="200">РЈСЃРїС–С€РЅРµ РѕС‚СЂРёРјР°РЅРЅСЏ СЃРїРёСЃРєСѓ.</response>
        /// <response code="401">РљРѕСЂРёСЃС‚СѓРІР°С‡ РЅРµ Р°РІС‚РѕСЂРёР·РѕРІР°РЅРёР№.</response>
        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(IEnumerable<DishDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<DishDto>>> GetDishes([FromQuery] int? categoryId)
        {
            var query = _context.Dishes
                .Include(d => d.Category)
                .AsQueryable();

            if (categoryId.HasValue)
            {
                query = query.Where(d => d.CategoryId == categoryId.Value);
            }

            var dishes = await query.ToListAsync();

            var dtos = dishes.Select(d => new DishDto
            {
                Id = d.Id,
                NameUA = d.NameUA,
                NameEN = d.NameEN,
                DescriptionUA = d.DescriptionUA,
                DescriptionEN = d.DescriptionEN,
                Price = d.Price,
                ImageUrl = d.ImageUrl,
                PreparationTimeMinutes = d.PreparationTimeMinutes,
                IsAvailable = d.IsAvailable,
                CategoryId = d.CategoryId,
                CategoryNameUA = d.Category?.NameUA ?? "РќРµРІС–РґРѕРјР° РєР°С‚РµРіРѕСЂС–СЏ"
            });

            return Ok(dtos);
        }

        /// <summary>
        /// РћС‚СЂРёРјСѓС” РґРµС‚Р°Р»СЊРЅСѓ С–РЅС„РѕСЂРјР°С†С–СЋ РїСЂРѕ РєРѕРЅРєСЂРµС‚РЅСѓ СЃС‚СЂР°РІСѓ Р·Р° С—С— ID.
        /// </summary>
        /// <param name="id">РЈРЅС–РєР°Р»СЊРЅРёР№ С–РґРµРЅС‚РёС„С–РєР°С‚РѕСЂ СЃС‚СЂР°РІРё.</param>
        /// <returns>DTO СЃС‚СЂР°РІРё.</returns>
        /// <response code="200">РЎС‚СЂР°РІСѓ Р·РЅР°Р№РґРµРЅРѕ.</response>
        /// <response code="401">РљРѕСЂРёСЃС‚СѓРІР°С‡ РЅРµ Р°РІС‚РѕСЂРёР·РѕРІР°РЅРёР№.</response>
        /// <response code="404">РЎС‚СЂР°РІСѓ РЅРµ Р·РЅР°Р№РґРµРЅРѕ.</response>
        [HttpGet("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(DishDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DishDto>> GetDish(int id)
        {
            var d = await _context.Dishes
                .Include(x => x.Category)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (d == null)
            {
                return NotFound("РЎС‚СЂР°РІСѓ РЅРµ Р·РЅР°Р№РґРµРЅРѕ");
            }

            var dto = new DishDto
            {
                Id = d.Id,
                NameUA = d.NameUA,
                NameEN = d.NameEN,
                DescriptionUA = d.DescriptionUA,
                DescriptionEN = d.DescriptionEN,
                Price = d.Price,
                ImageUrl = d.ImageUrl,
                PreparationTimeMinutes = d.PreparationTimeMinutes,
                IsAvailable = d.IsAvailable,
                CategoryId = d.CategoryId,
                CategoryNameUA = d.Category?.NameUA ?? "РќРµРІС–РґРѕРјР° РєР°С‚РµРіРѕСЂС–СЏ"
            };

            return Ok(dto);
        }

        /// <summary>
        /// РЎС‚РІРѕСЂСЋС” РЅРѕРІСѓ СЃС‚СЂР°РІСѓ.
        /// </summary>
        /// <remarks>
        /// РџСЂРёР№РјР°С” РґР°РЅС– Сѓ С„РѕСЂРјР°С‚С– `multipart/form-data`, С‰Рѕ РґРѕР·РІРѕР»СЏС” Р·Р°РІР°РЅС‚Р°Р¶СѓРІР°С‚Рё Р·РѕР±СЂР°Р¶РµРЅРЅСЏ.
        /// Р’Р»Р°СЃРЅРёРє СЂРµСЃС‚РѕСЂР°РЅСѓ РјРѕР¶Рµ РґРѕРґР°РІР°С‚Рё СЃС‚СЂР°РІРё Р»РёС€Рµ Сѓ СЃРІРѕС— РєР°С‚РµРіРѕСЂС–С—.
        /// </remarks>
        /// <param name="createDto">Р”Р°РЅС– РґР»СЏ СЃС‚РІРѕСЂРµРЅРЅСЏ СЃС‚СЂР°РІРё (РќР°Р·РІР°, Р¦С–РЅР°, РћРїРёСЃ, Р—РѕР±СЂР°Р¶РµРЅРЅСЏ).</param>
        /// <returns>РЎС‚РІРѕСЂРµРЅР° СЃС‚СЂР°РІР°.</returns>
        /// <response code="201">РЎС‚СЂР°РІСѓ СѓСЃРїС–С€РЅРѕ СЃС‚РІРѕСЂРµРЅРѕ.</response>
        /// <response code="400">РљР°С‚РµРіРѕСЂС–С— РЅРµ С–СЃРЅСѓС” Р°Р±Рѕ РґР°РЅС– РЅРµРєРѕСЂРµРєС‚РЅС–.</response>
        /// <response code="401">РљРѕСЂРёСЃС‚СѓРІР°С‡ РЅРµ Р°РІС‚РѕСЂРёР·РѕРІР°РЅРёР№.</response>
        /// <response code="403">РЎРїСЂРѕР±Р° РґРѕРґР°С‚Рё СЃС‚СЂР°РІСѓ РІ С‡СѓР¶РёР№ СЂРµСЃС‚РѕСЂР°РЅ.</response>
        [HttpPost]
        [Authorize(Roles = "Admin,RestaurantOwner,KitchenStaff")]
        [ProducesResponseType(typeof(DishDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<DishDto>> CreateDish([FromForm] CreateDishDto createDto)
        {
            var category = await _context.Categories
                .Include(c => c.Restaurant)
                .FirstOrDefaultAsync(c => c.Id == createDto.CategoryId);

            if (category == null) return BadRequest("РљР°С‚РµРіРѕСЂС–С— РЅРµ С–СЃРЅСѓС”.");

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var userRoleStr = User.FindFirstValue(ClaimTypes.Role);

            if (userRoleStr == UserRole.RestaurantOwner.ToString())
            {
                if (category.Restaurant.OwnerId != userId)
                {
                    return StatusCode(403, "Р’Рё РЅРµ РјРѕР¶РµС‚Рµ РґРѕРґР°РІР°С‚Рё СЃС‚СЂР°РІРё РІ С‡СѓР¶РёР№ СЂРµСЃС‚РѕСЂР°РЅ!");
                }
            }

            string? imagePath = null;
            if (createDto.Image != null)
            {
                imagePath = await _fileService.SaveFileAsync(createDto.Image, "dishes");
            }

            var dish = new Dish
            {
                NameUA = createDto.NameUA,
                NameEN = createDto.NameEN,
                DescriptionUA = createDto.DescriptionUA,
                DescriptionEN = createDto.DescriptionEN,
                Price = createDto.Price,
                ImageUrl = imagePath,
                PreparationTimeMinutes = createDto.PreparationTimeMinutes,
                CategoryId = createDto.CategoryId,
                IsAvailable = true
            };

            _context.Dishes.Add(dish);
            await _context.SaveChangesAsync();

            var responseDto = new DishDto
            {
                Id = dish.Id,
                NameUA = dish.NameUA,
                NameEN = dish.NameEN,
                DescriptionUA = dish.DescriptionUA,
                DescriptionEN = dish.DescriptionEN,
                Price = dish.Price,
                ImageUrl = dish.ImageUrl,
                PreparationTimeMinutes = dish.PreparationTimeMinutes,
                IsAvailable = dish.IsAvailable,
                CategoryId = dish.CategoryId,
                CategoryNameUA = category.NameUA
            };

            return CreatedAtAction(nameof(GetDish), new { id = dish.Id }, responseDto);
        }

        /// <summary>
        /// Р’РёРґР°Р»СЏС” СЃС‚СЂР°РІСѓ Р· РјРµРЅСЋ.
        /// </summary>
        /// <remarks>
        /// Р’Р»Р°СЃРЅРёРє СЂРµСЃС‚РѕСЂР°РЅСѓ РјРѕР¶Рµ РІРёРґР°Р»СЏС‚Рё Р»РёС€Рµ СЃС‚СЂР°РІРё СЃРІРѕРіРѕ Р·Р°РєР»Р°РґСѓ.
        /// </remarks>
        /// <param name="id">ID СЃС‚СЂР°РІРё.</param>
        /// <returns>РЎС‚Р°С‚СѓСЃ 204 No Content.</returns>
        /// <response code="204">РЎС‚СЂР°РІСѓ СѓСЃРїС–С€РЅРѕ РІРёРґР°Р»РµРЅРѕ.</response>
        /// <response code="401">РљРѕСЂРёСЃС‚СѓРІР°С‡ РЅРµ Р°РІС‚РѕСЂРёР·РѕРІР°РЅРёР№.</response>
        /// <response code="403">РЎРїСЂРѕР±Р° РІРёРґР°Р»РёС‚Рё СЃС‚СЂР°РІСѓ Р· С‡СѓР¶РѕРіРѕ СЂРµСЃС‚РѕСЂР°РЅСѓ.</response>
        /// <response code="404">РЎС‚СЂР°РІСѓ РЅРµ Р·РЅР°Р№РґРµРЅРѕ.</response>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,KitchenStaff,RestaurantOwner")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteDish(int id)
        {
            var dish = await _context.Dishes
                .Include(d => d.Category)
                .ThenInclude(c => c.Restaurant)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (dish == null)
            {
                return NotFound();
            }

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            if (userRole == UserRole.RestaurantOwner.ToString())
            {
                if (dish.Category?.Restaurant?.OwnerId != userId)
                {
                    return StatusCode(403, "Р’Рё РЅРµ РјРѕР¶РµС‚Рµ РІРёРґР°Р»СЏС‚Рё СЃС‚СЂР°РІРё Р· С‡СѓР¶РѕРіРѕ СЂРµСЃС‚РѕСЂР°РЅСѓ!");
                }
            }

            _context.Dishes.Remove(dish);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
