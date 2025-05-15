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
    /// Creates an error response
    /// </summary>
    public static Response CreateErrorResponse(string errorMessage)
    {
        return new Response(
            500,
            "VISION_DENIED",
            errorMessage
        );
    }

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
            _ => $"Unknown territories observed. Sending scouts to investigate {location}."
        };
    }
}
