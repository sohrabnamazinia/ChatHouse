using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalR.Test
{
    public class MessageModel
    {
        public ChatRoomHubUserViewModel UserModel { get; set; }
        public MessageType MessageType { get; set; }
        public Object Message { get; set; }
        public int RoomId { get; set; }
        public bool IsMe { get; set; }
    }

    public class ChatRoomHubUserViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string ImageLink { get; set; }
    }

    public enum MessageType
    {
        Text,
        File,
        JoinNotification,
        LeftNotification
    }

    public class LeaveRoomViewModel
    {
        public ChatRoomHubUserViewModel UserModel { get; set; }
        public int RoomId { get; set; }
    }

    public class JoinRoomViewModel
    {
        public ChatRoomHubUserViewModel UserModel { get; set; }
        public int RoomId { get; set; }
    }

    public class ReceiveRoomNotification
    {
        public RoomNotification Notification { get; set; }
        public ChatRoomHubUserViewModel UserModel { get; set; }
        public int RoomId { get; set; }
        public bool IsMe { get; set; }

    }

    public enum RoomNotification
    {
        Join,
        Left
    }

    public class LoadMessageViewModel
    {
        public int Id { get; set; }
        public int ParentId { get; set; }
        public string Content { get; set; }
        public MessageType ContetntType { get; set; }
        public DateTime SentDate { get; set; }
        public ChatRoomHubUserViewModel Sender { get; set; }
        public bool IsMe { get; set; }
        public int RoomId { get; set; }
    }

    public class FinishRoomViewModel
    {
        public int RoomId { get; set; }
    }
}
