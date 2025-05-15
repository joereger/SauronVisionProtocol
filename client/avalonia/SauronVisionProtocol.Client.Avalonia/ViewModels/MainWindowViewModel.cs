using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SauronVisionProtocol.Client.Avalonia.Models;
using SauronVisionProtocol.Client.Avalonia.Services;

namespace SauronVisionProtocol.Client.Avalonia.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    private readonly IProtocolClientService _protocolClient;
    
    // Connection properties
    [ObservableProperty]
    private string _serverHost = "sauronvisionprotocol.eastus.cloudapp.azure.com";

    [ObservableProperty]
    private string _serverPort = "9000";

    [ObservableProperty]
    private bool _isConnected;

    [ObservableProperty]
    private string _connectionStatus = "Disconnected";
    
    // UI properties
    [ObservableProperty]
    private string _eyeImageSource = "avares://SauronVisionProtocol.Client.Avalonia/Assets/Images/eye-disconnected.gif";

    // Command properties
    [ObservableProperty]
    private string _commandText = "";

    [ObservableProperty]
    private string _commandOutputText = "Welcome to Sauron Vision Protocol client\nReady to connect...";

    [ObservableProperty]
    private string _statusText = "Ready";

    [ObservableProperty]
    private Command? _selectedCommand;

    // Command collection
    public ObservableCollection<Command> AvailableCommands { get; } = new();

    // Constructor with dependency injection
    public MainWindowViewModel(IProtocolClientService protocolClient)
    {
        _protocolClient = protocolClient ?? throw new ArgumentNullException(nameof(protocolClient));
        
        // Initialize with available commands
        AvailableCommands.Add(new Command { Name = "PALANTIR_GAZE", Description = "Directs the Eye of Sauron's gaze to a location" });
        AvailableCommands.Add(new Command { Name = "EYE_OF_SAURON", Description = "Controls the intensity of the gaze" });
        AvailableCommands.Add(new Command { Name = "RING_COMMAND", Description = "Commands minions to perform actions" });
        
        // Subscribe to protocol client events
        _protocolClient.Connected += OnClientConnected;
        _protocolClient.Disconnected += OnClientDisconnected;
        _protocolClient.ResponseReceived += OnResponseReceived;
        
        // Update UI connection status
        IsConnected = _protocolClient.IsConnected;
        ConnectionStatus = IsConnected ? "Connected" : "Disconnected";
    }
    
    private void OnClientConnected(object? sender, EventArgs e)
    {
        IsConnected = true;
        ConnectionStatus = "Connected";
        StatusText = "Connected to server";
        CommandOutputText += "\nSuccessfully connected!";
        EyeImageSource = "avares://SauronVisionProtocol.Client.Avalonia/Assets/Images/eye-connected.gif";
    }
    
    private void OnClientDisconnected(object? sender, EventArgs e)
    {
        IsConnected = false;
        ConnectionStatus = "Disconnected";
        StatusText = "Disconnected from server";
        CommandOutputText += "\nDisconnected from server.";
        EyeImageSource = "avares://SauronVisionProtocol.Client.Avalonia/Assets/Images/eye-disconnected.gif";
    }
    
    private void OnResponseReceived(object? sender, ResponseReceivedEventArgs e)
    {
        CommandOutputText += $"\n{e.Response}";
        StatusText = "Response received";
    }

    // Connect command
    [RelayCommand]
    private async Task Connect()
    {
        try
        {
            StatusText = "Connecting...";
            CommandOutputText += $"\nAttempting to connect to {ServerHost}:{ServerPort}...";
            
            if (!int.TryParse(ServerPort, out int port))
            {
                StatusText = "Invalid port number";
                CommandOutputText += "\nError: Port must be a valid number";
                return;
            }
            
            await _protocolClient.ConnectAsync(ServerHost, port);
        }
        catch (Exception ex)
        {
            StatusText = "Connection error";
            CommandOutputText += $"\nError connecting: {ex.Message}";
            IsConnected = false;
            ConnectionStatus = "Disconnected";
        }
    }

    // Disconnect command
    [RelayCommand]
    private void Disconnect()
    {
        try
        {
            StatusText = "Disconnecting...";
            CommandOutputText += "\nDisconnecting from server...";
            _protocolClient.Disconnect();
        }
        catch (Exception ex)
        {
            StatusText = "Error during disconnect";
            CommandOutputText += $"\nError disconnecting: {ex.Message}";
        }
    }

    // Send command
    [RelayCommand]
    private async Task SendCommand()
    {
        if (!_protocolClient.IsConnected)
        {
            StatusText = "Not connected to server";
            return;
        }

        if (string.IsNullOrWhiteSpace(CommandText))
        {
            StatusText = "Command cannot be empty";
            return;
        }

        try
        {
            StatusText = "Sending command...";
            CommandOutputText += $"\n> {CommandText}";
            
            // Parse command and parameters (simple space-separated parsing)
            string[] parts = CommandText.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            string commandName = parts.Length > 0 ? parts[0] : string.Empty;
            string[] parameters = parts.Length > 1 ? parts[1..] : Array.Empty<string>();
            
            var command = new Command 
            { 
                Name = commandName,
                Parameters = parameters,
                Timestamp = DateTime.UtcNow
            };
            
            await _protocolClient.SendCommandAsync(command);
            
            // Clear command text after sending
            CommandText = "";
        }
        catch (Exception ex)
        {
            StatusText = "Error sending command";
            CommandOutputText += $"\nError: {ex.Message}";
        }
    }

    // Handle selection changed
    partial void OnSelectedCommandChanged(Command? value)
    {
        if (value != null)
        {
            CommandText = value.Name;
        }
    }
}
