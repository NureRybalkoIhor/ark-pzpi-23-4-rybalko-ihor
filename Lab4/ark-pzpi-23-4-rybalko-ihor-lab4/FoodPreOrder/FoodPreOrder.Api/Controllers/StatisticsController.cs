п»їusing FoodPreOrder.Api.Services;
using FoodPreOrder.Application.DTOs.Admin;
using FoodPreOrder.Application.Interfaces;
using FoodPreOrder.Domain.Enums;
using FoodPreOrder.Persistence.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FoodPreOrder.Api.Controllers
{
    /// <summary>
    /// РљРѕРЅС‚СЂРѕР»РµСЂ РґР»СЏ Р°РЅР°Р»С–С‚РёРєРё С‚Р° Р·РІС–С‚РЅРѕСЃС‚С–.
    /// Р—Р°Р±РµР·РїРµС‡СѓС” РіРµРЅРµСЂР°С†С–СЋ С„С–РЅР°РЅСЃРѕРІРёС… Р·РІС–С‚С–РІ, Р°РЅР°Р»С–Р· РїРѕРїСѓР»СЏСЂРЅРѕСЃС‚С– СЃС‚СЂР°РІ С‚Р° Р·Р°РІР°РЅС‚Р°Р¶РµРЅРѕСЃС‚С– СЂРµСЃС‚РѕСЂР°РЅСѓ.
    /// РџС–РґС‚СЂРёРјСѓС” РµРєСЃРїРѕСЂС‚ РґР°РЅРёС… Сѓ С„РѕСЂРјР°С‚С– PDF.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class StatisticsController : ControllerBase
    {
        private readonly IStatisticsService _statsService;
        private readonly ApplicationDbContext _context;
        private readonly PdfReportService _pdfService;

        public StatisticsController(IStatisticsService statsService, ApplicationDbContext context, PdfReportService pdfService)
        {
            _statsService = statsService;
            _context = context;
            _pdfService = pdfService;
        }

        /// <summary>
        /// РћС‚СЂРёРјСѓС” СЃС‚Р°С‚РёСЃС‚РёРєСѓ РґРѕС…РѕРґС–РІ С‚Р° РєС–Р»СЊРєРѕСЃС‚С– Р·Р°РјРѕРІР»РµРЅСЊ РїРѕ РґРЅСЏС… Р·Р° РІРєР°Р·Р°РЅРёР№ РїРµСЂС–РѕРґ.
        /// </summary>
        /// <param name="restaurantId">ID СЂРµСЃС‚РѕСЂР°РЅСѓ.</param>
        /// <param name="from">РџРѕС‡Р°С‚РєРѕРІР° РґР°С‚Р° РїРµСЂС–РѕРґСѓ.</param>
        /// <param name="to">РљС–РЅС†РµРІР° РґР°С‚Р° РїРµСЂС–РѕРґСѓ.</param>
        /// <returns>РЎРїРёСЃРѕРє С‰РѕРґРµРЅРЅРѕС— СЃС‚Р°С‚РёСЃС‚РёРєРё (JSON).</returns>
        [HttpGet("restaurant/{restaurantId}/daily")]
        [Authorize(Roles = "Admin,RestaurantOwner")]
        [ProducesResponseType(typeof(List<DailyStatsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<List<DailyStatsDto>>> GetDailyStats(int restaurantId, [FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            if (!await CheckAccess(restaurantId)) return StatusCode(403, "Р¦Рµ РЅРµ РІР°С€ СЂРµСЃС‚РѕСЂР°РЅ");
            var stats = await _statsService.GetDailyStatsAsync(restaurantId, from, to);
            return Ok(stats);
        }

        /// <summary>
        /// Р“РµРЅРµСЂСѓС” С‚Р° Р·Р°РІР°РЅС‚Р°Р¶СѓС” PDF-Р·РІС–С‚ Р· С„С–РЅР°РЅСЃРѕРІРѕСЋ СЃС‚Р°С‚РёСЃС‚РёРєРѕСЋ Р·Р° РїРµСЂС–РѕРґ.
        /// </summary>
        /// <param name="restaurantId">ID СЂРµСЃС‚РѕСЂР°РЅСѓ.</param>
        /// <param name="from">РџРѕС‡Р°С‚РєРѕРІР° РґР°С‚Р°.</param>
        /// <param name="to">РљС–РЅС†РµРІР° РґР°С‚Р°.</param>
        /// <returns>Р¤Р°Р№Р» PDF.</returns>
        [HttpGet("restaurant/{restaurantId}/report/pdf")]
        [Authorize(Roles = "Admin,RestaurantOwner")]
        [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> DownloadReportPdf(int restaurantId, [FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            if (!await CheckAccess(restaurantId)) return StatusCode(403, "Р¦Рµ РЅРµ РІР°С€ СЂРµСЃС‚РѕСЂР°РЅ");

            var stats = await _statsService.GetDailyStatsAsync(restaurantId, from, to);
            var restaurant = await _context.Restaurants.FindAsync(restaurantId);

            var pdfFile = _pdfService.GenerateFinancialReport(restaurant.NameUA, from, to, stats);

            return File(pdfFile, "application/pdf", $"FinancialReport_{from:yyyyMMdd}-{to:yyyyMMdd}.pdf");
        }

        /// <summary>
        /// Р—Р°РІР°РЅС‚Р°Р¶СѓС” РґРµС‚Р°Р»СЊРЅРёР№ Р¶СѓСЂРЅР°Р» Р·Р°РјРѕРІР»РµРЅСЊ Р·Р° РєРѕРЅРєСЂРµС‚РЅСѓ РґР°С‚Сѓ Сѓ С„РѕСЂРјР°С‚С– PDF.
        /// </summary>
        /// <param name="restaurantId">ID СЂРµСЃС‚РѕСЂР°РЅСѓ.</param>
        /// <param name="date">Р”Р°С‚Р° Р·РІС–С‚Сѓ.</param>
        /// <returns>Р¤Р°Р№Р» PDF Р· Р»РѕРіРѕРј РѕРїРµСЂР°С†С–Р№.</returns>
        [HttpGet("restaurant/{restaurantId}/daily-log/pdf")]
        [Authorize(Roles = "Admin,RestaurantOwner")]
        [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> DownloadDailyLogPdf(int restaurantId, [FromQuery] DateTime date)
        {
            if (!await CheckAccess(restaurantId)) return StatusCode(403, "Р¦Рµ РЅРµ РІР°С€ СЂРµСЃС‚РѕСЂР°РЅ");

            var logs = await _statsService.GetDailyOrderLogAsync(restaurantId, date);
            var restaurant = await _context.Restaurants.FindAsync(restaurantId);

            var pdfFile = _pdfService.GenerateDailyLogReport(restaurant.NameUA, date, logs);

            return File(pdfFile, "application/pdf", $"DailyLog_{date:yyyyMMdd}.pdf");
        }

        /// <summary>
        /// РђРЅР°Р»С–Р· РїС–РєРѕРІРѕРіРѕ РЅР°РІР°РЅС‚Р°Р¶РµРЅРЅСЏ. РџРѕРєР°Р·СѓС” РєС–Р»СЊРєС–СЃС‚СЊ Р·Р°РјРѕРІР»РµРЅСЊ Сѓ СЂРѕР·СЂС–Р·С– РіРѕРґРёРЅ РґРѕР±Рё.
        /// </summary>
        /// <param name="restaurantId">ID СЂРµСЃС‚РѕСЂР°РЅСѓ.</param>
        /// <param name="from">РџРѕС‡Р°С‚РєРѕРІР° РґР°С‚Р°.</param>
        /// <param name="to">РљС–РЅС†РµРІР° РґР°С‚Р°.</param>
        /// <returns>РЎС‚Р°С‚РёСЃС‚РёРєР° РїРѕ РіРѕРґРёРЅР°С… (JSON).</returns>
        [HttpGet("restaurant/{restaurantId}/peak-hours")]
        [Authorize(Roles = "Admin,RestaurantOwner")]
        [ProducesResponseType(typeof(List<PeakLoadDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<List<PeakLoadDto>>> GetPeakHours(
            int restaurantId,
            [FromQuery] DateTime from,
            [FromQuery] DateTime to)
        {
            if (!await CheckAccess(restaurantId)) return StatusCode(403, "Р¦Рµ РЅРµ РІР°С€ СЂРµСЃС‚РѕСЂР°РЅ");

            var peaks = await _statsService.GetPeakLoadingAsync(restaurantId, from, to);
            return Ok(peaks);
        }

        /// <summary>
        /// Р—Р°РІР°РЅС‚Р°Р¶СѓС” PDF-Р·РІС–С‚ РїСЂРѕ РїС–РєРѕРІС– РіРѕРґРёРЅРё РЅР°РІР°РЅС‚Р°Р¶РµРЅРЅСЏ.
        /// </summary>
        /// <returns>Р¤Р°Р№Р» PDF.</returns>
        [HttpGet("restaurant/{restaurantId}/peak-hours/pdf")]
        [Authorize(Roles = "Admin,RestaurantOwner")]
        [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> DownloadPeakHoursPdf(
            int restaurantId,
            [FromQuery] DateTime from,
            [FromQuery] DateTime to)
        {
            if (!await CheckAccess(restaurantId)) return StatusCode(403, "Р¦Рµ РЅРµ РІР°С€ СЂРµСЃС‚РѕСЂР°РЅ");

            var peaks = await _statsService.GetPeakLoadingAsync(restaurantId, from, to);
            var restaurant = await _context.Restaurants.FindAsync(restaurantId);

            var pdfFile = _pdfService.GeneratePeakHoursReport(restaurant.NameUA, peaks);
            string fileName = $"PeakHours_{from:yyyyMMdd}-{to:yyyyMMdd}.pdf";

            return File(pdfFile, "application/pdf", fileName);
        }

        /// <summary>
        /// РћС‚СЂРёРјСѓС” СЂРµР№С‚РёРЅРі РЅР°Р№РїРѕРїСѓР»СЏСЂРЅС–С€РёС… СЃС‚СЂР°РІ Р·Р° РєС–Р»СЊРєС–СЃС‚СЋ РїСЂРѕРґР°Р¶С–РІ.
        /// </summary>
        /// <param name="restaurantId">ID СЂРµСЃС‚РѕСЂР°РЅСѓ.</param>
        /// <returns>РЎРїРёСЃРѕРє РўРћРџ СЃС‚СЂР°РІ (JSON).</returns>
        [HttpGet("restaurant/{restaurantId}/top-dishes")]
        [Authorize(Roles = "Admin,RestaurantOwner")]
        [ProducesResponseType(typeof(List<TopDishDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<List<TopDishDto>>> GetTopDishes(int restaurantId)
        {
            if (!await CheckAccess(restaurantId)) return StatusCode(403, "Р¦Рµ РЅРµ РІР°С€ СЂРµСЃС‚РѕСЂР°РЅ");
            var tops = await _statsService.GetTopDishesAsync(restaurantId);
            return Ok(tops);
        }

        /// <summary>
        /// Р—Р°РІР°РЅС‚Р°Р¶СѓС” PDF-Р·РІС–С‚ Р· СЂРµР№С‚РёРЅРіРѕРј РїРѕРїСѓР»СЏСЂРЅРёС… СЃС‚СЂР°РІ.
        /// </summary>
        /// <returns>Р¤Р°Р№Р» PDF.</returns>
        [HttpGet("restaurant/{restaurantId}/top-dishes/pdf")]
        [Authorize(Roles = "Admin,RestaurantOwner")]
        [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> DownloadTopDishesPdf(int restaurantId)
        {
            if (!await CheckAccess(restaurantId)) return StatusCode(403, "Р¦Рµ РЅРµ РІР°С€ СЂРµСЃС‚РѕСЂР°РЅ");

            var tops = await _statsService.GetTopDishesAsync(restaurantId);
            var restaurant = await _context.Restaurants.FindAsync(restaurantId);

            var pdfFile = _pdfService.GenerateTopDishesReport(restaurant.NameUA, tops);

            return File(pdfFile, "application/pdf", "TopDishes.pdf");
        }

        /// <summary>
        /// Р“Р»РѕР±Р°Р»СЊРЅР° Р°РЅР°Р»С–С‚РёРєР° СЃРёСЃС‚РµРјРё (Dashboard).
        /// </summary>
        /// <remarks>
        /// Р”РѕСЃС‚СѓРїРЅРѕ С‚С–Р»СЊРєРё РґР»СЏ РђРґРјС–РЅС–СЃС‚СЂР°С‚РѕСЂР° СЃРёСЃС‚РµРјРё. РџРѕРєР°Р·СѓС” Р·Р°РіР°Р»СЊРЅСѓ РєС–Р»СЊРєС–СЃС‚СЊ РєРѕСЂРёСЃС‚СѓРІР°С‡С–РІ, СЂРµСЃС‚РѕСЂР°РЅС–РІ С‚Р° РѕР±С–Рі РєРѕС€С‚С–РІ.
        /// </remarks>
        /// <returns>Р”Р°РЅС– РґР»СЏ РґР°С€Р±РѕСЂРґСѓ.</returns>
        [HttpGet("admin/dashboard")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(AdminDashboardDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<AdminDashboardDto>> GetAdminDashboard()
        {
            var dashboard = await _statsService.GetSystemDashboardAsync();
            return Ok(dashboard);
        }

        /// <summary>
        /// Р—Р°РІР°РЅС‚Р°Р¶СѓС” РіР»РѕР±Р°Р»СЊРЅРёР№ Р·РІС–С‚ РїРѕ СЃРёСЃС‚РµРјС– Сѓ С„РѕСЂРјР°С‚С– PDF (РґР»СЏ РђРґРјС–РЅС–СЃС‚СЂР°С‚РѕСЂР°).
        /// </summary>
        /// <returns>Р¤Р°Р№Р» PDF.</returns>
        [HttpGet("admin/dashboard/pdf")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> DownloadAdminDashboardPdf()
        {
            var dashboardData = await _statsService.GetSystemDashboardAsync();
            var pdfFile = _pdfService.GenerateSystemDashboardReport(dashboardData);

            string fileName = $"System_Dashboard_{DateTime.Now:yyyyMMdd_HHmm}.pdf";
            return File(pdfFile, "application/pdf", fileName);
        }

        private async Task<bool> CheckAccess(int restaurantId)
        {
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            if (userRole == UserRole.Admin.ToString()) return true;

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var restaurant = await _context.Restaurants.FindAsync(restaurantId);

            return restaurant != null && restaurant.OwnerId == userId;
        }
    }
}
