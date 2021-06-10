using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using SAM_Backend.Models;
using SAM_Backend.Services;
using SAM_Backend.ViewModels.ChatRoomHubViewModel;
using SAM_Backend.ViewModels.Hubs.ChatRoomHubViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SAM_Backend.Hubs
{
    public class ChatRoomHub : Hub
    {
        #region Fields
        private readonly AppDbContext DbContext;
        private readonly UserManager<AppUser> userManager;
        private readonly IMinIOService minIOService;
        #endregion
        public ChatRoomHub(AppDbContext AppDbContext, UserManager<AppUser> userManager, IMinIOService minIOService)
        {
            #region DI
            DbContext = AppDbContext;
            this.userManager = userManager;
            this.minIOService = minIOService;
            #endregion DI
        }
        public async Task SendMessageToRoom(MessageViewModel messageModel)
        {
            #region check room end date
            var room = DbContext.Rooms.Find(messageModel.RoomId);
            if ((DateTime.Compare(room.EndDate, DateTime.Now) <= 0))
            {
                await Clients.Caller.SendAsync("FinishRoom", new FinishRoomViewModel() { RoomId = room.Id });
                return;           
            }
            #endregion

            #region Text
            if (messageModel.MessageType == MessageType.Text)
            {
                #region Db
                RoomMessage message = new RoomMessage()
                {
                    SentDate = DateTime.Now,
                    Sender = await userManager.FindByNameAsync(messageModel.UserModel.Username),
                    ContentType = MessageType.Text,
                    Content = messageModel.Message.ToString(),
                    Room = room,
                    Parent = messageModel.ParentId != -1 ? DbContext.RoomsMessages.Find(messageModel.ParentId) : null
                };
                DbContext.RoomsMessages.Add(message);
                DbContext.SaveChanges();
                #endregion Db
            }
            #endregion Text

            #region Image File
            else if (messageModel.MessageType == MessageType.ImageFile)
            {
                #region get data
                IFormFile image = (IFormFile)messageModel.Message;
                var user = await userManager.FindByNameAsync(messageModel.UserModel.Username);
                #endregion

                #region minio
                var response = await minIOService.UploadRoomImageMessage(image, user, room, messageModel.ParentId);
                if (!response.Done) throw new Exception(response.Message);
                #endregion

            }
            #endregion File

            #region Send
            messageModel.IsMe = false;
            await Clients.OthersInGroup(messageModel.RoomId.ToString()).SendAsync("ReceiveRoomMessage", messageModel);
            messageModel.IsMe = true;
            await Clients.Caller.SendAsync("ReceiveRoomMessage", messageModel);
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
            else if ((!room.Members.Contains(user)) && room.Creator != user)
            {
                outputModel.Message = "User is not a member of the room!";
                throw new Exception(outputModel.Message);
            }
            else if ((DateTime.Compare(room.EndDate, DateTime.Now) <= 0))
            {
                await Clients.Caller.SendAsync("FinishRoom", new FinishRoomViewModel() { RoomId = room.Id });
                return;
            }
            #endregion

            #region attempt
            await Groups.AddToGroupAsync(Context.ConnectionId, inputModel.RoomId.ToString());
            outputModel.Done = true;
            ReceiveRoomNotificationViewModel notificationViewModel = new ReceiveRoomNotificationViewModel()
            {
                Notification = RoomNotification.Join,
                UserModel = inputModel.UserModel,
                RoomId = inputModel.RoomId
            };
            notificationViewModel.IsMe = false;
            await Clients.OthersInGroup(inputModel.RoomId.ToString()).SendAsync("ReceiveRoomNotification", notificationViewModel);
            notificationViewModel.IsMe = true;
            await Clients.Caller.SendAsync("ReceiveRoomNotification", notificationViewModel);
            #region Db
            //RoomMessage message = new RoomMessage()
            //{
            //    ContentType = MessageType.JoinNotification,
            //    Room = DbContext.Rooms.Find(inputModel.RoomId),
            //    SentDate = DateTime.Now,
            //    Sender = await userManager.FindByNameAsync(inputModel.UserModel.Username),
            //};
            //DbContext.RoomsMessages.Add(message);
            #endregion Db
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
            notificationViewModel.IsMe = false;
            await Clients.OthersInGroup(inputModel.RoomId.ToString()).SendAsync("ReceiveRoomNotification", notificationViewModel);
            notificationViewModel.IsMe = true;
            await Clients.Caller.SendAsync("ReceiveRoomNotification", notificationViewModel);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, inputModel.RoomId.ToString());

            #region Db
            //RoomMessage message = new RoomMessage()
            //{
            //    ContentType = MessageType.LeftNotification,
            //    Room = DbContext.Rooms.Find(inputModel.RoomId),
            //    SentDate = DateTime.Now,
            //    Sender = await userManager.FindByNameAsync(inputModel.UserModel.Username),
            //};
            //DbContext.RoomsMessages.Add(message);
            #endregion Db
            #endregion
        }
        public async Task LoadRoomMessages(int RoomId, string Username)
        {
            #region fetch messages
            List<LoadMessageViewModel> messages = null;
            List<RoomMessage> roomMessages = null;
            try
            {
                roomMessages = DbContext.RoomsMessages.Where(x => x.Room.Id == RoomId).Select(x => x).ToList();
                messages = roomMessages.Select(x => new LoadMessageViewModel(x)).ToList();
                foreach (var message in messages)
                {
                    if (message.ContetntType == MessageType.ImageFile)
                    {
                        message.LinkIfImage = await minIOService.GenerateUrlRoomImageMessage(RoomId.ToString(), message.Content);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            #endregion fetch messages

            #region set IsMe & return
            messages.ForEach(x => x.IsMe = x.Sender.Username.Equals(Username) ? true : false);
            await Clients.Caller.SendAsync("ReceiveRoomAllMessages", messages);
            #endregion
        }

    }
}
