using InfinityServer.Classes.Server.PacketSystem;
using InfinityServer.Classes.Server.Security;
using System;
using System.Collections.Concurrent;
using System.Net;
using System.Windows.Forms;
using WatsonTcp;

namespace InfinityServer.Classes.Server
{
    public class InfinityTcpServer
    {
        private WatsonTcpServer _tcpServer;
        private WatsonTcpServerSettings _tcpServerSettings;
        private PacketHandler _serverPacketHandler;

        private ConcurrentDictionary<Guid, ClientMetadata> _connectedClients;
        private ConcurrentDictionary<Guid, ClientHandler> _connectedClientHandlers;

        private KeyManager _serverKeyManager;

        private Guid _serverGuid;

        private string _host;
        private int _port;

        private bool _isServerRunning = false;


        public WatsonTcpServer GetTcpServer => _tcpServer;
        public PacketHandler GetServerPacketHandler => _serverPacketHandler;
        public KeyManager GetServerKeyManager => _serverKeyManager;
        public ConcurrentDictionary<Guid, ClientMetadata> GetConnectedClients => _connectedClients;

        public Guid GetServerGuid => _serverGuid;

        public InfinityTcpServer(int port)
        {
            _host = IPAddress.Any.ToString();
            _port = port;

            _serverGuid = Guid.NewGuid();

            _tcpServer = new WatsonTcpServer(_host, _port);
            _tcpServerSettings = new WatsonTcpServerSettings();
            _serverPacketHandler = new PacketHandler();
            _serverKeyManager = new KeyManager();

            _connectedClients = new ConcurrentDictionary<Guid, ClientMetadata>();
            _connectedClientHandlers = new ConcurrentDictionary<Guid, ClientHandler>();

            _tcpServer.Events.ClientConnected += ClientConnected;
            _tcpServer.Events.ClientDisconnected += ClientDisconnected;
            _tcpServer.Events.StreamReceived += StreamReceived;
        }

        public void StartServer()
        {
            if (_isServerRunning)
            {
                MessageBox.Show("Server Is Already Running.");
                return;
            }

            _tcpServer.Start();

            _isServerRunning = true;
        }

        public void StopServer()
        {
            if (!_isServerRunning)
            {
                MessageBox.Show("Server Is Not Running Already.");
                return;
            }

            _tcpServer.Stop();

            _isServerRunning = false;
        }

        private void ClientConnected(object sender, ConnectionEventArgs e)
        {
            Guid clientGuid = e.Client.Guid;

            ClientHandler clientHandler = new ClientHandler(this, clientGuid);

            _connectedClients.TryAdd(clientGuid, e.Client);
            _connectedClientHandlers.TryAdd(clientGuid, clientHandler);

            clientHandler.ClientConnected(e);
        }

        private void StreamReceived(object sender, StreamReceivedEventArgs e)
        {
            ClientHandler clientHandler = _connectedClientHandlers[e.Client.Guid];

            clientHandler.StreamReceived(sender, e);
        }

        private void ClientDisconnected(object sender, DisconnectionEventArgs e)
        {
            _connectedClients.TryRemove(e.Client.Guid, out _);
            _connectedClientHandlers.TryRemove(e.Client.Guid, out _);
        }
    }
}
