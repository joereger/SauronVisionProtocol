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
