namespace SauronVisionProtocol.Server.Services;

/// <summary>
/// Interface for the TCP server component
/// </summary>
public interface ITcpServer
{
    /// <summary>
    /// Start the TCP server on the specified port
    /// </summary>
    /// <param name="port">Port to listen on</param>
    Task StartAsync(int port);
    
    /// <summary>
    /// Stop the TCP server
    /// </summary>
    Task StopAsync();
    
    /// <summary>
    /// Event raised when a new client connects
    /// </summary>
    event EventHandler<TcpClientConnectedEventArgs>? ClientConnected;
    
    /// <summary>
    /// Event raised when a client disconnects
    /// </summary>
    event EventHandler<TcpClientDisconnectedEventArgs>? ClientDisconnected;
    
    /// <summary>
    /// Event raised when data is received from a client
    /// </summary>
    event EventHandler<TcpDataReceivedEventArgs>? DataReceived;
}

/// <summary>
/// Event arguments for client connected event
/// </summary>
public class TcpClientConnectedEventArgs : EventArgs
{
    public Guid ClientId { get; }
    public string RemoteEndPoint { get; }
    
    public TcpClientConnectedEventArgs(Guid clientId, string remoteEndPoint)
    {
        ClientId = clientId;
        RemoteEndPoint = remoteEndPoint;
    }
}

/// <summary>
/// Event arguments for client disconnected event
/// </summary>
public class TcpClientDisconnectedEventArgs : EventArgs
{
    public Guid ClientId { get; }
    
    public TcpClientDisconnectedEventArgs(Guid clientId)
    {
        ClientId = clientId;
    }
}

/// <summary>
/// Event arguments for data received event
/// </summary>
public class TcpDataReceivedEventArgs : EventArgs
{
    public Guid ClientId { get; }
    public string Data { get; }
    
    public TcpDataReceivedEventArgs(Guid clientId, string data)
    {
        ClientId = clientId;
        Data = data;
    }
}
