using Microsoft.AspNetCore.SignalR;
using SAM_Backend.ViewModels.ChatRoomHubViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAM_Backend.Hubs
{
    public class ChatRoomHub : Hub
    {
        // Just for testing connection
        public async void SendMessage(MessageSenderViewModel sender, MessageViewModel messageModel)
        {
            if (messageModel.MessageType == MessageType.Text)
            {
                string message = messageModel.Message.ToString();
                await Clients.All.SendAsync("ReceiveMessage", sender, messageModel);
            }
            else
            {
                await Clients.All.SendAsync("ReceiveMessage", sender, "Message Format not supported yet");
            }
        }
    }
}
