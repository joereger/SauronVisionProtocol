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
            // Use the new specific method for invalid format
            return Task.FromResult(Response.CreateUnknownCommandFormatResponse().ToProtocolString());
        }
        
        Response response;
        try
        {
            // Process based on command type
            response = command switch
            {
                PalantirGazeCommand palantirGazeCommand => ProcessPalantirGazeCommand(palantirGazeCommand),
                EyeOfSauronCommand eyeOfSauronCommand => ProcessEyeOfSauronCommand(eyeOfSauronCommand),
                RingCommand ringCommand => ProcessRingCommand(ringCommand),
                // Handle cases where Command.FromProtocolString might return a base Command type or an unknown derived type
                _ => Response.CreateErrorResponse(command.CommandName, $"The command '{command.CommandName}' is recognized but not yet fully wielded by the server.", 404) // 404 Not Found or 501 Not Implemented
            };
        }
        catch (ArgumentException ex) // Catch specific parameter validation errors
        {
            _logger.LogWarning(ex, "Invalid parameter for command {CommandName}: {ErrorMessage}", command.CommandName, ex.Message);
            response = Response.CreateErrorResponse(command.CommandName, ex.Message); // Defaults to 400
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error processing command {CommandString}", commandString);
            response = Response.CreateErrorResponse("INTERNAL_SERVER_ERROR", "A shadow falls upon the server; an unexpected error occurred."); // This will set statusCode to 500
        }
        
        _logger.LogInformation("Generated response: {Response}", response.ToProtocolString());
        return Task.FromResult(response.ToProtocolString());
    }

    private Response ProcessPalantirGazeCommand(PalantirGazeCommand command)
    {
        _logger.LogInformation("Processing PALANTIR_GAZE command for location: {Location}", command.Location);
        
        if (string.IsNullOrWhiteSpace(command.Location) || command.Location.Equals("unknown", StringComparison.OrdinalIgnoreCase))
        {
            // Using ArgumentException for parameter validation, which will be caught above
            throw new ArgumentException("A specific location must be named for the Palantir's gaze.");
        }
        
        return Response.CreateVisionGrantedResponse(command.Location);
    }

    private Response ProcessEyeOfSauronCommand(EyeOfSauronCommand command)
    {
        _logger.LogInformation("Processing EYE_OF_SAURON command with intensity: {Intensity}", command.Intensity);

        if (command.Intensity < 1 || command.Intensity > 10)
        {
            throw new ArgumentException("The Eye's intensity must be a value between 1 and 10.");
        }

        // For now, just acknowledge. Actual logic for different intensities can be added.
        return Response.CreateGazeIntensifiedResponse(command.Intensity);
    }

    private Response ProcessRingCommand(RingCommand command)
    {
        _logger.LogInformation("Processing RING_COMMAND with action: {Action}", command.Action);

        if (string.IsNullOrWhiteSpace(command.Action) || command.Action.Equals("assemble", StringComparison.OrdinalIgnoreCase))
        {
            // Default action "assemble" might require specific handling or be disallowed if parameters are expected
            throw new ArgumentException("A specific action must be commanded of the minions.");
        }
        
        // For now, just acknowledge. Actual logic for different actions can be added.
        return Response.CreateMinionsObeyingResponse(command.Action);
    }
}
