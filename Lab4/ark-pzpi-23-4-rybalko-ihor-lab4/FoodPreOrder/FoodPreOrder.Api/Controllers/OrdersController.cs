п»їusing FoodPreOrder.Api.Extensions;
using FoodPreOrder.Api.Services;
using FoodPreOrder.Application.DTOs.Orders;
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
    /// РљРѕРЅС‚СЂРѕР»РµСЂ РґР»СЏ СѓРїСЂР°РІР»С–РЅРЅСЏ Р¶РёС‚С‚С”РІРёРј С†РёРєР»РѕРј Р·Р°РјРѕРІР»РµРЅСЊ.
    /// Р’С–РґРїРѕРІС–РґР°С” Р·Р° СЃС‚РІРѕСЂРµРЅРЅСЏ, РїРµСЂРµРіР»СЏРґ С–СЃС‚РѕСЂС–С— С‚Р° Р·РјС–РЅСѓ СЃС‚Р°С‚СѓСЃС–РІ Р·Р°РјРѕРІР»РµРЅСЊ (Processing workflow).
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ICalculationService _calcService;

        public OrdersController(ApplicationDbContext context, ICalculationService calcService)
        {
            _context = context;
            _calcService = calcService;
        }

        /// <summary>
        /// РћС‚СЂРёРјСѓС” СЃРїРёСЃРѕРє Р·Р°РјРѕРІР»РµРЅСЊ.
        /// </summary>
        /// <remarks>
        /// Р›РѕРіС–РєР° С„С–Р»СЊС‚СЂР°С†С–С— Р·Р°Р»РµР¶РёС‚СЊ РІС–Рґ СЂРѕР»С–:
        /// - **Customer**: Р‘Р°С‡РёС‚СЊ С‚С–Р»СЊРєРё РІР»Р°СЃРЅС– Р·Р°РјРѕРІР»РµРЅРЅСЏ.
        /// - **Admin/Staff/Owner**: Р‘Р°С‡Р°С‚СЊ Р·Р°РјРѕРІР»РµРЅРЅСЏ РІСЃС–С… РєРѕСЂРёСЃС‚СѓРІР°С‡С–РІ (РјРѕР¶РЅР° РґРѕРґР°С‚Рё С„С–Р»СЊС‚СЂР°С†С–СЋ РїРѕ СЂРµСЃС‚РѕСЂР°РЅСѓ РІ РјР°Р№Р±СѓС‚РЅСЊРѕРјСѓ).
        /// РЎРїРёСЃРѕРє РІС–РґСЃРѕСЂС‚РѕРІР°РЅРѕ Р·Р° РґР°С‚РѕСЋ СЃС‚РІРѕСЂРµРЅРЅСЏ (РЅР°Р№РЅРѕРІС–С€С– Р·РІРµСЂС…Сѓ).
        /// </remarks>
        /// <returns>РЎРїРёСЃРѕРє DTO Р·Р°РјРѕРІР»РµРЅСЊ.</returns>
        /// <response code="200">РЈСЃРїС–С€РЅРµ РѕС‚СЂРёРјР°РЅРЅСЏ СЃРїРёСЃРєСѓ.</response>
        /// <response code="401">РљРѕСЂРёСЃС‚СѓРІР°С‡ РЅРµ Р°РІС‚РѕСЂРёР·РѕРІР°РЅРёР№.</response>
        [HttpGet]
        [Authorize]
        [ProducesResponseType(typeof(IEnumerable<OrderDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrders()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            var query = _context.Orders
                .Include(o => o.Items).ThenInclude(i => i.Dish)
                .Include(o => o.User)
                .AsQueryable();

            if (userRole == UserRole.Customer.ToString())
            {
                query = query.Where(o => o.UserId == userId);
            }

            var orders = await query.OrderByDescending(o => o.CreatedAt).ToListAsync();
            return Ok(orders.Select(MapToDto));
        }

        /// <summary>
        /// РћС‚СЂРёРјСѓС” РґРµС‚Р°Р»С– РєРѕРЅРєСЂРµС‚РЅРѕРіРѕ Р·Р°РјРѕРІР»РµРЅРЅСЏ Р·Р° ID.
        /// </summary>
        /// <remarks>
        /// РљР»С–С”РЅС‚ РЅРµ РјРѕР¶Рµ РїРµСЂРµРіР»СЏРґР°С‚Рё С‡СѓР¶С– Р·Р°РјРѕРІР»РµРЅРЅСЏ (РїРѕРІРµСЂРЅРµС‚СЊСЃСЏ 403 Forbidden).
        /// </remarks>
        /// <param name="id">ID Р·Р°РјРѕРІР»РµРЅРЅСЏ.</param>
        /// <returns>Р”РµС‚Р°Р»С– Р·Р°РјРѕРІР»РµРЅРЅСЏ.</returns>
        /// <response code="200">Р—Р°РјРѕРІР»РµРЅРЅСЏ Р·РЅР°Р№РґРµРЅРѕ.</response>
        /// <response code="401">РљРѕСЂРёСЃС‚СѓРІР°С‡ РЅРµ Р°РІС‚РѕСЂРёР·РѕРІР°РЅРёР№.</response>
        /// <response code="403">РЎРїСЂРѕР±Р° РїРµСЂРµРіР»СЏРЅСѓС‚Рё С‡СѓР¶Рµ Р·Р°РјРѕРІР»РµРЅРЅСЏ.</response>
        /// <response code="404">Р—Р°РјРѕРІР»РµРЅРЅСЏ РЅРµ Р·РЅР°Р№РґРµРЅРѕ.</response>
        [HttpGet("{id}")]
        [Authorize]
        [ProducesResponseType(typeof(OrderDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<OrderDto>> GetOrder(int id)
        {
            var order = await _context.Orders
               .Include(o => o.Items).ThenInclude(i => i.Dish)
               .Include(o => o.User)
               .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return NotFound();

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            if (userRole == UserRole.Customer.ToString() && order.UserId != userId)
            {
                return Forbid();
            }

            return Ok(MapToDto(order));
        }

        /// <summary>
        /// РЎС‚РІРѕСЂСЋС” РЅРѕРІРµ Р·Р°РјРѕРІР»РµРЅРЅСЏ.
        /// </summary>
        /// <remarks>
        /// Р’РёРєРѕРЅСѓС” РЅРёР·РєСѓ РїРµСЂРµРІС–СЂРѕРє:
        /// 1. Р§Р°СЃ РІС–Р·РёС‚Сѓ РјР°С” Р±СѓС‚Рё РјС–РЅС–РјСѓРј С‡РµСЂРµР· 10 С…РІРёР»РёРЅ РІС–Рґ РїРѕС‚РѕС‡РЅРѕРіРѕ С‡Р°СЃСѓ.
        /// 2. РЎС‚СЂР°РІРё РїРѕРІРёРЅРЅС– С–СЃРЅСѓРІР°С‚Рё РІ Р±Р°Р·С–.
        /// 
        /// РўР°РєРѕР¶ Р°РІС‚РѕРјР°С‚РёС‡РЅРѕ СЂРѕР·СЂР°С…РѕРІСѓС” Р·Р°РіР°Р»СЊРЅСѓ РІР°СЂС‚С–СЃС‚СЊ (`TotalAmount`) С‚Р° РїСЂРѕРіРЅРѕР·РѕРІР°РЅРёР№ С‡Р°СЃ РіРѕС‚РѕРІРЅРѕСЃС‚С– (`EstimatedReadyTime`).
        /// </remarks>
        /// <param name="createDto">Р”Р°РЅС– Р·Р°РјРѕРІР»РµРЅРЅСЏ (Р§Р°СЃ РІС–Р·РёС‚Сѓ, РЎРїРёСЃРѕРє СЃС‚СЂР°РІ, РљРѕРјРµРЅС‚Р°СЂ).</param>
        /// <returns>РЎС‚РІРѕСЂРµРЅРµ Р·Р°РјРѕРІР»РµРЅРЅСЏ.</returns>
        /// <response code="201">Р—Р°РјРѕРІР»РµРЅРЅСЏ СѓСЃРїС–С€РЅРѕ СЃС‚РІРѕСЂРµРЅРѕ.</response>
        /// <response code="400">РџРѕРјРёР»РєР° РІР°Р»С–РґР°С†С–С— (РЅРµРєРѕСЂРµРєС‚РЅРёР№ С‡Р°СЃ, РЅРµС–СЃРЅСѓСЋС‡РёР№ СЂРµСЃС‚РѕСЂР°РЅ Р°Р±Рѕ СЃС‚СЂР°РІР°).</response>
        /// <response code="401">РљРѕСЂРёСЃС‚СѓРІР°С‡ РЅРµ Р°РІС‚РѕСЂРёР·РѕРІР°РЅРёР№.</response>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(OrderDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<OrderDto>> CreateOrder(CreateOrderDto createDto)
        {
            if (createDto.VisitTime < DateTime.UtcNow.AddMinutes(10))
            {
                return BadRequest("Р§Р°СЃ РІС–Р·РёС‚Сѓ РјР°С” Р±СѓС‚Рё РјС–РЅС–РјСѓРј С‡РµСЂРµР· 10 С…РІРёР»РёРЅ.");
            }

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var restaurant = await _context.Restaurants.FindAsync(createDto.RestaurantId);
            if (restaurant == null) return BadRequest("Р РµСЃС‚РѕСЂР°РЅ РЅРµ Р·РЅР°Р№РґРµРЅРѕ");

            var order = new Order
            {
                UserId = userId,
                RestaurantId = createDto.RestaurantId,
                VisitTime = createDto.VisitTime,
                Comment = createDto.Comment,
                Status = OrderStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            decimal totalAmount = 0;
            var dishEntities = new List<Dish>();

            foreach (var itemDto in createDto.Items)
            {
                var dish = await _context.Dishes.FindAsync(itemDto.DishId);
                if (dish == null) return BadRequest($"РЎС‚СЂР°РІСѓ {itemDto.DishId} РЅРµ Р·РЅР°Р№РґРµРЅРѕ");

                dishEntities.Add(dish);

                var orderItem = new OrderItem
                {
                    DishId = dish.Id,
                    Quantity = itemDto.Quantity,
                    Price = dish.Price
                };

                totalAmount += orderItem.Price * orderItem.Quantity;
                order.Items.Add(orderItem);
            }

            order.TotalAmount = totalAmount;

            order.EstimatedReadyTime = _calcService.CalculateEstimatedReadyTime(DateTime.UtcNow, dishEntities);

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            var createdOrder = await _context.Orders
                .Include(o => o.Items).ThenInclude(i => i.Dish)
                .Include(o => o.User)
                .FirstAsync(o => o.Id == order.Id);

            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, MapToDto(createdOrder));
        }

        /// <summary>
        /// РћРЅРѕРІР»СЋС” СЃС‚Р°С‚СѓСЃ Р·Р°РјРѕРІР»РµРЅРЅСЏ (Workflow).
        /// </summary>
        /// <remarks>
        /// Р”РѕСЃС‚СѓРїРЅРѕ РґР»СЏ Admin С‚Р° KitchenStaff.
        /// Р—РјС–РЅР° СЃС‚Р°С‚СѓСЃСѓ Р°РІС‚РѕРјР°С‚РёС‡РЅРѕ РіРµРЅРµСЂСѓС” СЃРїРѕРІС–С‰РµРЅРЅСЏ (Notification) РґР»СЏ РєР»С–С”РЅС‚Р°.
        /// 
        /// **Р›РѕРіС–РєР° СЃРїРѕРІС–С‰РµРЅСЊ:**
        /// - `Paid` -> "РћРїР»Р°С‚Сѓ РѕС‚СЂРёРјР°РЅРѕ"
        /// - `Cooking` -> "РљСѓС…РЅСЏ РїРѕС‡Р°Р»Р° РіРѕС‚СѓРІР°С‚Рё"
        /// - `Ready` -> "Р“РѕС‚РѕРІРѕ РґРѕ РІРёРґР°С‡С–"
        /// - `Completed` -> "Р”СЏРєСѓС”РјРѕ Р·Р° Р·Р°РјРѕРІР»РµРЅРЅСЏ"
        /// - `Cancelled` -> "Р—Р°РјРѕРІР»РµРЅРЅСЏ СЃРєР°СЃРѕРІР°РЅРѕ"
        /// </remarks>
        /// <param name="id">ID Р·Р°РјРѕРІР»РµРЅРЅСЏ.</param>
        /// <param name="dto">РќРѕРІРёР№ СЃС‚Р°С‚СѓСЃ.</param>
        /// <returns>Р РµР·СѓР»СЊС‚Р°С‚ РѕРїРµСЂР°С†С–С— С‚Р° РїСЂР°РїРѕСЂРµС†СЊ, С‡Рё Р±СѓР»Рѕ РЅР°РґС–СЃР»Р°РЅРѕ СЃРїРѕРІС–С‰РµРЅРЅСЏ.</returns>
        /// <response code="200">РЎС‚Р°С‚СѓСЃ СѓСЃРїС–С€РЅРѕ Р·РјС–РЅРµРЅРѕ.</response>
        /// <response code="403">РЎРїСЂРѕР±Р° РєРµСЂСѓРІР°С‚Рё Р·Р°РјРѕРІР»РµРЅРЅСЏРј С‡СѓР¶РѕРіРѕ СЂРµСЃС‚РѕСЂР°РЅСѓ (РґР»СЏ Р’Р»Р°СЃРЅРёРєР°/РџРµСЂСЃРѕРЅР°Р»Сѓ).</response>
        /// <response code="404">Р—Р°РјРѕРІР»РµРЅРЅСЏ РЅРµ Р·РЅР°Р№РґРµРЅРѕ.</response>
        [HttpPatch("{id}/status")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateOrderStatus(int id, [FromBody] UpdateOrderStatusDto dto)
        {
            var order = await _context.Orders
                .Include(o => o.Restaurant)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return NotFound();

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            if (userRole != UserRole.Admin.ToString())
            {
                if (userRole == UserRole.Admin.ToString() && order.Restaurant.OwnerId != userId)
                    return StatusCode(403, "Р’Рё РЅРµ РјРѕР¶РµС‚Рµ РєРµСЂСѓРІР°С‚Рё Р·Р°РјРѕРІР»РµРЅРЅСЏРјРё С‡СѓР¶РѕРіРѕ СЂРµСЃС‚РѕСЂР°РЅСѓ!");

                if (userRole == UserRole.KitchenStaff.ToString())
                {
                    var staffUser = await _context.Users.FindAsync(userId);
                    if (staffUser == null || staffUser.RestaurantId != order.RestaurantId)
                        return StatusCode(403, "Р’Рё РЅРµ С” РїСЂР°С†С–РІРЅРёРєРѕРј С†СЊРѕРіРѕ СЂРµСЃС‚РѕСЂР°РЅСѓ!");
                }
            }

            var oldStatus = order.Status;
            order.Status = dto.Status;

            string? notificationMessage = null;

            switch (dto.Status)
            {
                case OrderStatus.Pending:
                    break;

                case OrderStatus.Paid:
                    notificationMessage = $"РћРїР»Р°С‚Сѓ Р·Р° Р·Р°РјРѕРІР»РµРЅРЅСЏ #{order.Id} РѕС‚СЂРёРјР°РЅРѕ. Р”СЏРєСѓС”РјРѕ!";
                    break;

                case OrderStatus.Cooking:
                    if (oldStatus == OrderStatus.Pending || oldStatus == OrderStatus.Paid)
                    {
                        notificationMessage = $"РљСѓС…РЅСЏ РїРѕС‡Р°Р»Р° РіРѕС‚СѓРІР°С‚Рё РІР°С€Рµ Р·Р°РјРѕРІР»РµРЅРЅСЏ #{order.Id}. РЎРєРѕСЂРѕ Р±СѓРґРµ СЃРјР°С‡РЅРѕ!";
                    }
                    break;

                case OrderStatus.Ready:
                    notificationMessage = $"Р’Р°С€Рµ Р·Р°РјРѕРІР»РµРЅРЅСЏ #{order.Id} РіРѕС‚РѕРІРµ РґРѕ РІРёРґР°С‡С–! Р§РµРєР°С”РјРѕ РЅР° РІР°СЃ.";
                    break;

                case OrderStatus.Completed:
                    notificationMessage = $"Р”СЏРєСѓС”РјРѕ, С‰Рѕ РѕР±СЂР°Р»Рё {order.Restaurant.NameUA}! Р‘СѓРґРµРјРѕ РІРґСЏС‡РЅС– Р·Р° РІР°С€ РІС–РґРіСѓРє Сѓ РґРѕРґР°С‚РєСѓ.";
                    break;

                case OrderStatus.Cancelled:
                    notificationMessage = $"Р’Р°С€Рµ Р·Р°РјРѕРІР»РµРЅРЅСЏ #{order.Id} Р±СѓР»Рѕ СЃРєР°СЃРѕРІР°РЅРѕ. РЇРєС‰Рѕ С†Рµ РїРѕРјРёР»РєР°, Р·РІРµСЂРЅС–С‚СЊСЃСЏ РґРѕ РїРµСЂСЃРѕРЅР°Р»Сѓ.";
                    break;
            }

            if (!string.IsNullOrEmpty(notificationMessage))
            {
                var notification = new Notification
                {
                    UserId = order.UserId,
                    Message = notificationMessage,
                    DateSent = DateTime.UtcNow,
                    IsRead = false
                };
                _context.Notifications.Add(notification);
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = $"РЎС‚Р°С‚СѓСЃ Р·РјС–РЅРµРЅРѕ РЅР° {order.Status}", notificationSent = notificationMessage != null });
        }

        [HttpPatch("iot/{id}/status")]
        [AllowAnonymous]
        public async Task<IActionResult> UpdateOrderStatusFromIoT(int id, [FromQuery] string serialNumber, [FromBody] UpdateOrderStatusDto dto)
        {
            var device = await _context.IoTDevices.FirstOrDefaultAsync(d => d.SerialNumber == serialNumber && d.IsActive);
            if (device == null) return Unauthorized("РџСЂРёСЃС‚СЂС–Р№ РЅРµ Р·Р°СЂРµС”СЃС‚СЂРѕРІР°РЅРѕ Р°Р±Рѕ РІС–РЅ Р·Р°Р±Р»РѕРєРѕРІР°РЅРёР№");

            var order = await _context.Orders
                .Include(o => o.Restaurant)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return NotFound("Р—Р°РјРѕРІР»РµРЅРЅСЏ РЅРµ Р·РЅР°Р№РґРµРЅРѕ");

            if (order.RestaurantId != device.RestaurantId)
                return StatusCode(403, "Р¦РµР№ С‚РµСЂРјС–РЅР°Р» РЅРµ РјР°С” РїСЂР°РІ РЅР° РєРµСЂСѓРІР°РЅРЅСЏ С†РёРј Р·Р°РјРѕРІР»РµРЅРЅСЏРј");

            var oldStatus = order.Status;
            order.Status = dto.Status;

            string? notificationMessage = null;

            switch (dto.Status)
            {
                case OrderStatus.Paid:
                    notificationMessage = $"РћРїР»Р°С‚Сѓ Р·Р° Р·Р°РјРѕРІР»РµРЅРЅСЏ #{order.Id} РѕС‚СЂРёРјР°РЅРѕ. Р”СЏРєСѓС”РјРѕ!";
                    break;

                case OrderStatus.Cooking:
                    if (oldStatus == OrderStatus.Pending || oldStatus == OrderStatus.Paid)
                    {
                        notificationMessage = $"РљСѓС…РЅСЏ РїРѕС‡Р°Р»Р° РіРѕС‚СѓРІР°С‚Рё Р·Р°РјРѕРІР»РµРЅРЅСЏ #{order.Id}. РЎРєРѕСЂРѕ Р±СѓРґРµ СЃРјР°С‡РЅРѕ!";
                    }
                    break;

                case OrderStatus.Ready:
                    notificationMessage = $"Р’Р°С€Рµ Р·Р°РјРѕРІР»РµРЅРЅСЏ #{order.Id} РіРѕС‚РѕРІРµ РґРѕ РІРёРґР°С‡С–! Р§РµРєР°С”РјРѕ РЅР° РІР°СЃ.";
                    break;

                case OrderStatus.Completed:
                    notificationMessage = $"Р”СЏРєСѓС”РјРѕ, С‰Рѕ РѕР±СЂР°Р»Рё {order.Restaurant.NameUA}!";
                    break;

                case OrderStatus.Cancelled:
                    notificationMessage = $"Р—Р°РјРѕРІР»РµРЅРЅСЏ #{order.Id} Р±СѓР»Рѕ СЃРєР°СЃРѕРІР°РЅРѕ.";
                    break;
            }

            if (!string.IsNullOrEmpty(notificationMessage))
            {
                var notification = new Notification
                {
                    UserId = order.UserId,
                    Message = notificationMessage,
                    DateSent = DateTime.UtcNow,
                    IsRead = false
                };
                _context.Notifications.Add(notification);
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "IoT Sync Success",
                newStatus = order.Status.ToString(),
                orderId = order.Id
            });
        }

        private static OrderDto MapToDto(Order order)
        {
            return new OrderDto
            {
                Id = order.Id,
                CreatedAt = order.CreatedAt.ToUkraineTime(),
                VisitTime = order.VisitTime.ToUkraineTime(),
                Status = order.Status.ToString(),
                TotalAmount = order.TotalAmount,
                Comment = order.Comment,
                RestaurantId = order.RestaurantId,
                UserId = order.UserId,
                UserName = order.User?.FullName ?? "Unknown",
                Items = order.Items.Select(i => new OrderItemDto
                {
                    Id = i.Id,
                    DishId = i.DishId,
                    DishName = i.Dish?.NameUA ?? "Unknown",
                    Quantity = i.Quantity,
                    Price = i.Price
                }).ToList()
            };
        }
    }
}
