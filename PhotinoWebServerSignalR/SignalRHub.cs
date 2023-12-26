using Microsoft.AspNetCore.SignalR;

namespace PhotinoWebServerSignalR
{
    public class SignalRHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            // You can send to ALL and then any open window will receive the same message and show the same results.
            await Clients.All.SendAsync("ReceiveMessage", $"{Context.ConnectionId} has joined");
        }

        public async Task ServerMessageHandler(string Message)
        {
            MessageObject L_MessageObject = Newtonsoft.Json.JsonConvert.DeserializeObject<MessageObject>(Message);

            await _ProcessMessage(L_MessageObject);

            // Here we just respond to the window that made the call to the server.
            await Clients.Caller.SendAsync("ClientMessageHandler", L_MessageObject);
        }

        private async Task _ProcessMessage(MessageObject P_MessageObject)
        {
            switch (P_MessageObject.MessageType)
            {
                case MessageObjectType.Greetings:
                    P_MessageObject.Message = "Message received from script. It said: " + P_MessageObject.Message;
                    break;

                case MessageObjectType._DoNothing:
                    P_MessageObject.Message = "Message received from script, but it said to do nothing :(";
                    break;
            }
        }
    }
}