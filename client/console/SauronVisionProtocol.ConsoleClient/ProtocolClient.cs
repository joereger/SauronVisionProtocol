using System.Net.Sockets;
using System.Text;

namespace SauronVisionProtocol.ConsoleClient;

public class ProtocolClient
{
    private TcpClient? tcpClient;
    private NetworkStream? networkStream;
    private StreamReader? reader;
    private StreamWriter? writer;
    private bool isConnected;
    
    public event EventHandler<string>? ConnectionStatusChanged;
    public event EventHandler<string>? CommandSent;
    public event EventHandler<string>? ResponseReceived;
    public event EventHandler<string>? LogMessageAdded;
    
    public bool IsConnected => isConnected;
    
    public async Task<bool> ConnectAsync(string serverAddress, int port)
    {
        try
        {
            // Close existing connection if any
            Disconnect();
            
            // Create a new TCP client and connect
            tcpClient = new TcpClient();
            await tcpClient.ConnectAsync(serverAddress, port);
            
            // Get the network stream
            networkStream = tcpClient.GetStream();
            reader = new StreamReader(networkStream);
            writer = new StreamWriter(networkStream) { AutoFlush = true };
            
            // Read the welcome message
            string? welcomeMessage = await reader.ReadLineAsync();
            if (welcomeMessage != null)
            {
                LogMessageAdded?.Invoke(this, $"SERVER: {welcomeMessage}");
            }
            
            isConnected = true;
            ConnectionStatusChanged?.Invoke(this, "Connected");
            
            return true;
        }
        catch (Exception ex)
        {
            LogMessageAdded?.Invoke(this, $"ERROR: {ex.Message}");
            ConnectionStatusChanged?.Invoke(this, $"Connection failed: {ex.Message}");
            return false;
        }
    }
    
    public void Disconnect()
    {
        if (writer != null)
        {
            writer.Dispose();
            writer = null;
        }
        
        if (reader != null)
        {
            reader.Dispose();
            reader = null;
        }
        
        if (networkStream != null)
        {
            networkStream.Dispose();
            networkStream = null;
        }
        
        if (tcpClient != null)
        {
            tcpClient.Dispose();
            tcpClient = null;
        }
        
        isConnected = false;
        ConnectionStatusChanged?.Invoke(this, "Disconnected");
    }
    
    public async Task<string?> SendPalantirGazeCommandAsync(string location)
    {
        if (!isConnected || writer == null || reader == null)
        {
            LogMessageAdded?.Invoke(this, "ERROR: Not connected to server");
            return null;
        }
        
        try
        {
            // Format the command according to the protocol
            string command = $"PALANTIR_GAZE {location}";
            CommandSent?.Invoke(this, command);
            LogMessageAdded?.Invoke(this, $"CLIENT: {command}");
            
            // Send the command
            await writer.WriteLineAsync(command);
            
            // Read the response
            string? response = await reader.ReadLineAsync();
            if (response != null)
            {
                ResponseReceived?.Invoke(this, response);
                LogMessageAdded?.Invoke(this, $"SERVER: {response}");
                return response;
            }
            
            return null;
        }
        catch (Exception ex)
        {
            LogMessageAdded?.Invoke(this, $"ERROR: {ex.Message}");
            return null;
        }
    }
}
