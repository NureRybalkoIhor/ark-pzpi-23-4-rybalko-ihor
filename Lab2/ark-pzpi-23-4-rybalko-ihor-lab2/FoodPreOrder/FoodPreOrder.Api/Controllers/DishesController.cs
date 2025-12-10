using Microsoft.AspNetCore.Http;
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


        [HttpGet]
        [Authorize]
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
                CategoryNameUA = d.Category?.NameUA ?? "Невідома категорія"
            });

            return Ok(dtos);
        }


        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<DishDto>> GetDish(int id)
        {
            var d = await _context.Dishes
                .Include(x => x.Category)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (d == null)
            {
                return NotFound("Страву не знайдено");
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
                CategoryNameUA = d.Category?.NameUA ?? "Невідома категорія"
            };

            return Ok(dto);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,KitchenStaff")]
        public async Task<ActionResult<DishDto>> CreateDish([FromForm] CreateDishDto createDto)
        {
            var category = await _context.Categories
                .Include(c => c.Restaurant)
                .FirstOrDefaultAsync(c => c.Id == createDto.CategoryId);

            if (category == null) return BadRequest("Категорії не існує.");

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            if (userRole == UserRole.Admin.ToString())
            {
                if (category.Restaurant.OwnerId != userId)
                {
                    return StatusCode(403, "Ви не можете додавати страви в чужий ресторан!");
                }
            }

            string? imagePath = null;
            if (createDto.Image != null)
            {
                imagePath = await _fileService.SaveFileAsync(createDto.Image);
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

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,KitchenStaff")]
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

            if (userRole == UserRole.Admin.ToString())
            {
                if (dish.Category?.Restaurant?.OwnerId != userId)
                {
                    return StatusCode(403, "Ви не можете видаляти страви з чужого ресторану!");
                }
            }

            _context.Dishes.Remove(dish);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
