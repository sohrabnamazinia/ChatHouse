using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAM_Backend.ViewModels.ChatRoomHubViewModel
{
    public class JoinRoomResponseViewModel
    {
        public bool Done { get; set; }
        public string Message { get; set; }
    }
    public class JoinRoomViewModel
    {
        public ChatRoomHubUserViewModel UserModel { get; set; }
        public int RoomId { get; set; }
    }

    public class LeaveRoomViewModel
    {
        public ChatRoomHubUserViewModel UserModel { get; set; }
        public int RoomId { get; set; }
    }
}
