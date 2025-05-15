namespace SauronVisionProtocol.Shared.Models;

/// <summary>
/// Base class for all SVP commands
/// </summary>
public abstract class Command
{
    /// <summary>
    /// The name of the command
    /// </summary>
    public abstract string CommandName { get; }

    /// <summary>
    /// Converts the command to its string representation for the protocol
    /// </summary>
    public virtual string ToProtocolString()
    {
        return CommandName;
    }

    /// <summary>
    /// Factory method to create a command from a protocol string
    /// </summary>
    public static Command? FromProtocolString(string protocolString)
    {
        if (string.IsNullOrWhiteSpace(protocolString))
            return null;

        string[] parts = protocolString.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0)
            return null;

        return parts[0].ToUpperInvariant() switch
        {
            "PALANTIR_GAZE" => parts.Length > 1 
                ? new PalantirGazeCommand { Location = parts[1] } 
                : new PalantirGazeCommand(),
            "EYE_OF_SAURON" => parts.Length > 1 
                ? new EyeOfSauronCommand { Intensity = int.TryParse(parts[1], out int intensity) ? intensity : 1 } 
                : new EyeOfSauronCommand(),
            "RING_COMMAND" => parts.Length > 1 
                ? new RingCommand { Action = string.Join(" ", parts.Skip(1)) } 
                : new RingCommand(),
            _ => null
        };
    }
}

/// <summary>
/// Command to direct the Eye of Sauron's gaze to a specific location
/// </summary>
public class PalantirGazeCommand : Command
{
    public override string CommandName => "PALANTIR_GAZE";
    
    public string Location { get; set; } = "unknown";

    public override string ToProtocolString()
    {
        return $"{CommandName} {Location}";
    }
}

/// <summary>
/// Command to control the intensity of the Eye of Sauron
/// </summary>
public class EyeOfSauronCommand : Command
{
    public override string CommandName => "EYE_OF_SAURON";
    
    /// <summary>
    /// The intensity level of the gaze, from 1 to 10
    /// </summary>
    public int Intensity { get; set; } = 1;

    public override string ToProtocolString()
    {
        return $"{CommandName} {Intensity}";
    }
}

/// <summary>
/// Command to direct minions to perform specific actions
/// </summary>
public class RingCommand : Command
{
    public override string CommandName => "RING_COMMAND";
    
    /// <summary>
    /// The action to be performed by the minions
    /// </summary>
    public string Action { get; set; } = "assemble";

    public override string ToProtocolString()
    {
        return $"{CommandName} {Action}";
    }
}
