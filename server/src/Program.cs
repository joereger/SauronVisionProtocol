using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SauronVisionProtocol.Server.Services;
using Serilog;

namespace SauronVisionProtocol.Server;

public class Program
{
    public static void Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.Console()
            .CreateLogger();
            
        try
        {
            Log.Information("Starting SauronVisionProtocol server...");
            CreateHostBuilder(args).Build().Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Server terminated unexpectedly");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .UseSerilog()
            .ConfigureServices((hostContext, services) =>
            {
                // Register our services
                services.AddSingleton<ITcpServer, TcpServer>();
                services.AddSingleton<ICommandProcessor, CommandProcessor>();
                
                // Register the hosted service that will run the TCP server
                services.AddHostedService<TcpServerHostedService>();
            });
}
