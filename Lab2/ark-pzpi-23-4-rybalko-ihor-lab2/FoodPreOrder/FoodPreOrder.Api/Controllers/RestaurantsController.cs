using FoodPreOrder.Api.Services;
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
        [HttpGet]
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

        [HttpGet("{id}")]
        public async Task<ActionResult<RestaurantDto>> GetRestaurant(int id)
        {
            var r = await _context.Restaurants
                .Include(rest => rest.Owner)
                .FirstOrDefaultAsync(rest => rest.Id == id);

            if (r == null)
            {
                return NotFound("Заклад не знайдено");
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

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<RestaurantDto>> CreateRestaurant(CreateRestaurantDto createDto)
        {
            var owner = await _context.Users.FindAsync(createDto.OwnerId);
            if (owner == null)
            {
                return BadRequest($"Користувача з ID {createDto.OwnerId} не існує.");
            }

            var restaurant = new Restaurant
            {
                NameUA = createDto.NameUA,
                NameEN = createDto.NameEN,
                Address = createDto.Address,
                ImageUrl = createDto.ImageUrl,
                Latitude = createDto.Latitude,
                Longitude = createDto.Longitude,
                OwnerId = createDto.OwnerId,
                IsActive = true
            };

            _context.Restaurants.Add(restaurant);
            await _context.SaveChangesAsync();

            var responseDto = new RestaurantDto
            {
                Id = restaurant.Id,
                NameUA = restaurant.NameUA,
                NameEN = restaurant.NameEN,
                Address = restaurant.Address,
                ImageUrl = restaurant.ImageUrl,
                Latitude = restaurant.Latitude,
                Longitude = restaurant.Longitude,
                IsActive = restaurant.IsActive,
                OwnerId = restaurant.OwnerId,
                Owner = new OwnerDto
                {
                    Id = owner.Id,
                    FullName = owner.FullName,
                    Email = owner.Email
                }
            };

            return CreatedAtAction(nameof(GetRestaurant), new { id = restaurant.Id }, responseDto);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteRestaurant(int id)
        {
            var restaurant = await _context.Restaurants.FindAsync(id);

            if (restaurant == null)
            {
                return NotFound("Заклад не знайдено");
            }

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            if (userRole == UserRole.Admin.ToString())
            {
                if (restaurant.OwnerId != userId)
                {
                    return StatusCode(403, "Ви не маєте права видаляти (деактивувати) чужий ресторан!");
                }
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "Ресторан успішно деактивовано (архівовано)." });
        }
    }
}
