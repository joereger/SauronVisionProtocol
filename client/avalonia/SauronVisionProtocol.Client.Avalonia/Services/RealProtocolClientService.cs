using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using SauronVisionProtocol.Client.Avalonia.Models; // Assuming Command model is here

namespace SauronVisionProtocol.Client.Avalonia.Services
{
    public class RealProtocolClientService : IProtocolClientService, IDisposable
    {
        private TcpClient? _tcpClient;
        private NetworkStream? _stream;
        private StreamReader? _reader;
        private StreamWriter? _writer;
        private Task? _receiveLoopTask;
        private bool _isTryingToConnect = false;

        public event EventHandler? Connected;
        public event EventHandler? Disconnected;
        public event EventHandler<ResponseReceivedEventArgs>? ResponseReceived;

        public bool IsConnected => _tcpClient?.Connected ?? false;

        public async Task ConnectAsync(string host, int port)
        {
            if (IsConnected || _isTryingToConnect) return;

            _isTryingToConnect = true;
            try
            {
                _tcpClient = new TcpClient();
                await _tcpClient.ConnectAsync(host, port);
                _stream = _tcpClient.GetStream();
                _reader = new StreamReader(_stream, Encoding.UTF8);
                _writer = new StreamWriter(_stream, Encoding.UTF8) { AutoFlush = true };

                Connected?.Invoke(this, EventArgs.Empty);
                _receiveLoopTask = Task.Run(ReceiveLoop);
            }
            catch (Exception ex)
            {
                // Handle connection error, maybe raise an event or log
                Console.WriteLine($"Connection error: {ex.Message}"); // Basic logging
                Disconnect(); // Ensure cleanup
                throw; // Re-throw to notify the caller (e.g., ViewModel)
            }
            finally
            {
                _isTryingToConnect = false;
            }
        }

        public void Disconnect()
        {
            if (!IsConnected && _tcpClient == null) return;

            try
            {
                _reader?.Close();
                _writer?.Close();
                _stream?.Close();
                _tcpClient?.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Disconnection error: {ex.Message}"); // Basic logging
            }
            finally
            {
                _reader = null;
                _writer = null;
                _stream = null;
                _tcpClient = null;
                Disconnected?.Invoke(this, EventArgs.Empty);
            }
        }

        public async Task SendCommandAsync(Command command)
        {
            if (!IsConnected || _writer == null)
                throw new InvalidOperationException("Not connected to server.");

            // Convert Command object to protocol string
            // This assumes Command has a ToProtocolString() method or similar
            // For now, let's assume a simple format: COMMAND_NAME PARAM1 PARAM2...
            string commandString = command.Name;
            if (command.Parameters != null && command.Parameters.Length > 0)
            {
                commandString += " " + string.Join(" ", command.Parameters);
            }
            
            await _writer.WriteLineAsync(commandString);
        }

        private async Task ReceiveLoop()
        {
            try
            {
                while (IsConnected && _reader != null)
                {
                    string? responseLine = await _reader.ReadLineAsync();
                    if (responseLine != null)
                    {
                        // Basic parsing of "STATUS_CODE TYPE "MESSAGE""
                        // This is a simplified parser and might need to be more robust
                        string[] parts = responseLine.Split(new[] { ' ' }, 3);
                        bool success = parts.Length > 0 && parts[0] == "200";
                        string responseType = parts.Length > 1 ? parts[1] : "UNKNOWN";
                        string message = parts.Length > 2 ? parts[2].Trim('"') : "No message";

                        ResponseReceived?.Invoke(this, new ResponseReceivedEventArgs(responseLine, success, responseType, message));
                    }
                    else
                    {
                        // Stream closed by server
                        Disconnect();
                        break;
                    }
                }
            }
            catch (IOException)
            {
                // Likely connection lost
                Disconnect();
            }
            catch (ObjectDisposedException)
            {
                // Client was disposed, probably during disconnect
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Receive loop error: {ex.Message}");
                Disconnect();
            }
        }

        public void Dispose()
        {
            Disconnect();
            GC.SuppressFinalize(this);
        }
    }
}
