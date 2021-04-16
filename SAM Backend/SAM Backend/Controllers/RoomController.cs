using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SAM_Backend.Models;
using SAM_Backend.Services;
using SAM_Backend.Utility;
using SAM_Backend.ViewModels.Account;
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

        public RoomController(ILogger<AccountController> logger, IJWTService jWTService, AppDbContext context, IMinIOService minIOService, UserManager<AppUser> userManager)
        {
            this.logger = logger;
            this.jWTService = jWTService;
            this.context = context;
            this.minIOService = minIOService;
            this.userManager = userManager;
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
            var updatedInterests = model.Interests;
            if (!(InterestsService.IsValidRoomInterest(updatedInterests))) return BadRequest("Interests list is not in a valid format for a Room");
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
            //user.InRooms.Add(room);
            context.SaveChanges();
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
            return Ok(new AppUserViewModel(user));
            #endregion
        }

    }
}
