using System;
using System.Threading.Tasks;
using SauronVisionProtocol.Client.Avalonia.Models;

namespace SauronVisionProtocol.Client.Avalonia.Services;

/// <summary>
/// A mock implementation of the IProtocolClientService for development and testing.
/// </summary>
public class MockProtocolClientService : IProtocolClientService
{
    private Random _random = new Random();
    private bool _isConnected;
    
    /// <inheritdoc />
    public event EventHandler? Connected;
    
    /// <inheritdoc />
    public event EventHandler? Disconnected;
    
    /// <inheritdoc />
    public event EventHandler<ResponseReceivedEventArgs>? ResponseReceived;
    
    /// <inheritdoc />
    public bool IsConnected => _isConnected;
    
    /// <inheritdoc />
    public async Task ConnectAsync(string host, int port)
    {
        // Simulate network delay
        await Task.Delay(1000);
        
        // Simulate successful connection
        _isConnected = true;
        Connected?.Invoke(this, EventArgs.Empty);
    }
    
    /// <inheritdoc />
    public void Disconnect()
    {
        if (!_isConnected)
            return;
        
        _isConnected = false;
        Disconnected?.Invoke(this, EventArgs.Empty);
    }
    
    /// <inheritdoc />
    public async Task SendCommandAsync(Command command)
    {
        if (!_isConnected)
            throw new InvalidOperationException("Cannot send command when not connected.");
        
        // Simulate network delay
        await Task.Delay(_random.Next(300, 800));
        
        // Generate a mock response based on the command
        string responseText;
        bool isSuccess;
        string responseType;
        string message;
        
        switch (command.Name)
        {
            case "PALANTIR_GAZE":
                isSuccess = true;
                responseType = "VISION_GRANTED";
                var locations = new[] { "gondor", "mordor", "isengard", "helm's deep", "minas tirith", "rivendell" };
                var location = command.Parameters.Length > 0 
                    ? command.Parameters[0] 
                    : locations[_random.Next(locations.Length)];
                    
                var troopCounts = new[] { "3,000", "5,000", "10,000", "15,000", "20,000" };
                var troopCount = troopCounts[_random.Next(troopCounts.Length)];
                
                message = $"The eye of Sauron turns to {location}. Armies of {troopCount} orcs detected.";
                
                if (location.Equals("gondor", StringComparison.OrdinalIgnoreCase))
                    message += " The white city stands vulnerable.";
                else if (location.Equals("helm's deep", StringComparison.OrdinalIgnoreCase))
                    message += " The fortress is preparing for battle.";
                else
                    message += " The minions of Sauron await command.";
                break;
                
            case "EYE_OF_SAURON":
                isSuccess = _random.Next(10) > 2; // 80% success rate
                responseType = isSuccess ? "GAZE_INTENSIFIED" : "GAZE_UNCHANGED";
                
                if (isSuccess)
                    message = "The Eye of Sauron burns brighter, its gaze piercing through clouds and darkness.";
                else
                    message = "The Eye remains fixed, unmoved by your command. It has found something of interest elsewhere.";
                break;
                
            case "RING_COMMAND":
                isSuccess = _random.Next(10) > 3; // 70% success rate
                responseType = isSuccess ? "MINIONS_OBEYING" : "MINIONS_RESISTING";
                
                if (isSuccess)
                    message = "The servants of Sauron hear the call of the One Ring and move to obey. The darkness spreads.";
                else
                    message = "The command echoes in the void. The minions are distracted, fighting among themselves.";
                break;
                
            default:
                isSuccess = false;
                responseType = "COMMAND_UNKNOWN";
                message = "The Dark Lord does not recognize this command. The Eye turns away in disinterest.";
                break;
        }
        
        // Format the response in the protocol format
        responseText = $"{(isSuccess ? "200" : "500")} {responseType} \"{message}\"";
        
        // Raise the event
        ResponseReceived?.Invoke(this, new ResponseReceivedEventArgs(responseText, isSuccess, responseType, message));
    }
}
