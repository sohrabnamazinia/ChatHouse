using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalR.Test
{
    public class MessageModel
    {
        public MessageType MessageType { get; set; }
        public Object Message { get; set; }
    }

    public enum MessageType
    {
        Text,
        Image,
        File
    }

    public class MessageSenderModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string ImageLink { get; set; }
    }
}
