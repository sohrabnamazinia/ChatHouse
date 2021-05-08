using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAM_Backend.ViewModels.ChatRoomHubViewModel
{
    public class ReceiveRoomNotificationViewModel
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
}
