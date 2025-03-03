using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SuperSimpleTcp;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;  // Required for WPF components such as Application and Dispatcher

namespace TCPServer.viewModel
{
    public partial class TCPServerViewModel : ObservableObject
    {
        private SimpleTcpServer? server;

        private const int ROT_KEY = 55;

        [ObservableProperty]
        private string? _messageInfo;

        [ObservableProperty]
        private string? _serverIP;

        [ObservableProperty]
        private string? _message;

        [ObservableProperty]
        private bool _isSendButtonEnabled;

        [ObservableProperty]
        private bool _isStartButtonEnabled;

        public ObservableCollection<string> ClientIPList { get; } = new ObservableCollection<string>();

        public TCPServerViewModel()
        {
            ServerIP = "127.0.0.1:9000";  // Default IP and port
            IsStartButtonEnabled = true;
            IsSendButtonEnabled = false;
        }

        [RelayCommand]
        public void StartServer()
        {
            if (server == null)
            {
                server = new SimpleTcpServer(ServerIP);
                server.Events.ClientConnected += Events_ClientConnected;
                server.Events.ClientDisconnected += Events_ClientDisconnected;
                server.Events.DataReceived += Events_DataReceived;
                server.Start();

                MessageInfo += "Server started...\n";
                IsStartButtonEnabled = false;
                IsSendButtonEnabled = true;
            }
        }

        [RelayCommand]
        public void SendMessage()
        {
            if (server != null && !string.IsNullOrWhiteSpace(Message))
            {
                string encryptedMessage = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(Message));
                foreach (var client in server.GetClients())
                {
                    server.Send(client, encryptedMessage);
                }
                Message = string.Empty; // Clear message field
            }
        }

        private void Events_DataReceived(object? sender, DataReceivedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                string encryptedMessage = Encoding.UTF8.GetString(e.Data);
                MessageInfo += $"{e.IpPort}: {encryptedMessage}\n";

                // Forward the received (already encrypted) message to other clients
                foreach (var client in server.GetClients())
                {
                    if (client != e.IpPort)
                    {
                        server.Send(client, encryptedMessage);
                    }
                }
            });
        }

        private void Events_ClientDisconnected(object? sender, ConnectionEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                MessageInfo += $"{e.IpPort}: disconnected\n";
                ClientIPList.Remove(e.IpPort);
            });
        }

        private void Events_ClientConnected(object? sender, ConnectionEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                MessageInfo += $"{e.IpPort} connected\n";
                ClientIPList.Add(e.IpPort);
            });
        }
    }
}
