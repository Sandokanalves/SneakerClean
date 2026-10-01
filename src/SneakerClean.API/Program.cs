using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SneakerClean.API.BackgroundServices;
using SneakerClean.Application.Interfaces;
using SneakerClean.Application.Services;
using SneakerClean.Domain.Entities;
using SneakerClean.Domain.Enums;
using SneakerClean.Domain.Interfaces;
using SneakerClean.Infrastructure.Messaging;
using SneakerClean.Infrastructure.Persistence;
using SneakerClean.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// ─── CORS ──────────────────────────────────────────────────────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowVueApp", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// ─── DbContext ─────────────────────────────────────────────────────────────────
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ─── Redis Cache ───────────────────────────────────────────────────────────────
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379";
    options.InstanceName = "SneakerClean_";
});

// ─── Repositories ──────────────────────────────────────────────────────────────
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IDashboardRepository, DashboardRepository>();

// ─── Application Services ──────────────────────────────────────────────────────
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICustomerService, CustomerService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();

// Decorator Pattern para Caching no OrderService
builder.Services.AddScoped<OrderService>();
builder.Services.AddScoped<IOrderService>(provider =>
    new CacheOrderService(
        provider.GetRequiredService<OrderService>(),
        provider.GetRequiredService<Microsoft.Extensions.Caching.Distributed.IDistributedCache>()
    ));

// ─── Mensageria RabbitMQ ────────────────────────────────────────────────────────
builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection("RabbitMQ"));
builder.Services.AddScoped<RabbitMqPublisher>();
builder.Services.AddHostedService<OrderCreatedConsumer>();

// ─── JWT Authentication ─────────────────────────────────────────────────────────
var secretKey = builder.Configuration["JwtSettings:SecretKey"]
    ?? throw new InvalidOperationException("JwtSettings:SecretKey não configurada.");
var key = Encoding.ASCII.GetBytes(secretKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();
builder.Services.AddControllers();

// OpenAPI Nativo (.NET 10)
builder.Services.AddOpenApi();

var app = builder.Build();

// ─── Aplicar Migrations + Seed ─────────────────────────────────────────────────
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    var dbContext = services.GetRequiredService<ApplicationDbContext>();

    var retries = 10;
    while (retries > 0)
    {
        try
        {
            logger.LogInformation("Aplicando migrações do EF Core...");
            dbContext.Database.Migrate();
            logger.LogInformation("Migrações aplicadas com sucesso.");
            break;
        }
        catch (Exception ex)
        {
            retries--;
            logger.LogWarning(ex, "Banco de dados não disponível. Tentando novamente em 5s... ({R} tentativas restantes)", retries);
            if (retries == 0) throw;
            Thread.Sleep(5000);
        }
    }

    // ─── Seed: Primeiro Admin ───────────────────────────────────────────────────
    try
    {
        var adminEmail = "admin@primeshoecare.com";
        var existingAdmin = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == adminEmail);

        if (existingAdmin == null)
        {
            logger.LogInformation("Criando usuário Admin padrão (seed)...");
            var passwordHash = BCrypt.Net.BCrypt.HashPassword("Admin@2026!");
            var adminUser = new User("Administrador", adminEmail, passwordHash, UserRole.Admin);
            dbContext.Users.Add(adminUser);
            await dbContext.SaveChangesAsync();
            logger.LogInformation("Usuário Admin padrão criado: {Email} / Senha: Admin@2026!", adminEmail);
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Erro ao criar o usuário Admin padrão.");
    }
}

// ─── Middleware Pipeline ────────────────────────────────────────────────────────
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// Global Exception Handler
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Erro não tratado: {Message}", ex.Message);
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(new
        {
            message = "Ocorreu um erro interno no servidor.",
            detail = app.Environment.IsDevelopment() ? ex.Message : null
        });
    }
});

app.UseCors("AllowVueApp");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();