using System.Text.RegularExpressions;
using SauronVisionProtocol.Client.Services;

namespace SauronVisionProtocol.Client;

public partial class MainPage : ContentPage
{
	private readonly ProtocolClient protocolClient;
	private readonly Regex responseRegex = new Regex(@"^(\d+)\s+(\w+)\s+""(.+)""$");

	public MainPage()
	{
		InitializeComponent();
		
		// Initialize the protocol client
		protocolClient = new ProtocolClient();
		
		// Subscribe to client events
		protocolClient.ConnectionStatusChanged += OnConnectionStatusChanged;
		protocolClient.CommandSent += OnCommandSent;
		protocolClient.ResponseReceived += OnResponseReceived;
		protocolClient.LogMessageAdded += OnLogMessageAdded;
		
		// Initialize UI elements
		LocationPicker.SelectedIndex = 0;
		UpdateUIForConnectionState(false);
	}
	
	private void UpdateUIForConnectionState(bool isConnected)
	{
		ConnectButton.IsEnabled = !isConnected;
		DisconnectButton.IsEnabled = isConnected;
		SendCommandButton.IsEnabled = isConnected;
		LocationPicker.IsEnabled = isConnected;
		
		ConnectionStatusLabel.Text = isConnected ? "Connected" : "Disconnected";
		ConnectionStatusLabel.TextColor = isConnected ? Colors.Green : Colors.Red;
	}
	
	private async void OnConnectClicked(object sender, EventArgs e)
	{
		if (string.IsNullOrWhiteSpace(ServerAddressEntry.Text))
		{
			await DisplayAlert("Error", "Please enter a server address", "OK");
			return;
		}
		
		if (!int.TryParse(PortEntry.Text, out int port))
		{
			await DisplayAlert("Error", "Please enter a valid port number", "OK");
			return;
		}
		
		// Disable buttons during connection
		ConnectButton.IsEnabled = false;
		ConnectionStatusLabel.Text = "Connecting...";
		
		bool connected = await protocolClient.ConnectAsync(ServerAddressEntry.Text, port);
		
		if (!connected)
		{
			ConnectButton.IsEnabled = true;
			ConnectionStatusLabel.Text = "Connection failed";
			ConnectionStatusLabel.TextColor = Colors.Red;
		}
	}
	
	private void OnDisconnectClicked(object sender, EventArgs e)
	{
		protocolClient.Disconnect();
	}
	
	private async void OnSendCommandClicked(object sender, EventArgs e)
	{
		if (LocationPicker.SelectedItem == null)
		{
			await DisplayAlert("Error", "Please select a location", "OK");
			return;
		}
		
		string location = LocationPicker.SelectedItem.ToString()!;
		await protocolClient.SendPalantirGazeCommandAsync(location);
	}
	
	private void OnConnectionStatusChanged(object? sender, string status)
	{
		MainThread.BeginInvokeOnMainThread(() =>
		{
			ConnectionStatusLabel.Text = status;
			ConnectionStatusLabel.TextColor = status.StartsWith("Connected") ? Colors.Green : Colors.Red;
			UpdateUIForConnectionState(protocolClient.IsConnected);
		});
	}
	
	private void OnCommandSent(object? sender, string command)
	{
		MainThread.BeginInvokeOnMainThread(() =>
		{
			CommandSentLabel.Text = command;
		});
	}
	
	private void OnResponseReceived(object? sender, string response)
	{
		MainThread.BeginInvokeOnMainThread(() =>
		{
			RawResponseLabel.Text = response;
			
			// Parse the response using the regex
			var match = responseRegex.Match(response);
			if (match.Success)
			{
				string statusCode = match.Groups[1].Value;
				string responseType = match.Groups[2].Value;
				string message = match.Groups[3].Value;
				
				StatusLabel.Text = statusCode;
				StatusLabel.TextColor = statusCode == "200" ? Colors.Green : Colors.Red;
				
				ResponseTypeLabel.Text = responseType;
				MessageLabel.Text = message;
			}
			else
			{
				StatusLabel.Text = "Unknown";
				ResponseTypeLabel.Text = "Unknown";
				MessageLabel.Text = response;
			}
		});
	}
	
	private void OnLogMessageAdded(object? sender, string message)
	{
		MainThread.BeginInvokeOnMainThread(() =>
		{
			CommunicationLogLabel.Text += message + "\n";
		});
	}
}
