namespace SauronVisionProtocol.Server.Services;

/// <summary>
/// Interface for processing commands received from clients
/// </summary>
public interface ICommandProcessor
{
    /// <summary>
    /// Process a command string and return a response
    /// </summary>
    /// <param name="commandString">Command string from client</param>
    /// <returns>Response string to send back to client</returns>
    Task<string> ProcessCommandAsync(string commandString);
}
