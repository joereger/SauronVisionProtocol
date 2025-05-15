using System.Text.RegularExpressions;

// SauronVisionProtocol Console Client
Console.WriteLine("SauronVisionProtocol Console Client");
Console.WriteLine("===================================");

var client = new SauronVisionProtocol.ConsoleClient.ProtocolClient();

// Subscribe to events
client.ConnectionStatusChanged += (sender, status) => Console.WriteLine($"Connection status: {status}");
client.CommandSent += (sender, command) => Console.WriteLine($"Command sent: {command}");
client.ResponseReceived += (sender, response) => Console.WriteLine($"Response received: {response}");
client.LogMessageAdded += (sender, message) => Console.WriteLine(message);

// Main menu loop
bool exit = false;
while (!exit)
{
    Console.WriteLine("\nMenu:");
    Console.WriteLine("1. Connect to server");
    Console.WriteLine("2. Disconnect from server");
    Console.WriteLine("3. Send PALANTIR_GAZE command");
    Console.WriteLine("4. Exit");
    Console.Write("\nSelect an option: ");
    
    string? input = Console.ReadLine();
    
    switch (input)
    {
        case "1":
            await ConnectToServerAsync(client);
            break;
        case "2":
            DisconnectFromServer(client);
            break;
        case "3":
            await SendPalantirGazeCommandAsync(client);
            break;
        case "4":
            exit = true;
            break;
        default:
            Console.WriteLine("Invalid option, please try again.");
            break;
    }
}

// Clean up before exit
client.Disconnect();
Console.WriteLine("Disconnected from server. Goodbye!");

// Connection handler
async Task ConnectToServerAsync(SauronVisionProtocol.ConsoleClient.ProtocolClient client)
{
    Console.Write("Enter server address (default: 135.237.4.223): ");
    string? address = Console.ReadLine();
    if (string.IsNullOrWhiteSpace(address))
    {
        address = "135.237.4.223";
    }
    
    Console.Write("Enter port (default: 9000): ");
    string? portStr = Console.ReadLine();
    if (!int.TryParse(portStr, out int port) || string.IsNullOrWhiteSpace(portStr))
    {
        port = 9000;
    }
    
    Console.WriteLine($"Connecting to {address}:{port}...");
    bool success = await client.ConnectAsync(address, port);
    
    if (success)
    {
        Console.WriteLine("Successfully connected to server.");
    }
    else
    {
        Console.WriteLine("Failed to connect to server.");
    }
}

// Disconnect handler
void DisconnectFromServer(SauronVisionProtocol.ConsoleClient.ProtocolClient client)
{
    client.Disconnect();
    Console.WriteLine("Disconnected from server.");
}

// Send command handler
async Task SendPalantirGazeCommandAsync(SauronVisionProtocol.ConsoleClient.ProtocolClient client)
{
    if (!client.IsConnected)
    {
        Console.WriteLine("Not connected to server. Please connect first.");
        return;
    }
    
    Console.WriteLine("Available locations:");
    string[] locations = 
    { 
        "gondor", 
        "mordor", 
        "rohan", 
        "isengard", 
        "minas tirith", 
        "helm's deep", 
        "moria" 
    };
    
    for (int i = 0; i < locations.Length; i++)
    {
        Console.WriteLine($"{i + 1}. {locations[i]}");
    }
    
    Console.Write("Select a location (1-7): ");
    string? selection = Console.ReadLine();
    
    if (int.TryParse(selection, out int index) && index >= 1 && index <= locations.Length)
    {
        string location = locations[index - 1];
        Console.WriteLine($"Sending PALANTIR_GAZE command for location: {location}");
        
        string? response = await client.SendPalantirGazeCommandAsync(location);
        
        if (response != null)
        {
            Console.WriteLine("\nResponse analysis:");
            var match = Regex.Match(response, @"^(\d+)\s+(\w+)\s+""(.+)""$");
            if (match.Success)
            {
                string statusCode = match.Groups[1].Value;
                string responseType = match.Groups[2].Value;
                string message = match.Groups[3].Value;
                
                Console.WriteLine($"Status Code: {statusCode}");
                Console.WriteLine($"Response Type: {responseType}");
                Console.WriteLine($"Message: {message}");
            }
        }
    }
    else
    {
        Console.WriteLine("Invalid selection.");
    }
}
