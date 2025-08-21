using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Pictyping.Infrastructure.Data;
using StackExchange.Redis;
using Pictyping.API.Services;
using Pictyping.Core.Interfaces;
using Pictyping.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Pictyping API",
        Version = "v1",
        Description = "Pictyping game API for typing battles and rankings"
    });
});

// Database
builder.Services.AddDbContext<PictypingDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Redis
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configuration = ConfigurationOptions.Parse(
        builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379",
        true);
    return ConnectionMultiplexer.Connect(configuration);
});

// CORS - Unity WebGLとドメイン間通信のため
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins",
        policy =>
        {
            policy.WithOrigins(
                    "https://pictyping.com",
                    "https://new.pictyping.com",
                    "http://localhost:3000",
                    "http://localhost:5173")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        });
});

// Authentication
var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key is not configured");
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "Pictyping";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "PictypingUsers";

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew = TimeSpan.Zero
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                // Allow the token to be passed from query string for SignalR
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) && 
                    (path.StartsWithSegments("/hubs")))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    })
    .AddCookie("TempCookie", options =>
    {
        options.Cookie.Name = "Pictyping.TempAuth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = builder.Environment.IsDevelopment() 
            ? CookieSecurePolicy.SameAsRequest 
            : CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
    })
    .AddGoogle(googleOptions =>
    {
        googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"] ?? "";
        googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"] ?? "";
        googleOptions.SignInScheme = "TempCookie";
    });

builder.Services.AddAuthorization();

// Services
builder.Services.AddScoped<Pictyping.API.Services.IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<Pictyping.API.Services.IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<Pictyping.Core.Interfaces.IUserService, Pictyping.Infrastructure.Services.UserService>();
builder.Services.AddScoped<Pictyping.Core.Interfaces.ITypingBattleService, Pictyping.Infrastructure.Services.TypingBattleService>();
builder.Services.AddScoped<Pictyping.Core.Interfaces.IRankingService, Pictyping.Infrastructure.Services.RankingService>();
builder.Services.AddScoped<Pictyping.Core.Interfaces.IDataSeedingService, Pictyping.Infrastructure.Services.DataSeedingService>();

var app = builder.Build();

// Initialize database and seed data
// Only initialize database in non-test environments
if (!app.Environment.IsEnvironment("Testing"))
{
    await InitializeDatabaseAsync(app);
}

async Task InitializeDatabaseAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<PictypingDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    
    try
    {
        // Test database connection
        await context.Database.CanConnectAsync();
        logger.LogInformation("Database connection successful");
        
        // Check if we have existing data (migrated from Rails)
        var userCount = await context.Users.CountAsync();
        logger.LogInformation($"Found {userCount} users in database");
        
        // Only seed if no data exists and seeding is enabled
        if (userCount == 0 && app.Environment.IsDevelopment())
        {
            var configuration = scope.ServiceProvider.GetRequiredService<IConfiguration>();
            var seedData = configuration.GetValue<bool>("DataSeeding:SeedDataOnStartup", false);
            
            if (seedData)
            {
                logger.LogInformation("Starting development data seeding...");
                var dataSeedingService = scope.ServiceProvider.GetRequiredService<IDataSeedingService>();
                await dataSeedingService.SeedDevelopmentDataAsync();
            }
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Database initialization failed");
        throw;
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(c =>
    {
        c.SerializeAsV2 = true;
    });
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Pictyping API V1");
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowSpecificOrigins");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Health check endpoint
app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }));

app.Run();

public partial class Program { }