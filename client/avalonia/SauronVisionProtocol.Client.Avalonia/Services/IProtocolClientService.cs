using System;
using System.Threading.Tasks;
using SauronVisionProtocol.Client.Avalonia.Models;

namespace SauronVisionProtocol.Client.Avalonia.Services;

/// <summary>
/// Interface for the protocol client that communicates with the server.
/// </summary>
public interface IProtocolClientService
{
    /// <summary>
    /// Event raised when the client successfully connects to the server.
    /// </summary>
    event EventHandler Connected;
    
    /// <summary>
    /// Event raised when the client disconnects from the server.
    /// </summary>
    event EventHandler Disconnected;
    
    /// <summary>
    /// Event raised when a response is received from the server.
    /// </summary>
    event EventHandler<ResponseReceivedEventArgs> ResponseReceived;
    
    /// <summary>
    /// Gets a value indicating whether the client is currently connected to the server.
    /// </summary>
    bool IsConnected { get; }
    
    /// <summary>
    /// Asynchronously connects to the server.
    /// </summary>
    /// <param name="host">The host to connect to.</param>
    /// <param name="port">The port to connect to.</param>
    /// <returns>A task representing the connection operation.</returns>
    Task ConnectAsync(string host, int port);
    
    /// <summary>
    /// Disconnects from the server.
    /// </summary>
    void Disconnect();
    
    /// <summary>
    /// Asynchronously sends a command to the server.
    /// </summary>
    /// <param name="command">The command to send.</param>
    /// <returns>A task representing the send operation.</returns>
    Task SendCommandAsync(Command command);
}

/// <summary>
/// Event arguments for the ResponseReceived event.
/// </summary>
public class ResponseReceivedEventArgs : EventArgs
{
    /// <summary>
    /// Gets the response that was received.
    /// </summary>
    public string Response { get; }
    
    /// <summary>
    /// Gets a value indicating whether the response indicates success.
    /// </summary>
    public bool IsSuccess { get; }
    
    /// <summary>
    /// Gets the type of the response.
    /// </summary>
    public string ResponseType { get; }
    
    /// <summary>
    /// Gets the message content of the response.
    /// </summary>
    public string Message { get; }
    
    /// <summary>
    /// Initializes a new instance of the <see cref="ResponseReceivedEventArgs"/> class.
    /// </summary>
    public ResponseReceivedEventArgs(string response, bool isSuccess, string responseType, string message)
    {
        Response = response;
        IsSuccess = isSuccess;
        ResponseType = responseType;
        Message = message;
    }
}
