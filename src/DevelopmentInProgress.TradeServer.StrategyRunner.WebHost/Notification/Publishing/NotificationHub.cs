using DipSocket.Messages;
using DipSocket.Server;
using Newtonsoft.Json;
using System;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace DevelopmentInProgress.TradeServer.StrategyRunner.WebHost.Notification.Publishing
{
    public class NotificationHub : DipSocketServer
    {
        public NotificationHub(ConnectionManager connectionManager, ChannelManager channelManager)
            : base(connectionManager, channelManager)
        {
        }

        public async override Task OnClientConnectAsync(WebSocket websocket, string clientId, string data)
        {
            if (string.IsNullOrWhiteSpace(clientId))
            {
                throw new ArgumentNullException("clientId cannot be null or empty.");
            }

            if(string.IsNullOrWhiteSpace(data))
            {
                throw new ArgumentNullException("The strategy to subscribe to must be specified in the data parameter.");
            }

            var connection = await base.AddWebSocketAsync(websocket).ConfigureAwait(false);

            connection.Name = clientId;

            SubscribeToChannel(data, websocket);

            var connectionInfo = connection.GetConnectionInfo();

            var json = JsonConvert.SerializeObject(connectionInfo);

            var message = new Message { MethodName = "OnConnected", SenderConnectionId = "StrategyRunner", Data = json };

            await SendMessageAsync(websocket, message).ConfigureAwait(false);
        }
        
        public async override Task ReceiveAsync(WebSocket webSocket, Message message)
        {
            try
            {
                switch (message.MessageType)
                {
                    case MessageType.UnsubscribeFromChannel:
                        var unsubscribedChannel = UnsubscribeFromChannel(message.Data, webSocket);
                        break;
                }
            }
            catch (Exception ex)
            {
                var errorMessage = new Message { MethodName = message.MethodName, SenderConnectionId = message.SenderConnectionId, Data = $"{MessageType.UnsubscribeFromChannel} Error : {ex.Message}" };
                await SendMessageAsync(webSocket, message).ConfigureAwait(false);
            }
        }
    }
}
