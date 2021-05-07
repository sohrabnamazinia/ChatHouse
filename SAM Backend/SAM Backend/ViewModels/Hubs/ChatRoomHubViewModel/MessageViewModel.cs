using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAM_Backend.ViewModels.ChatRoomHubViewModel
{
    public class MessageViewModel 
    {
        public ChatRoomHubUserViewModel UserModel { get; set; }
        public MessageType MessageType { get; set; }
        public Object Message { get; set; }
        public int RoomId{ get; set; }
        public bool IsMe { get; set; }
    }

    public enum MessageType
    {
        Text,
        File
    }
}
