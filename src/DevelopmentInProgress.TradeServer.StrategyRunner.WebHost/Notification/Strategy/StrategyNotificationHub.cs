using DevelopmentInProgress.Socket.Messages;
using DevelopmentInProgress.Socket.Server;
using DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Notification.Server;
using Newtonsoft.Json;
using System;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Notification.Strategy
{
    public class StrategyNotificationHub : SocketServer, IServerNotification
    {
        public StrategyNotificationHub(ConnectionManager connectionManager, ChannelManager channelManager)
            : base(connectionManager, channelManager)
        {
        }

        public event EventHandler<ServerNotificationEventArgs> ServerNotification;

        public async override Task OnClientConnectAsync(WebSocket websocket, string clientId, string strategyName)
        {
            if (string.IsNullOrWhiteSpace(clientId))
            {
                throw new ArgumentNullException("clientId cannot be null or empty.");
            }

            if(string.IsNullOrWhiteSpace(strategyName))
            {
                throw new ArgumentNullException("The strategy to subscribe to must be specified in the data parameter.");
            }

            var connection = await base.AddWebSocketAsync(websocket).ConfigureAwait(false);

            connection.Name = clientId;

            SubscribeToChannel(strategyName, websocket);

            var connectionInfo = connection.GetConnectionInfo();

            var json = JsonConvert.SerializeObject(connectionInfo);

            var message = new Message { MethodName = "OnConnected", SenderConnectionId = "StrategyRunner", Data = json };

            await SendMessageAsync(websocket, message).ConfigureAwait(false);

            OnServerNotification();
        }
        
        public async override Task ReceiveAsync(WebSocket webSocket, Message message)
        {
            try
            {
                switch (message.MessageType)
                {
                    case MessageType.UnsubscribeFromChannel:
                        UnsubscribeFromChannel(message.Data, webSocket);
                        break;
                }

                OnServerNotification();
            }
            catch (Exception ex)
            {
                var errorMessage = new Message { MethodName = message.MethodName, SenderConnectionId = message.SenderConnectionId, Data = $"{MessageType.UnsubscribeFromChannel} Error : {ex.Message}" };
                await SendMessageAsync(webSocket, errorMessage).ConfigureAwait(false);
            }
        }

        private void OnServerNotification()
        {
            var serverNotification = ServerNotification;
            serverNotification?.Invoke(this, new ServerNotificationEventArgs());
        }
    }
}
