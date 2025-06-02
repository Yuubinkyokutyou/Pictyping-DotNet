using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Pictyping.Infrastructure.Data;
using StackExchange.Redis;
using System.Text;
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
    
    // JWT Bearer認証の設定
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
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
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? ""))
        };
        
        // ドメイン間でトークンを共有するための設定
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                // クエリパラメータからもトークンを取得（リダイレクト時用）
                var token = context.Request.Query["access_token"];
                if (!string.IsNullOrEmpty(token))
                {
                    context.Token = token;
                }
                return Task.CompletedTask;
            }
        };
    })
    .AddGoogle(googleOptions =>
    {
        googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"] ?? "";
        googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"] ?? "";
    });

builder.Services.AddAuthorization();

// Services
builder.Services.AddScoped<Pictyping.API.Services.IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<Pictyping.Core.Interfaces.IUserService, Pictyping.Infrastructure.Services.UserService>();
builder.Services.AddScoped<Pictyping.Core.Interfaces.ITypingBattleService, Pictyping.Infrastructure.Services.TypingBattleService>();
builder.Services.AddScoped<Pictyping.Core.Interfaces.IRankingService, Pictyping.Infrastructure.Services.RankingService>();
builder.Services.AddScoped<Pictyping.Core.Interfaces.IDataSeedingService, Pictyping.Infrastructure.Services.DataSeedingService>();

var app = builder.Build();

// Initialize database and seed data
await InitializeDatabaseAsync(app);

async Task InitializeDatabaseAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<PictypingDbContext>();
    await context.Database.EnsureCreatedAsync();
    
    // Seed development data if in development environment
    if (app.Environment.IsDevelopment())
    {
        var dataSeedingService = scope.ServiceProvider.GetRequiredService<IDataSeedingService>();
        await dataSeedingService.SeedDevelopmentDataAsync();
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