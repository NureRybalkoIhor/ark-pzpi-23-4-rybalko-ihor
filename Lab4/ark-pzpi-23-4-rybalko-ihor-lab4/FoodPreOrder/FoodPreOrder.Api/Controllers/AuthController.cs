п»їusing FoodPreOrder.Domain.Entities;
using FoodPreOrder.Domain.Enums;
using FoodPreOrder.Persistence.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;
using FoodPreOrder.Application.DTOs.Auth;
using Microsoft.AspNetCore.Authorization;
using FoodPreOrder.Infrastructure.Security;

namespace FoodPreOrder.Api.Controllers
{
    /// <summary>
    /// РљРѕРЅС‚СЂРѕР»РµСЂ РґР»СЏ РєРµСЂСѓРІР°РЅРЅСЏ Р°СѓС‚РµРЅС‚РёС„С–РєР°С†С–С”СЋ С‚Р° РѕР±Р»С–РєРѕРІРёРјРё Р·Р°РїРёСЃР°РјРё РєРѕСЂРёСЃС‚СѓРІР°С‡С–РІ.
    /// Р’С–РґРїРѕРІС–РґР°С” Р·Р° СЂРµС”СЃС‚СЂР°С†С–СЋ, РІС…С–Рґ, РІС–РґРЅРѕРІР»РµРЅРЅСЏ РїР°СЂРѕР»СЋ С‚Р° РѕС‚СЂРёРјР°РЅРЅСЏ РїСЂРѕС„С–Р»СЋ.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IJwtTokenGenerator _jwtGenerator;

        public AuthController(ApplicationDbContext context, IConfiguration configuration, IJwtTokenGenerator jwtGenerator)
        {
            _context = context;
            _jwtGenerator = jwtGenerator;
        }

        /// <summary>
        /// Р РµС”СЃС‚СЂР°С†С–СЏ РЅРѕРІРѕРіРѕ РєРѕСЂРёСЃС‚СѓРІР°С‡Р° РІ СЃРёСЃС‚РµРјС–.
        /// </summary>
        /// <remarks>
        /// РЎС‚РІРѕСЂСЋС” РЅРѕРІРѕРіРѕ РєРѕСЂРёСЃС‚СѓРІР°С‡Р° Р· СЂРѕР»Р»СЋ "Customer" Р·Р° Р·Р°РјРѕРІС‡СѓРІР°РЅРЅСЏРј.
        /// РџР°СЂРѕР»СЊ Р·Р±РµСЂС–РіР°С”С‚СЊСЃСЏ Сѓ С…РµС€РѕРІР°РЅРѕРјСѓ РІРёРіР»СЏРґС–.
        /// </remarks>
        /// <param name="request">DTO Р· РґР°РЅРёРјРё РґР»СЏ СЂРµС”СЃС‚СЂР°С†С–С— (Р†Рј'СЏ, Email, РўРµР»РµС„РѕРЅ, РџР°СЂРѕР»СЊ).</param>
        /// <returns>РџРѕРІС–РґРѕРјР»РµРЅРЅСЏ РїСЂРѕ СѓСЃРїС–С€РЅСѓ СЂРµС”СЃС‚СЂР°С†С–СЋ.</returns>
        /// <response code="200">Р РµС”СЃС‚СЂР°С†С–СЏ РїСЂРѕР№С€Р»Р° СѓСЃРїС–С€РЅРѕ.</response>
        /// <response code="400">РљРѕСЂРёСЃС‚СѓРІР°С‡ Р· С‚Р°РєРёРј Email РІР¶Рµ С–СЃРЅСѓС”.</response>
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<User>> Register(RegisterUserDto request)
        {
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            {
                return BadRequest("РљРѕСЂРёСЃС‚СѓРІР°С‡ Р· С‚Р°РєРёРј email РІР¶Рµ С–СЃРЅСѓС”.");
            }

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var user = new User
            {
                FullName = request.FullName,
                Email = request.Email,
                Phone = request.Phone,
                PasswordHash = passwordHash,
                Role = UserRole.Customer,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok("Р РµС”СЃС‚СЂР°С†С–СЏ СѓСЃРїС–С€РЅР°! РўРµРїРµСЂ РІРё РјРѕР¶РµС‚Рµ СѓРІС–Р№С‚Рё.");
        }

        /// <summary>
        /// Р’С…С–Рґ Сѓ СЃРёСЃС‚РµРјСѓ (Login).
        /// </summary>
        /// <remarks>
        /// РџРµСЂРµРІС–СЂСЏС” email С‚Р° РїР°СЂРѕР»СЊ. РЈ СЂР°Р·С– СѓСЃРїС–С…Сѓ РїРѕРІРµСЂС‚Р°С” JWT С‚РѕРєРµРЅ, СЏРєРёР№ РїРѕС‚СЂС–Р±РЅРѕ РІРёРєРѕСЂРёСЃС‚РѕРІСѓРІР°С‚Рё РґР»СЏ Р°РІС‚РѕСЂРёР·РѕРІР°РЅРёС… Р·Р°РїРёС‚С–РІ.
        /// </remarks>
        /// <param name="request">Email С‚Р° РїР°СЂРѕР»СЊ РєРѕСЂРёСЃС‚СѓРІР°С‡Р°.</param>
        /// <returns>РћР±'С”РєС‚ Р· JWT С‚РѕРєРµРЅРѕРј С‚Р° С–РЅС„РѕСЂРјР°С†С–С”СЋ РїСЂРѕ РєРѕСЂРёСЃС‚СѓРІР°С‡Р°.</returns>
        /// <response code="200">РЈСЃРїС–С€РЅРёР№ РІС…С–Рґ. РџРѕРІРµСЂС‚Р°С” С‚РѕРєРµРЅ.</response>
        /// <response code="400">РќРµРІС–СЂРЅРёР№ email Р°Р±Рѕ РїР°СЂРѕР»СЊ.</response>
        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AuthResponseDto>> Login(LoginDto request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null)
            {
                return BadRequest("РќРµРІС–СЂРЅРёР№ email Р°Р±Рѕ РїР°СЂРѕР»СЊ.");
            }

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return BadRequest("РќРµРІС–СЂРЅРёР№ email Р°Р±Рѕ РїР°СЂРѕР»СЊ.");
            }

            string token = _jwtGenerator.GenerateToken(user);

            var response = new AuthResponseDto
            {
                Token = token,
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role.ToString()
            };

            return Ok(response);
        }

        /// <summary>
        /// Р—Р°РїРёС‚ РЅР° РІС–РґРЅРѕРІР»РµРЅРЅСЏ Р·Р°Р±СѓС‚РѕРіРѕ РїР°СЂРѕР»СЋ.
        /// </summary>
        /// <remarks>
        /// Р“РµРЅРµСЂСѓС” С‚РёРјС‡Р°СЃРѕРІРёР№ С‚РѕРєРµРЅ РґР»СЏ СЃРєРёРґР°РЅРЅСЏ РїР°СЂРѕР»СЋ. 
        /// РЈ СЂРµР°Р»СЊРЅРѕРјСѓ СЃРµСЂРµРґРѕРІРёС‰С– С†РµР№ С‚РѕРєРµРЅ РЅР°РґСЃРёР»Р°С”С‚СЊСЃСЏ РЅР° РїРѕС€С‚Сѓ, С‚СѓС‚ РІС–РЅ РїРѕРІРµСЂС‚Р°С”С‚СЊСЃСЏ Сѓ РІС–РґРїРѕРІС–РґС– РґР»СЏ С‚РµСЃС‚СѓРІР°РЅРЅСЏ.
        /// </remarks>
        /// <param name="request">Email РєРѕСЂРёСЃС‚СѓРІР°С‡Р°.</param>
        /// <returns>РўРѕРєРµРЅ РґР»СЏ СЃРєРёРґР°РЅРЅСЏ РїР°СЂРѕР»СЋ (Reset Token).</returns>
        /// <response code="200">РўРѕРєРµРЅ СѓСЃРїС–С€РЅРѕ Р·РіРµРЅРµСЂРѕРІР°РЅРѕ.</response>
        /// <response code="400">РљРѕСЂРёСЃС‚СѓРІР°С‡Р° Р· С‚Р°РєРёРј email РЅРµ Р·РЅР°Р№РґРµРЅРѕ.</response>
        [HttpPost("forgot-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null)
            {
                return BadRequest("РљРѕСЂРёСЃС‚СѓРІР°С‡Р° РЅРµ Р·РЅР°Р№РґРµРЅРѕ.");
            }

            string token = Convert.ToHexString(System.Security.Cryptography.RandomNumberGenerator.GetBytes(64));

            user.PasswordResetToken = token;
            user.ResetTokenExpires = DateTime.UtcNow.AddHours(1);

            await _context.SaveChangesAsync();

            return Ok(new { message = "РўРѕРєРµРЅ СЃС‚РІРѕСЂРµРЅРѕ (РІ СЂРµР°Р»СЊРЅРѕСЃС‚С– РІС–РґРїСЂР°РІР»РµРЅРѕ РЅР° РїРѕС€С‚Сѓ)", resetToken = token });
        }

        /// <summary>
        /// Р’СЃС‚Р°РЅРѕРІР»РµРЅРЅСЏ РЅРѕРІРѕРіРѕ РїР°СЂРѕР»СЋ Р·Р° РґРѕРїРѕРјРѕРіРѕСЋ С‚РѕРєРµРЅР° РІС–РґРЅРѕРІР»РµРЅРЅСЏ.
        /// </summary>
        /// <param name="request">РўРѕРєРµРЅ РІС–РґРЅРѕРІР»РµРЅРЅСЏ С‚Р° РЅРѕРІРёР№ РїР°СЂРѕР»СЊ.</param>
        /// <returns>РџРѕРІС–РґРѕРјР»РµРЅРЅСЏ РїСЂРѕ СѓСЃРїС–С€РЅСѓ Р·РјС–РЅСѓ РїР°СЂРѕР»СЋ.</returns>
        /// <response code="200">РџР°СЂРѕР»СЊ СѓСЃРїС–С€РЅРѕ Р·РјС–РЅРµРЅРѕ.</response>
        /// <response code="400">РќРµРІС–СЂРЅРёР№ Р°Р±Рѕ РїСЂРѕСЃС‚СЂРѕС‡РµРЅРёР№ С‚РѕРєРµРЅ.</response>
        [HttpPost("reset-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.PasswordResetToken == request.Token);

            if (user == null || user.ResetTokenExpires < DateTime.UtcNow)
            {
                return BadRequest("РќРµРІС–СЂРЅРёР№ Р°Р±Рѕ РїСЂРѕСЃС‚СЂРѕС‡РµРЅРёР№ С‚РѕРєРµРЅ.");
            }

            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            user.PasswordHash = passwordHash;

            user.PasswordResetToken = null;
            user.ResetTokenExpires = null;

            await _context.SaveChangesAsync();

            return Ok("РџР°СЂРѕР»СЊ СѓСЃРїС–С€РЅРѕ Р·РјС–РЅРµРЅРѕ! РўРµРїРµСЂ РјРѕР¶РµС‚Рµ СѓРІС–Р№С‚Рё Р· РЅРѕРІРёРј РїР°СЂРѕР»РµРј.");
        }

        /// <summary>
        /// РћС‚СЂРёРјР°РЅРЅСЏ РїСЂРѕС„С–Р»СЋ РїРѕС‚РѕС‡РЅРѕРіРѕ РєРѕСЂРёСЃС‚СѓРІР°С‡Р°.
        /// </summary>
        /// <remarks>
        /// Р’РёРјР°РіР°С” РЅР°СЏРІРЅРѕСЃС‚С– JWT С‚РѕРєРµРЅР° РІ Р·Р°РіРѕР»РѕРІРєСѓ Authorization (Bearer Token).
        /// </remarks>
        /// <returns>Р”Р°РЅС– РїСЂРѕС„С–Р»СЋ (ID, Р†Рј'СЏ, Email, Р РѕР»СЊ).</returns>
        /// <response code="200">РЈСЃРїС–С€РЅРµ РѕС‚СЂРёРјР°РЅРЅСЏ РїСЂРѕС„С–Р»СЋ.</response>
        /// <response code="401">РљРѕСЂРёСЃС‚СѓРІР°С‡ РЅРµ Р°РІС‚РѕСЂРёР·РѕРІР°РЅРёР№ (РІС–РґСЃСѓС‚РЅС–Р№ Р°Р±Рѕ РЅРµРІС–СЂРЅРёР№ С‚РѕРєРµРЅ).</response>
        /// <response code="404">РљРѕСЂРёСЃС‚СѓРІР°С‡Р° РЅРµ Р·РЅР°Р№РґРµРЅРѕ РІ Р±Р°Р·С– РґР°РЅРёС….</response>
        [HttpGet("me")]
        [Authorize]
        [ProducesResponseType(typeof(UserProfileDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserProfileDto>> GetMe()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdString))
            {
                return Unauthorized();
            }

            int id = int.Parse(userIdString);

            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound("РљРѕСЂРёСЃС‚СѓРІР°С‡Р° РЅРµ Р·РЅР°Р№РґРµРЅРѕ");
            }

            var profile = new UserProfileDto
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Phone = user.Phone,
                Role = user.Role.ToString()
            };

            return Ok(profile);
        }
    }
}
