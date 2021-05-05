using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using SAM_Backend.Models;
using SAM_Backend.ViewModels.ChatRoomHubViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAM_Backend.Hubs
{
    public class ChatRoomHub : Hub
    {
        #region Fields
        private readonly AppDbContext DbContext;
        private readonly UserManager<AppUser> userManager;
        #endregion
        public ChatRoomHub(AppDbContext AppDbContext, UserManager<AppUser> userManager)
        {
            #region DI
            DbContext = AppDbContext;
            this.userManager = userManager;
            #endregion DI
        }

        public Task SendMessageToRoom(MessageViewModel messageModel)
        {
            #region Text 
            if (messageModel.MessageType == MessageType.Text)
            {
                return Clients.Group(messageModel.RoomId.ToString()).SendAsync("ReceiveRoomMessage", messageModel);
            }
            #endregion

            #region File
            else
            {
                return Clients.Group(messageModel.RoomId.ToString()).SendAsync("ReceiveRoomMessage", "Message Format not supported yet");
            }
            #endregion
        }

        public async Task JoinRoom(JoinRoomViewModel inputModel)
        {
            #region get room & user
            JoinRoomResponseViewModel outputModel = new JoinRoomResponseViewModel();
            Room room = DbContext.Rooms.Find(inputModel.RoomId);
            AppUser user = await userManager.FindByNameAsync(inputModel.UserModel.Username);
            #endregion

            #region Failed cases
            if (room == null)
            {
                outputModel.Message = "Room Not Found";
                throw new Exception(outputModel.Message);
            }
            if (user == null)
            {
                outputModel.Message = "User not found";
                throw new Exception(outputModel.Message);
            }
            else if (room.Members.Contains(user) || room.Creator != user)
            {
                outputModel.Message = "User is not a member of the room!";
                throw new Exception(outputModel.Message);
            }
            #endregion

            #region done
            await Groups.AddToGroupAsync(Context.ConnectionId, inputModel.RoomId.ToString());
            outputModel.Done = true;
            ReceiveRoomNotificationViewModel notificationViewModel = new ReceiveRoomNotificationViewModel()
            {
                Notification = RoomNotification.Join,
                UserModel = inputModel.UserModel,
                RoomId = inputModel.RoomId
            };
            await Clients.Group(inputModel.RoomId.ToString()).SendAsync("ReceiveRoomNotification", notificationViewModel);
            #endregion
        }

        public async Task LeaveRoom(LeaveRoomViewModel inputModel)
        {
            #region create model
            ReceiveRoomNotificationViewModel notificationViewModel = new ReceiveRoomNotificationViewModel()
            {
                Notification = RoomNotification.Left,
                UserModel = inputModel.UserModel,
                RoomId = inputModel.RoomId
            };
            #endregion

            #region attempt
            await Clients.Group(inputModel.RoomId.ToString()).SendAsync("ReceiveRoomNotification", notificationViewModel);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, inputModel.RoomId.ToString());
            #endregion
        }
    }
}
