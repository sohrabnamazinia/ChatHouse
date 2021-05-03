using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAM_Backend.Hubs
{
    public class ChatRoomHub : Hub
    {
        // Just for testing connection
        public async void SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", "Message From " + user + " : " + message);
        }
    }
}
