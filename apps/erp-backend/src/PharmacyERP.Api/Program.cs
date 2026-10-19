using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using PharmacyERP.Application.Common.Interfaces.Persistence;

using PharmacyERP.Infrastructure.Persistence.Database;
using PharmacyERP.Infrastructure.Persistence.Interceptors;
using PharmacyERP.Infrastructure.Persistence.Repositories;
using PharmacyERP.Infrastructure.Persistence.UoW;


namespace PharmacyERP.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' was not found.");

            builder.Services.AddScoped<TenantRlsConnectionInterceptor>();

            builder.Services.AddDbContext<PharmacyDbContext>((sp, options) =>
            {
                options.UseNpgsql(connectionString, npgsql =>
                {
                    npgsql.MigrationsHistoryTable("__EFMigrationsHistory", "integration");
                });

                options.AddInterceptors(sp.GetRequiredService<TenantRlsConnectionInterceptor>());
            });

          
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));


            builder.Services.AddControllers();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Pharmacy ERP API",
                    Version = "v1"
                });
            });

            var app = builder.Build();



            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Pharmacy ERP API v1");
                });
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}