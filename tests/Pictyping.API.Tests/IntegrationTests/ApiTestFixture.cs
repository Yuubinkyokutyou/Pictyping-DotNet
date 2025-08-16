using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Pictyping.Infrastructure.Data;
using StackExchange.Redis;

namespace Pictyping.API.Tests.IntegrationTests;

public class ApiTestFixture : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the real database context
            var dbDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<PictypingDbContext>));

            if (dbDescriptor != null)
            {
                services.Remove(dbDescriptor);
            }

            // Remove Redis connection
            var redisDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(IConnectionMultiplexer));
            if (redisDescriptor != null)
            {
                services.Remove(redisDescriptor);
            }

            // Add in-memory database
            services.AddDbContext<PictypingDbContext>(options =>
            {
                options.UseInMemoryDatabase("TestDatabase_" + Guid.NewGuid());
            });

            // Mock Redis
            var mockRedis = new Mock<IConnectionMultiplexer>();
            var mockDatabase = new Mock<IDatabase>();
            mockRedis.Setup(r => r.GetDatabase(It.IsAny<int>(), It.IsAny<object>())).Returns(mockDatabase.Object);
            services.AddSingleton(mockRedis.Object);

            services.AddLogging(builder => builder.AddConsole());

            // 既存の認証サービスを削除
            var authServiceDescriptors = services
                .Where(d => d.ServiceType == typeof(IAuthenticationService) ||
                           d.ServiceType == typeof(IAuthenticationSchemeProvider) ||
                           d.ServiceType == typeof(IAuthenticationHandlerProvider))
                .ToList();

            foreach (var descriptor in authServiceDescriptors)
            {
                services.Remove(descriptor);
            }

            // テスト用の認証を追加
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = TestAuthenticationHandler.AuthenticationScheme;
                options.DefaultChallengeScheme = TestAuthenticationHandler.AuthenticationScheme;
            })
            .AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>(
                TestAuthenticationHandler.AuthenticationScheme, options => { });
        });

        builder.UseEnvironment("Testing");
    }

    public async Task<PictypingDbContext> GetDbContextAsync()
    {
        var scope = Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<PictypingDbContext>();
        await context.Database.EnsureCreatedAsync();
        return context;
    }
}