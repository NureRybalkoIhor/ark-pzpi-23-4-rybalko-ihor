using FoodPreOrder.Application.DTOs.Reviews;
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
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ReviewsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("dish/{dishId}")]
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
                UserName = r.User?.FullName ?? "Анонім",
                CreatedAt = r.CreatedAt.ToUkraineTime()
            });

            return Ok(dtos);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ReviewDto>> CreateReview(CreateReviewDto createDto)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdStr == null) return Unauthorized();
            int userId = int.Parse(userIdStr);

            var dish = await _context.Dishes.FindAsync(createDto.DishId);
            if (dish == null) return BadRequest("Страву не знайдено");

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
                UserName = user?.FullName ?? "Я",
                CreatedAt = review.CreatedAt
            });
        }
    }
}
