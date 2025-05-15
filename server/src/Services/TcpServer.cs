using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Microsoft.Extensions.Logging;

namespace SauronVisionProtocol.Server.Services;

/// <summary>
/// Implementation of the TCP server
/// </summary>
public class TcpServer : ITcpServer, IDisposable
{
    private readonly ILogger<TcpServer> _logger;
    private readonly ICommandProcessor _commandProcessor;
    private TcpListener? _listener;
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private readonly ConcurrentDictionary<Guid, TcpClientConnection> _clients = new();
    private bool _isDisposed;

    public event EventHandler<TcpClientConnectedEventArgs>? ClientConnected;
    public event EventHandler<TcpClientDisconnectedEventArgs>? ClientDisconnected;
    public event EventHandler<TcpDataReceivedEventArgs>? DataReceived;

    public TcpServer(ILogger<TcpServer> logger, ICommandProcessor commandProcessor)
    {
        _logger = logger;
        _commandProcessor = commandProcessor;
    }

    public async Task StartAsync(int port)
    {
        _logger.LogInformation("Starting TCP server on port {Port}", port);
        
        try
        {
            _listener = new TcpListener(IPAddress.Any, port);
            _listener.Start();
            
            _logger.LogInformation("TCP server started and listening on port {Port}", port);

            // Start accepting client connections
            _ = AcceptClientsAsync(_cancellationTokenSource.Token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start TCP server");
            throw;
        }
    }

    public async Task StopAsync()
    {
        _logger.LogInformation("Stopping TCP server");
        
        _cancellationTokenSource.Cancel();
        
        // Close the listener
        _listener?.Stop();
        
        // Disconnect all clients
        foreach (var clientConnection in _clients.Values)
        {
            await DisconnectClientAsync(clientConnection.ClientId);
        }
        
        _logger.LogInformation("TCP server stopped");
    }

    private async Task AcceptClientsAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                if (_listener == null)
                {
                    _logger.LogError("TCP listener is null");
                    break;
                }
                
                var tcpClient = await _listener.AcceptTcpClientAsync();
                
                // Generate a unique ID for this client
                var clientId = Guid.NewGuid();
                var remoteEndPoint = tcpClient.Client.RemoteEndPoint?.ToString() ?? "unknown";
                
                // Create a new client connection
                var clientConnection = new TcpClientConnection(clientId, tcpClient);
                
                // Add to the clients dictionary
                if (_clients.TryAdd(clientId, clientConnection))
                {
                    _logger.LogInformation("Client {ClientId} connected from {RemoteEndPoint}", clientId, remoteEndPoint);
                    
                    // Raise event
                    ClientConnected?.Invoke(this, new TcpClientConnectedEventArgs(clientId, remoteEndPoint));
                    
                    // Start a task to handle this client's messages
                    _ = HandleClientAsync(clientConnection, cancellationToken);
                }
                else
                {
                    _logger.LogWarning("Failed to add client {ClientId} to clients dictionary", clientId);
                    tcpClient.Dispose();
                }
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "Error accepting client connection");
                
                // Wait a bit before trying again to prevent tight loop
                await Task.Delay(1000, cancellationToken);
            }
        }
    }

    private async Task HandleClientAsync(TcpClientConnection clientConnection, CancellationToken cancellationToken)
    {
        try
        {
            await using var stream = clientConnection.TcpClient.GetStream();
            using var reader = new StreamReader(stream, Encoding.UTF8);
            using var writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true };

            // Send a welcome message
            await writer.WriteLineAsync("WELCOME SAURON_VISION_PROTOCOL");
            
            while (!cancellationToken.IsCancellationRequested)
            {
                var commandString = await reader.ReadLineAsync(cancellationToken);
                
                if (string.IsNullOrEmpty(commandString))
                {
                    // Client disconnected
                    break;
                }
                
                _logger.LogInformation("Received command from client {ClientId}: {Command}", 
                    clientConnection.ClientId, commandString);
                
                // Raise event
                DataReceived?.Invoke(this, new TcpDataReceivedEventArgs(clientConnection.ClientId, commandString));
                
                // Process the command
                var response = await _commandProcessor.ProcessCommandAsync(commandString);
                
                // Send the response
                await writer.WriteLineAsync(response);
                
                _logger.LogInformation("Sent response to client {ClientId}: {Response}", 
                    clientConnection.ClientId, response);
            }
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.LogError(ex, "Error handling client {ClientId}", clientConnection.ClientId);
        }
        finally
        {
            // Disconnect the client
            await DisconnectClientAsync(clientConnection.ClientId);
        }
    }

    private async Task DisconnectClientAsync(Guid clientId)
    {
        if (_clients.TryRemove(clientId, out var clientConnection))
        {
            _logger.LogInformation("Disconnecting client {ClientId}", clientId);
            
            try
            {
                clientConnection.TcpClient.Close();
                clientConnection.TcpClient.Dispose();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error disposing client {ClientId}", clientId);
            }
            
            // Raise event
            ClientDisconnected?.Invoke(this, new TcpClientDisconnectedEventArgs(clientId));
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_isDisposed) return;
        
        if (disposing)
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            _listener?.Stop();
            
            foreach (var clientConnection in _clients.Values)
            {
                clientConnection.TcpClient.Dispose();
            }
            
            _clients.Clear();
        }
        
        _isDisposed = true;
    }
}

/// <summary>
/// Represents a client connection
/// </summary>
internal class TcpClientConnection
{
    public Guid ClientId { get; }
    public TcpClient TcpClient { get; }
    
    public TcpClientConnection(Guid clientId, TcpClient tcpClient)
    {
        ClientId = clientId;
        TcpClient = tcpClient;
    }
}
