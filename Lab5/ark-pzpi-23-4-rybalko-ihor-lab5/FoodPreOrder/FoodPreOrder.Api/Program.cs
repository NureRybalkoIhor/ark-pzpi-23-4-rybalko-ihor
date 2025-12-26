using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using FoodPreOrder.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using FoodPreOrder.Api.Services;
using FoodPreOrder.Infrastructure.Security;
using FoodPreOrder.Application.Interfaces;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            builder.Configuration.GetSection("Jwt:Key").Value!)),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true
    };
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString,
        b => b.MigrationsAssembly("FoodPreOrder.Persistence")));

builder.Services.AddScoped<IFileStorageService, FileStorageService>();
builder.Services.AddScoped<ICalculationService, CalculationService>();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IVerificationService, VerificationService>();
builder.Services.AddScoped<IStatisticsService, StatisticsService>();
builder.Services.AddScoped<PdfReportService>();
builder.Services.AddScoped<IoTService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();


builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "FoodPreOrder API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Р В Р’В Р Р†Р вЂљРІвЂћСћР В Р’В Р В РІР‚В Р В Р’В Р вЂ™Р’ВµР В Р’В Р СћРІР‚ВР В Р Р‹Р Р†Р вЂљРІР‚СљР В Р Р‹Р Р†Р вЂљРЎв„ўР В Р Р‹Р В Р вЂ° Р В Р Р‹Р Р†Р вЂљРЎв„ўР В Р’В Р РЋРІР‚СћР В Р’В Р РЋРІР‚СњР В Р’В Р вЂ™Р’ВµР В Р’В Р В РІР‚В¦ Р В Р Р‹Р РЋРІР‚Сљ Р В Р Р‹Р Р†Р вЂљРЎвЂєР В Р’В Р РЋРІР‚СћР В Р Р‹Р В РІР‚С™Р В Р’В Р РЋР’ВР В Р’В Р вЂ™Р’В°Р В Р Р‹Р Р†Р вЂљРЎв„ўР В Р Р‹Р Р†Р вЂљРІР‚Сљ: Bearer {Р В Р’В Р В РІР‚В Р В Р’В Р вЂ™Р’В°Р В Р Р‹Р Р†РІР‚С™Р’В¬Р В Р’В Р вЂ™Р’Вµ_Р В Р’В Р СћРІР‚ВР В Р’В Р РЋРІР‚СћР В Р’В Р В РІР‚В Р В Р’В Р РЋРІР‚вЂњР В Р’В Р вЂ™Р’Вµ_Р В Р Р‹Р В РЎвЂњР В Р’В Р вЂ™Р’В»Р В Р’В Р РЋРІР‚СћР В Р’В Р В РІР‚В Р В Р’В Р РЋРІР‚Сћ_Р В Р Р‹Р Р†Р вЂљРЎв„ўР В Р’В Р РЋРІР‚СћР В Р’В Р РЋРІР‚СњР В Р’В Р вЂ™Р’ВµР В Р’В Р В РІР‚В¦}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>(); // Вкажи свою назву контексту
        context.Database.Migrate();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Помилка під час створення бази даних.");
    }
}

app.Run();
