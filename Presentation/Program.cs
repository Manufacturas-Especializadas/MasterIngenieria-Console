using Infrastructure;
using Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddInfrastructureServices(builder.Configuration);

using IHost host = builder.Build();

Console.WriteLine("=== Programa iniciando ===");

using (var scope = host.Services.CreateScope())
{
    var orchestrator = scope.ServiceProvider.GetRequiredService<ExcelOrchestrator>();

    string pathProduction = @"\\192.168.25.54\Ing Industrial\Master de Ingeniería.xlsx";
    string pathDevelopment = @"\\192.168.25.54\usuarios2\A&T\Master de Ingeniería.xlsx";

    await orchestrator.RunAsync(pathProduction);
}

await host.RunAsync();