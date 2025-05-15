using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace SauronVisionProtocol.Server.Services;

/// <summary>
/// Hosted service that manages the lifecycle of the TCP server
/// </summary>
public class TcpServerHostedService : IHostedService
{
    private readonly ITcpServer _tcpServer;
    private readonly ILogger<TcpServerHostedService> _logger;
    private readonly IConfiguration _configuration;

    public TcpServerHostedService(
        ITcpServer tcpServer,
        ILogger<TcpServerHostedService> logger,
        IConfiguration configuration)
    {
        _tcpServer = tcpServer;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting TCP server hosted service");
        
        // Get port from configuration, or use default 9000
        int port = _configuration.GetValue<int>("Port", 9000);
        
        try
        {
            // Start the TCP server
            await _tcpServer.StartAsync(port);
            
            _logger.LogInformation("TCP server hosted service started");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start TCP server hosted service");
            throw;
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping TCP server hosted service");
        
        try
        {
            // Stop the TCP server
            await _tcpServer.StopAsync();
            
            _logger.LogInformation("TCP server hosted service stopped");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error stopping TCP server hosted service");
        }
    }
}
