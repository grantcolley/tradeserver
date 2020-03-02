using DipSocket.Messages;
using DipSocket.Server;
using Newtonsoft.Json;
using System;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Notification.Server
{
    public class ServerNotificationHub : DipSocketServer
    {
        private string serverChannelName;

        public ServerNotificationHub(ConnectionManager connectionManager, ChannelManager channelManager)
            : base(connectionManager, channelManager)
        {
        }

        public void SetServerChannelName(string serverName)
        {
            serverChannelName = serverName;
        }

        public async override Task OnClientConnectAsync(WebSocket websocket, string clientId, string data)
        {
            if (string.IsNullOrWhiteSpace(serverChannelName))
            {
                throw new ArgumentNullException("The server channel has not been set.");
            }

            if (string.IsNullOrWhiteSpace(clientId))
            {
                throw new ArgumentNullException("clientId cannot be null or empty.");
            }

            if (string.IsNullOrWhiteSpace(data))
            {
                throw new ArgumentNullException("The server name to subscribe to must be specified in the data parameter.");
            }

            if (!serverChannelName.Equals(data))
            {
                throw new ArgumentNullException($"The server name {data} in the client request does not match the server channel {serverChannelName}");
            }

            var connection = await base.AddWebSocketAsync(websocket).ConfigureAwait(false);

            connection.Name = clientId;

            SubscribeToChannel(serverChannelName, websocket);

            var connectionInfo = connection.GetConnectionInfo();

            var json = JsonConvert.SerializeObject(connectionInfo);

            var message = new Message { MethodName = "OnConnected", SenderConnectionId = "Server", Data = json };

            await SendMessageAsync(websocket, message).ConfigureAwait(false);
        }

        public async override Task ReceiveAsync(WebSocket webSocket, Message message)
        {
            try
            {
                switch (message.MessageType)
                {
                    case MessageType.UnsubscribeFromChannel:
                        UnsubscribeFromChannel(serverChannelName, webSocket);
                        break;
                }
            }
            catch (Exception ex)
            {
                var errorMessage = new Message { MethodName = message.MethodName, SenderConnectionId = message.SenderConnectionId, Data = $"{MessageType.UnsubscribeFromChannel} Error : {ex.Message}" };
                await SendMessageAsync(webSocket, errorMessage).ConfigureAwait(false);
            }
        }
    }
}
