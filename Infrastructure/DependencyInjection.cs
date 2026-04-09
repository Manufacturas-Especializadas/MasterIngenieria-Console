using Core.Interfaces;
using Infrastructure.Models;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("Connection");

            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString));

            //services
            services.AddScoped<IExcelReader, ClosedXmlReader>();
            services.AddScoped<ExcelOrchestrator>();

            return services;
        }
    }
}