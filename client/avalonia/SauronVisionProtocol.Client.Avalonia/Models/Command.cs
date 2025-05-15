using System;

namespace SauronVisionProtocol.Client.Avalonia.Models;

public class Command
{
    // Basic properties for command metadata
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    
    // Optional parameters specific to the command (can be extended later)
    public string[] Parameters { get; set; } = Array.Empty<string>();
    
    // For display convenience
    public override string ToString() => Name;
}
