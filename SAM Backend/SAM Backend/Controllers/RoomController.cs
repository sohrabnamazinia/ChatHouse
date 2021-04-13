using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SAM_Backend.Models;
using SAM_Backend.Services;
using SAM_Backend.Utility;
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

        public RoomController(ILogger<AccountController> logger, IJWTService jWTService, AppDbContext context, IMinIOService minIOService)
        {
            this.logger = logger;
            this.jWTService = jWTService;
            this.context = context;
            this.minIOService = minIOService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateRoom(CreateRoomViewModel model)
        {
            #region find user
            var user = await jWTService.FindUserByTokenAsync(Request, context);
            #endregion find user

            #region room 
            var room = new Room()
            {
                Creator = user,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                Name = model.Name,
                Description = model.Description
            };
            #endregion room

            #region Interests
            var updatedInterests = model.Interests;
            if (InterestsService.IsValidRoomInterest(updatedInterests)) InterestsService.SetInterestsForRoom(updatedInterests, room);
            else return BadRequest("Interest list is not in valid format for Room!");
            #endregion Interests

            #region return
            context.Rooms.Add(room);
            context.SaveChanges();
            return null;
            #endregion return
        }
    }
}
