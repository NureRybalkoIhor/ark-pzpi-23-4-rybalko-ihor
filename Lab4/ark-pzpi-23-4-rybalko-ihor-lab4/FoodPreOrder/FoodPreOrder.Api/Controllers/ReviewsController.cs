п»їusing FoodPreOrder.Application.DTOs.Reviews;
using FoodPreOrder.Domain.Entities;
using FoodPreOrder.Persistence.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using FoodPreOrder.Api.Extensions;

namespace FoodPreOrder.Api.Controllers
{
    /// <summary>
    /// РљРѕРЅС‚СЂРѕР»РµСЂ РґР»СЏ СѓРїСЂР°РІР»С–РЅРЅСЏ РІС–РґРіСѓРєР°РјРё С‚Р° СЂРµР№С‚РёРЅРіР°РјРё СЃС‚СЂР°РІ.
    /// Р”РѕР·РІРѕР»СЏС” РєРѕСЂРёСЃС‚СѓРІР°С‡Р°Рј РїРµСЂРµРіР»СЏРґР°С‚Рё РґСѓРјРєРё С–РЅС€РёС… С‚Р° Р·Р°Р»РёС€Р°С‚Рё РІР»Р°СЃРЅС– РєРѕРјРµРЅС‚Р°СЂС–.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ReviewsController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// РћС‚СЂРёРјСѓС” СЃРїРёСЃРѕРє РІС–РґРіСѓРєС–РІ РґР»СЏ РєРѕРЅРєСЂРµС‚РЅРѕС— СЃС‚СЂР°РІРё.
        /// </summary>
        /// <remarks>
        /// Р’С–РґРіСѓРєРё РІС–РґСЃРѕСЂС‚РѕРІР°РЅС– Р·Р° РґР°С‚РѕСЋ: РІС–Рґ РЅР°Р№РЅРѕРІС–С€РёС… РґРѕ РЅР°Р№СЃС‚Р°СЂС–С€РёС….
        /// Р¦РµР№ РјРµС‚РѕРґ РїСѓР±Р»С–С‡РЅРёР№ С– РЅРµ РІРёРјР°РіР°С” Р°РІС‚РѕСЂРёР·Р°С†С–С—.
        /// </remarks>
        /// <param name="dishId">ID СЃС‚СЂР°РІРё, РґР»СЏ СЏРєРѕС— РїРѕС‚СЂС–Р±РЅРѕ РѕС‚СЂРёРјР°С‚Рё РІС–РґРіСѓРєРё.</param>
        /// <returns>РЎРїРёСЃРѕРє DTO РІС–РґРіСѓРєС–РІ.</returns>
        /// <response code="200">РЈСЃРїС–С€РЅРµ РѕС‚СЂРёРјР°РЅРЅСЏ СЃРїРёСЃРєСѓ.</response>
        [HttpGet("dish/{dishId}")]
        [ProducesResponseType(typeof(IEnumerable<ReviewDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ReviewDto>>> GetDishReviews(int dishId)
        {
            var reviews = await _context.Reviews
                .Include(r => r.User)
                .Where(r => r.DishId == dishId)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            var dtos = reviews.Select(r => new ReviewDto
            {
                Id = r.Id,
                Rating = r.Rating,
                Comment = r.Comment,
                UserName = r.User?.FullName ?? "РђРЅРѕРЅС–Рј",
                CreatedAt = r.CreatedAt.ToUkraineTime()
            });

            return Ok(dtos);
        }

        /// <summary>
        /// Р”РѕРґР°С” РЅРѕРІРёР№ РІС–РґРіСѓРє РґРѕ СЃС‚СЂР°РІРё.
        /// </summary>
        /// <remarks>
        /// Р’РёРјР°РіР°С” Р°РІС‚РѕСЂРёР·Р°С†С–С— (JWT Token).
        /// РљРѕСЂРёСЃС‚СѓРІР°С‡ РїРµСЂРµРґР°С” РѕС†С–РЅРєСѓ (Rating) С‚Р° С‚РµРєСЃС‚РѕРІРёР№ РєРѕРјРµРЅС‚Р°СЂ.
        /// </remarks>
        /// <param name="createDto">Р”Р°РЅС– РґР»СЏ СЃС‚РІРѕСЂРµРЅРЅСЏ РІС–РґРіСѓРєСѓ (ID СЃС‚СЂР°РІРё, РѕС†С–РЅРєР°, РєРѕРјРµРЅС‚Р°СЂ).</param>
        /// <returns>РЎС‚РІРѕСЂРµРЅРёР№ РІС–РґРіСѓРє.</returns>
        /// <response code="200">Р’С–РґРіСѓРє СѓСЃРїС–С€РЅРѕ РґРѕРґР°РЅРѕ.</response>
        /// <response code="400">РЎС‚СЂР°РІСѓ РЅРµ Р·РЅР°Р№РґРµРЅРѕ.</response>
        /// <response code="401">РљРѕСЂРёСЃС‚СѓРІР°С‡ РЅРµ Р°РІС‚РѕСЂРёР·РѕРІР°РЅРёР№.</response>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(ReviewDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ReviewDto>> CreateReview(CreateReviewDto createDto)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdStr == null) return Unauthorized();
            int userId = int.Parse(userIdStr);

            var dish = await _context.Dishes.FindAsync(createDto.DishId);
            if (dish == null) return BadRequest("РЎС‚СЂР°РІСѓ РЅРµ Р·РЅР°Р№РґРµРЅРѕ");

            var review = new Review
            {
                DishId = createDto.DishId,
                UserId = userId,
                Rating = createDto.Rating,
                Comment = createDto.Comment,
                CreatedAt = DateTime.UtcNow
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            var user = await _context.Users.FindAsync(userId);

            return Ok(new ReviewDto
            {
                Id = review.Id,
                Rating = review.Rating,
                Comment = review.Comment,
                UserName = user?.FullName ?? "РЇ",
                CreatedAt = review.CreatedAt
            });
        }
    }
}
