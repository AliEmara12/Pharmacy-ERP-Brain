using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PharmacyERP.Application.Common.Interfaces.Context;
using PharmacyERP.Application.Common.Interfaces.Persistence;
using PharmacyERP.Infrastructure.Persistence.Database;
using PharmacyERP.Infrastructure.Persistence.Interceptors;
using PharmacyERP.Infrastructure.Persistence.Repositories;
using PharmacyERP.Infrastructure.Persistence.UoW;
using PharmacyERP.Infrastructure.Services.Context;

namespace PharmacyERP.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<TenantRlsConnectionInterceptor>();

        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' was not found.");

        services.AddDbContext<PharmacyDbContext>((sp, options) =>
        {
            options.UseNpgsql(connectionString, npgsql =>
            {
                npgsql.MigrationsHistoryTable("__EFMigrationsHistory", "integration");
            });
            options.AddInterceptors(sp.GetRequiredService<TenantRlsConnectionInterceptor>());
        });

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

        services.AddScoped<ITenantContext, TenantContext>();
        services.AddScoped<ICurrentUser, CurrentUser>();
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
        services.AddSingleton<IRuntimeModeProvider, RuntimeModeProvider>();

        // 4. خدمات التكامل (سيتم تفعيلها في Task 6)
        // services.AddScoped<IIdempotencyService, IdempotencyService>();
        // services.AddScoped<IOutboxService, OutboxService>();
        // services.AddScoped<IAuditLogger, AuditLogger>();

        return services;
    }
}