using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using SAM_Backend.Hubs;
using SAM_Backend.Models;
using SAM_Backend.Services;
using SAM_Backend.Utility;
using SAM_Backend.ViewModels.Account;
using SAM_Backend.ViewModels.ChatRoomHubViewModel;
using SAM_Backend.ViewModels.Hubs.ChatRoomHubViewModel;
using SAM_Backend.ViewModels.Room;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAM_Backend.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class RoomController : ControllerBase
    {
        private readonly ILogger<AccountController> logger;
        private readonly IJWTService jWTService;
        private readonly AppDbContext context;
        private readonly IMinIOService minIOService;
        private readonly UserManager<AppUser> userManager;
        private readonly IHubContext<ChatRoomHub> chatHubContext;

        public RoomController(ILogger<AccountController> logger, IJWTService jWTService, AppDbContext context, IMinIOService minIOService, UserManager<AppUser> userManager, IHubContext<ChatRoomHub> chatHubContext)
        {
            this.logger = logger;
            this.jWTService = jWTService;
            this.context = context;
            this.minIOService = minIOService;
            this.userManager = userManager;
            this.chatHubContext = chatHubContext;
        }

        [HttpPost]
        public async Task<IActionResult> CreateRoom(CreateRoomViewModel model)
        {
            #region validation
            var user = await jWTService.FindUserByTokenAsync(Request, context);
            var startDate = model.StartDate != null ? model.StartDate.Value : DateTime.Now;
            var endDate = model.EndDate != null ? model.EndDate.Value : DateTime.Parse(startDate.ToString()).Add(TimeSpan.FromHours(Constants.RoomDefaultExpirationPeriodInHours));
            if (DateTime.Compare(startDate, endDate) >= 0) return BadRequest("End date must be after start date!");
            if (DateTime.Compare(DateTime.Now, endDate) >= 0) return BadRequest("Room date has been expired!");
            if (DateTime.Compare(DateTime.Now, startDate) > 0) startDate = DateTime.Now;
            var updatedInterests = model.Interests;
            if (!(InterestsService.IsValidRoomInterest(updatedInterests))) return BadRequest(Constants.InterestsRoomFormatError);
            #endregion

            #region room 
            var room = new Room()
            {
                Creator = user,
                StartDate = startDate,
                EndDate = endDate,
                Name = model.Name,
                Description = model.Description,
                Members = new List<AppUser>(),
                Interests = new Interests()
            };
            InterestsService.SetInterestsForRoom(updatedInterests, room);
            #endregion room

            #region return
            context.Rooms.Add(room);
            context.SaveChanges();
            return Ok(new RoomViewModel(room));
            #endregion return
        }

        [HttpPost]
        public async Task<IActionResult> JoinRoom(int roomId)
        {
            #region find user & room
            var user = await jWTService.FindUserByTokenAsync(Request, context);
            var room = context.Rooms.Find(roomId);
            if (room == null) return NotFound(Constants.RoomNotFound);
            #endregion
            
            #region check membership
            if (room.Members.Contains(user)) return BadRequest("User is already a member of the room!");
            if (room.Creator == user) return BadRequest("User is the creator of the room!");
            #endregion

            #region return
            room.Members.Add(user);
            context.SaveChanges();

            #region Save Notif
            RoomMessage message = new RoomMessage()
            {
                ContentType = MessageType.JoinNotification,
                Room = context.Rooms.Find(roomId),
                SentDate = DateTime.Now,
                Sender = await userManager.FindByNameAsync(user.UserName),
            };
            context.RoomsMessages.Add(message);
            context.SaveChanges();
            #endregion Save Notif
            return Ok(new AppUserViewModel(user));
            #endregion
        }

        [HttpGet]
        public IActionResult GetRoom(int roomId)
        {
            #region find room
            var room = context.Rooms.Find(roomId);
            if (room == null) return NotFound(Constants.RoomNotFound);
            #endregion

            #region return
            return Ok(new RoomViewModel(room));
            #endregion
        }

        [HttpDelete]
        public async Task<IActionResult> RemoveUser(int roomId, string username)
        {
            #region find users & room
            var user = await jWTService.FindUserByTokenAsync(Request, context);
            var room = context.Rooms.Find(roomId);
            if (room == null) return NotFound(Constants.RoomNotFound);
            if (room.Creator != user) return StatusCode(StatusCodes.Status403Forbidden, "the user is not allowed to do it for this room!");
            var removingUser = await userManager.FindByNameAsync(username);
            if (removingUser == null) return NotFound(Constants.UserNotFoundError);
            if (removingUser == user) return BadRequest("creator can not be removed!");
            if (!room.Members.Contains(removingUser)) return BadRequest("the user is not in this room!");
            #endregion

            #region remove user
            room.Members.Remove(removingUser);
            #endregion

            #region return
            context.SaveChanges();
            return Ok(new RoomViewModel(room));
            #endregion
        }

        [HttpDelete]
        public async Task<IActionResult> LeaveRoom(int roomId)
        {
            #region find user & room
            var user = await jWTService.FindUserByTokenAsync(Request, context);
            var room = context.Rooms.Find(roomId);
            if (room == null) return NotFound(Constants.RoomNotFound);
            if (!room.Members.Contains(user)) return BadRequest("User is not a member of the room");
            if (room.Creator == user) return BadRequest("Creator can not leave the room");
            #endregion

            #region leave room
            user.InRooms.Remove(room);
            #endregion

            #region return 
            context.SaveChanges();

            #region Save Notif
            RoomMessage message = new RoomMessage()
            {
                ContentType = MessageType.LeftNotification,
                Room = context.Rooms.Find(roomId),
                SentDate = DateTime.Now,
                Sender = await userManager.FindByNameAsync(user.UserName),
            };
            context.RoomsMessages.Add(message);
            context.SaveChanges();
            #endregion Save Notif
            return Ok(new AppUserViewModel(user));
            #endregion
        }
        
        [HttpPost]
        public async Task<IActionResult> UpdateRoom(UpdateRoomViewModel model)
        {
            #region find user and room
            var user = await jWTService.FindUserByTokenAsync(Request, context);
            var room = context.Rooms.Find(model.RoomId);
            if (room == null) return NotFound(Constants.RoomNotFound);
            if (user != room.Creator) return StatusCode(StatusCodes.Status403Forbidden, "Only creator can update the room!");
            #endregion

            #region name and description
            room.Name = model.Name != null ? model.Name : room.Name;
            room.Description = model.Description != null ? model.Description : room.Description;
            #endregion

            #region Date
            if (model.EndDate != null)
            {
                if (DateTime.Compare(DateTime.Now, model.EndDate.Value) >= 0) return BadRequest("room end date has been passed!");
                if (model.StartDate != null)
                {
                    if (DateTime.Compare(model.StartDate.Value, model.EndDate.Value) >= 0) return BadRequest("End date must be after start date!");
                    if (DateTime.Compare(DateTime.Now, model.StartDate.Value) > 0) model.StartDate = DateTime.Now;
                    room.StartDate = model.StartDate.Value;
                }
                room.EndDate = model.EndDate.Value;
            }
            else if (model.StartDate != null)
            {
                if (DateTime.Compare(DateTime.Now, model.StartDate.Value) >= 0) model.StartDate = DateTime.Now;
                if (model.EndDate != null)
                {
                    if (DateTime.Compare(model.StartDate.Value, model.EndDate.Value) >= 0) return BadRequest("End date must be after start date!");
                    room.StartDate = model.StartDate.Value;
                }
                room.StartDate = model.StartDate.Value;
            }
            #endregion

            #region Interests
            if (model.Interests != null)
            {
                if (!InterestsService.IsValidRoomInterest(model.Interests)) return BadRequest(Constants.InterestsRoomFormatError);
                InterestsService.SetInterestsForRoom(model.Interests, room);
            }
            #endregion

            #region Db & return 
            context.Rooms.Update(room);
            context.SaveChanges();
            return Ok(new RoomViewModel(room));
            #endregion
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteRoom(int roomId)
        {
            #region find user and room
            var user = await jWTService.FindUserByTokenAsync(Request, context);
            var room = context.Rooms.Find(roomId);
            if (room == null) return NotFound(Constants.RoomNotFound);
            if (room.Creator != user) return StatusCode(StatusCodes.Status403Forbidden, "Only creator can delete a room");
            #endregion

            #region delete and return
            var messages = context.RoomsMessages.Where(x => x.Room == room).ToList();
            context.RoomsMessages.RemoveRange(messages);

            context.Rooms.Remove(room);
            context.SaveChanges();
            return Ok("Room is deleted!");
            #endregion
        }

        #region Message
        [HttpPost]
        [FileUploadOperation.FileContentType]
        public async Task<IActionResult> SendMessage(IFormFile formFile, [FromQuery] MessageViewModel messageModel)
        {
            #region get user
            var user = await jWTService.FindUserByTokenAsync(Request, context);
            #endregion

            #region check failed cases
            var room = context.Rooms.Find(messageModel.RoomId);
            if (room == null) return NotFound("Room not Found");
            if ((!room.Members.Contains(user)) && room.Creator != user)
            {
                return StatusCode(StatusCodes.Status403Forbidden, "User is not a member/owner of the room!");
            }
            if ((DateTime.Compare(room.EndDate, DateTime.Now) <= 0))
            {
                messageModel.IsMe = true;
                await chatHubContext.Clients.Client(messageModel.ConnectionId).SendAsync("FinishRoom", new FinishRoomViewModel() { RoomId = room.Id });
                return BadRequest("Room has been finished, notification is just sent to the caller");
            }
            if (messageModel.MessageType == MessageType.Text && messageModel.Message == null)
            {
                return BadRequest("Message field must be not null in text message");
            }
            if (messageModel.MessageType == MessageType.ImageFile)
            {
                try
                {
                    var a = Request.Form.Files;
                }
                catch (InvalidOperationException)
                {
                    return BadRequest("form data must be not null in image type messaging requests");
                }
            }
            #endregion

            #region text
            if (messageModel.MessageType == MessageType.Text)
            {
                #region Send
                messageModel.IsMe = false;
                chatHubContext.Clients.GroupExcept(room.Id.ToString(), messageModel.ConnectionId).SendAsync("ReceiveRoomMessage", messageModel).Wait();
                messageModel.IsMe = true;
                chatHubContext.Clients.Client(messageModel.ConnectionId).SendAsync("ReceiveRoomMessage", messageModel).Wait();
                #endregion
                #region Db
                RoomMessage message = new RoomMessage()
                {
                    SentDate = DateTime.Now,
                    Sender = await userManager.FindByNameAsync(user.UserName),
                    ContentType = MessageType.Text,
                    Content = messageModel.Message.ToString(),
                    Room = room,
                    Parent = messageModel.ParentId != -1 ? context.RoomsMessages.Find(messageModel.ParentId) : null
                };
                context.RoomsMessages.Add(message);
                context.SaveChanges();
                #endregion Db
            }
            #endregion text

            #region Image
            else if (messageModel.MessageType == MessageType.ImageFile)
            {
                #region get data
                var fileCollection = Request.Form.Files;
                #endregion

                #region minio
                var response = await minIOService.UploadRoomImageMessage(fileCollection, user, room, messageModel.ParentId);
                if (!response.Done) return BadRequest(response.Message);
                #endregion

                #region Send
                messageModel.Message = response.roomImageMessage.LinkIfImage;
                messageModel.IsMe = false;
                chatHubContext.Clients.GroupExcept(room.Id.ToString(), messageModel.ConnectionId).SendAsync("ReceiveRoomMessage", messageModel).Wait();
                messageModel.IsMe = true;
                chatHubContext.Clients.Client(messageModel.ConnectionId).SendAsync("ReceiveRoomMessage", messageModel).Wait();
                #endregion

                #region return
                return Ok();
                #endregion
            }
            #endregion Image

            #region other media types
            else
            {
                return StatusCode(415, "Other data types transmission is not supported yet!");
            }
            #endregion other media types

            #region return
            return Ok();
            #endregion
        }
        #endregion

    }
}
