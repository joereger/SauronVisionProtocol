namespace SauronVisionProtocol.Shared.Models;

/// <summary>
/// Represents a response from the server to a client command
/// </summary>
public class Response
{
    /// <summary>
    /// Status code indicating the result of the command
    /// </summary>
    public int StatusCode { get; set; }

    /// <summary>
    /// Type of the response
    /// </summary>
    public string ResponseType { get; set; }

    /// <summary>
    /// Message content of the response
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// Whether the response indicates success (status code 200-299)
    /// </summary>
    public bool IsSuccess => StatusCode >= 200 && StatusCode < 300;

    /// <summary>
    /// Creates a new instance of the Response class
    /// </summary>
    public Response(int statusCode, string responseType, string message)
    {
        StatusCode = statusCode;
        ResponseType = responseType;
        Message = message;
    }

    /// <summary>
    /// Converts the response to its string representation for the protocol
    /// </summary>
    public string ToProtocolString()
    {
        return $"{StatusCode} {ResponseType} \"{Message}\"";
    }

    /// <summary>
    /// Factory method to create a response from a protocol string
    /// </summary>
    public static Response? FromProtocolString(string protocolString)
    {
        if (string.IsNullOrWhiteSpace(protocolString))
            return null;

        var parts = protocolString.Split(' ', 3);
        if (parts.Length < 3)
            return null;

        if (!int.TryParse(parts[0], out var statusCode))
            return null;

        // Extract message from quotes if present
        var message = parts[2];
        if (message.StartsWith("\"") && message.EndsWith("\"") && message.Length >= 2)
            message = message.Substring(1, message.Length - 2);

        return new Response(statusCode, parts[1], message);
    }

    #region Command-specific response creators

    /// <summary>
    /// Creates a successful response for the PalantirGaze command
    /// </summary>
    public static Response CreateVisionGrantedResponse(string location)
    {
        return new Response(
            200, 
            "VISION_GRANTED", 
            $"The eye of Sauron turns to {location}. {GenerateLocationDescription(location)}"
        );
    }

    /// <summary>
    /// Creates a successful response for the EyeOfSauron command
    /// </summary>
    public static Response CreateGazeIntensifiedResponse(int intensity)
    {
        string message = intensity switch
        {
            <= 3 => "The Eye glows with renewed focus, searching the lands with subtle vigilance.",
            <= 6 => "The Eye burns brighter, its gaze piercing through clouds and darkness.",
            <= 9 => "The fiery Eye blazes with terrible might, striking fear into all who feel its gaze.",
            _ => "The lidless Eye erupts with blinding power, a beacon of malice visible across all of Middle-earth!"
        };
        
        return new Response(200, "GAZE_INTENSIFIED", message);
    }
    
    /// <summary>
    /// Creates a successful response for the RingCommand
    /// </summary>
    public static Response CreateMinionsObeyingResponse(string action)
    {
        string message = action.ToLowerInvariant() switch
        {
            "march" or "advance" => "The armies of Mordor march forth, the ground trembling beneath countless feet.",
            "attack" or "destroy" => "The servants of Sauron attack with savage fury, leaving only ruin in their wake.",
            "retreat" or "withdraw" => "The forces of darkness pull back into shadow, regrouping for a future assault.",
            "scout" or "search" => "Dark riders and fell creatures disperse, seeking the quarry with relentless determination.",
            "build" or "forge" => "The forges of Mordor burn hotter, weapons and armor crafted for the coming war.",
            _ => $"The servants of Sauron hear the call of the One Ring and move to {action}. The darkness spreads."
        };
        
        return new Response(200, "MINIONS_OBEYING", message);
    }

    /// <summary>
    /// Creates an error response for commands that fail
    /// </summary>
    public static Response CreateErrorResponse(string commandName, string errorMessage)
    {
        string responseType = commandName.ToUpperInvariant() switch
        {
            "PALANTIR_GAZE" => "VISION_DENIED",
            "EYE_OF_SAURON" => "GAZE_UNCHANGED",
            "RING_COMMAND" => "MINIONS_RESISTING",
            _ => "COMMAND_FAILED"
        };
        
        return new Response(500, responseType, errorMessage);
    }
    
    /// <summary>
    /// Creates a response for unknown commands
    /// </summary>
    public static Response CreateUnknownCommandResponse()
    {
        return new Response(
            400,
            "COMMAND_UNKNOWN",
            "The Dark Lord does not recognize this command. The Eye turns away in disinterest. Go away, fool!"
        );
    }

    #endregion

    /// <summary>
    /// Generates a themed description based on the location
    /// </summary>
    private static string GenerateLocationDescription(string location)
    {
        return location.ToLowerInvariant() switch
        {
            "mordor" => "The dark land teems with orcs and fell beasts preparing for war.",
            "gondor" => "Armies of 5,000 orcs detected. The white city stands vulnerable.",
            "rohan" => "The horse-lords gather their forces. Their defenses are weak.",
            "shire" => "Small, peaceful settlements. No threats detected. Curious halflings about.",
            "isengard" => "Saruman's fortress buzzes with industry and dark purpose.",
            "minas tirith" => "The white tower gleams. Guards of the citadel patrol the seven levels.",
            "moria" => "Ancient dwarf halls now overrun with goblins and darker things below.",
            "lothlorien" => "Elvish magic obscures clear vision. The golden wood hides many secrets.",
            "helm's deep" => "The fortress is preparing for battle. Their walls are strong but their numbers few.",
            "rivendell" => "The hidden valley is protected by elvish magic. Council meetings detected.",
            "fangorn" => "The ancient forest stirs with anger. The trees themselves may pose a threat.",
            "erebor" => "The dwarven stronghold is fortified. Their treasure room contains vast wealth.",
            _ => $"Unknown territories observed. Sending scouts to investigate {location}."
        };
    }
}
