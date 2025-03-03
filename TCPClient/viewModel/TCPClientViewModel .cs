using chiffrement_C_sar;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SuperSimpleTcp;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Windows;
using TCPClient.model.BL;
using TCPClient.model.DAL;

namespace TCPClient.viewModel
{
    public partial class TCPClientViewModel : ObservableObject
    {
        private const int ROT_KEY = 55;
        private SimpleTcpClient client;
        private LogContext _logContext;


        [ObservableProperty]
        private string? _ipAddress = "127.0.0.1:9000";

        [ObservableProperty]
        private string? _message;

        [ObservableProperty]
        private string? _info;

        [ObservableProperty]
        private bool _isSendEnabled = false;

        [ObservableProperty]
        private bool _isConnectEnabled = true;
        public ObservableCollection<string> LogMessages { get; set; } = new ObservableCollection<string>();


        public TCPClientViewModel()
        {
            client = new SimpleTcpClient(_ipAddress);
            client.Events.Connected += Events_Connected;
            client.Events.Disconnected += Events_Disconnected;
            client.Events.DataReceived += Events_DataReceived;
            _logContext = new LogContext();
            LoadLogs();

        }

        private void LogMessage(string message)
        {
            var log = new Log
            {
                Message = message,
                Timestamp = DateTime.Now
            };

            _logContext.Logs.Add(log);
            _logContext.SaveChanges();
        }
        private void LoadLogs()
        {
            var logs = _logContext.Logs.OrderByDescending(log => log.Timestamp).Take(10).ToList();
            foreach (var log in logs)
            {
                LogMessages.Add($"{log.Timestamp}: {log.Message}");
            }
        }

        // Call this whenever you want to log a message
        private void Events_DataReceived(object? sender, SuperSimpleTcp.DataReceivedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                string encryptedMessage = Encoding.UTF8.GetString(e.Data);
                string decryptedMessage = Chiffrement_C_sar.RotString(encryptedMessage, -ROT_KEY); // Decrypt
                Info += $"Other Client: {decryptedMessage}\n";

                // Log the message in the database
                LogMessage(decryptedMessage);
            });
        }

        private void Events_Connected(object? sender, ConnectionEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Info += "Server connected\n";

                // Log the connection
                LogMessage("Server connected");
            });
        }

        private void Events_Disconnected(object sender, ConnectionEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                Info += "Server disconnected\n";

                // Log the disconnection
                LogMessage("Server disconnected");
            });
        }

        // Command to connect to the server
        [RelayCommand]
        public void ConnectToServer()
        {
            try
            {
                client.Connect();
                IsSendEnabled = true;
                IsConnectEnabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Command to send a message
        [RelayCommand]
        public void SendMessage()
        {
            if (client.IsConnected)
            {
                if (!string.IsNullOrEmpty(Message))
                {
                    string encryptedMessage = Chiffrement_C_sar.RotString(Message, ROT_KEY);
                    client.Send(encryptedMessage);
                    Info += $"Me: {Message}\n";
                    Message = string.Empty;
                }
            }
        }
    }
}
