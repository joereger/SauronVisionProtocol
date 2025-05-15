using System.Text;
using Microsoft.Extensions.Logging;
using SauronVisionProtocol.Shared.Models;

namespace SauronVisionProtocol.Server.Services;

/// <summary>
/// Processes commands from clients according to the SauronVisionProtocol
/// </summary>
public class CommandProcessor : ICommandProcessor
{
    private readonly ILogger<CommandProcessor> _logger;

    public CommandProcessor(ILogger<CommandProcessor> logger)
    {
        _logger = logger;
    }

    public Task<string> ProcessCommandAsync(string commandString)
    {
        _logger.LogInformation("Processing command: {Command}", commandString);
        
        // Parse the command
        var command = Command.FromProtocolString(commandString);
        
        if (command == null)
        {
            _logger.LogWarning("Invalid command format: {Command}", commandString);
            return Task.FromResult(Response.CreateErrorResponse("UNKNOWN_COMMAND", "Invalid command format").ToProtocolString());
        }
        
        // Process based on command type
        Response response = command switch
        {
            PalantirGazeCommand palantirGazeCommand => ProcessPalantirGazeCommand(palantirGazeCommand),
            _ => Response.CreateErrorResponse(command.CommandName, $"Unsupported command: {command.CommandName}")
        };
        
        _logger.LogInformation("Generated response: {Response}", response.ToProtocolString());
        
        return Task.FromResult(response.ToProtocolString());
    }

    private Response ProcessPalantirGazeCommand(PalantirGazeCommand command)
    {
        _logger.LogInformation("Processing PALANTIR_GAZE command for location: {Location}", command.Location);
        
        // Check if the location is empty or invalid
        if (string.IsNullOrWhiteSpace(command.Location))
        {
            return Response.CreateErrorResponse(command.CommandName, "Location cannot be empty");
        }
        
        // Generate vision response for the location
        return Response.CreateVisionGrantedResponse(command.Location);
    }
}
