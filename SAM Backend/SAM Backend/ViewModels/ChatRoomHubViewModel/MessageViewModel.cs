using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAM_Backend.ViewModels.ChatRoomHubViewModel
{
    public class MessageViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ImageLink { get; set; }
        public MessageType MessageType { get; set; }
        public Object Message { get; set; }
    }

    public enum MessageType
    {
        Text,
        Image,
        File
    }
}
