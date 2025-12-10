using FoodPreOrder.Application.DTOs.Restaurants;
using FoodPreOrder.Domain.Entities;
using FoodPreOrder.Domain.Enums;
using FoodPreOrder.Persistence.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;
using System.Security.Claims;

namespace FoodPreOrder.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize]
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

        [HttpPost]
        [Authorize(Roles = "Admin,KitchenStaff")]
        public async Task<ActionResult<CategoryDto>> CreateCategory(CreateCategoryDto createDto)
        {
            var restaurant = await _context.Restaurants.FindAsync(createDto.RestaurantId);

            if (restaurant == null)
            {
                return BadRequest($"Ресторан з ID {createDto.RestaurantId} не знайдено.");
            }

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            if (userRole == UserRole.Admin.ToString())
            {
                if (restaurant.OwnerId != userId)
                {
                    return StatusCode(403, "Ви не можете створювати категорії в чужому ресторані!");
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

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,KitchenStaff")]
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

            if (userRole == UserRole.Admin.ToString())
            {
                if (category.Restaurant.OwnerId != userId)
                {
                    return StatusCode(403, "Ви не можете видаляти категорії чужого ресторану!");
                }
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
