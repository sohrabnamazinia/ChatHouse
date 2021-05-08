using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SAM_Backend.Models;
using SAM_Backend.Services;
using SAM_Backend.Utility;
using SAM_Backend.ViewModels.Account;
using SAM_Backend.ViewModels.Search;

namespace SAM_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class SearchController : ControllerBase
    {
        #region Fields

        private readonly UserManager<AppUser> userManager;
        private readonly SignInManager<AppUser> signInManager;
        private readonly ILogger<SearchController> logger;
        private readonly IJWTService jWTService;
        private readonly IDataProtectionProvider dataProtectionProvider;
        private readonly AppDbContext context;
        private readonly IMinIOService minIOService;
        private readonly IDataProtector protector;

        #endregion

        public SearchController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ILogger<SearchController> logger, IJWTService jWTService, IDataProtectionProvider dataProtectionProvider, AppDbContext context, IMinIOService minIOService)
        {
            #region Instantiation

            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
            this.jWTService = jWTService;
            this.dataProtectionProvider = dataProtectionProvider;
            this.context = context;
            this.minIOService = minIOService;
            this.protector = dataProtectionProvider.CreateProtector(DataProtectionPurposeStrings.UserIdQueryString);

            #endregion
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<ICollection<AppUserSearchViewModel>>> Suggest([FromQuery] PaginationParameters pagination)
        {
            #region Find users
            List<AppUser> suggestedUsers = await context.Users.OrderByDescending(x => x.Followers.Count).Skip((pagination.PageNumber - 1) * (pagination.PageSize)).Take(pagination.PageSize).ToListAsync();
            #endregion

            #region update image links
            foreach (var user in suggestedUsers)
                user.ImageLink = await minIOService.GenerateUrl(user.Id, user.ImageName);
            #endregion

            #region convert to viewModel
            var suggestedUsersViewModel = suggestedUsers.Select(x => new AppUserSearchViewModel(x)).ToList();
            #endregion

            #region return
            return Ok(suggestedUsersViewModel);
            #endregion
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<ICollection<AppUserSearchViewModel>>> Category(string name, int? category, [FromQuery] PaginationParameters pagination)
        {
            #region check input

            if (category < 0 || category > 13)
                return BadRequest("category is not supported");

            #endregion

            #region create users Query

            IQueryable<AppUser> usersQuery;
            switch (category)
            {
                case 0:
                    usersQuery = context.Users.Where(user => user.Interests.Wellness > 0);
                    break;
                case 1:
                    usersQuery = context.Users.Where(user => user.Interests.Identity > 0);
                    break;
                case 2:
                    usersQuery = context.Users.Where(user => user.Interests.Places > 0);
                    break;
                case 3:
                    usersQuery = context.Users.Where(user => user.Interests.WorldAffairs > 0);
                    break;
                case 4:
                    usersQuery = context.Users.Where(user => user.Interests.Tech > 0);
                    break;
                case 5:
                    usersQuery = context.Users.Where(user => user.Interests.HangingOut > 0);
                    break;
                case 6:
                    usersQuery = context.Users.Where(user => user.Interests.KnowLedge > 0);
                    break;
                case 7:
                    usersQuery = context.Users.Where(user => user.Interests.Hustle > 0);
                    break;
                case 8:
                    usersQuery = context.Users.Where(user => user.Interests.Sports > 0);
                    break;
                case 9:
                    usersQuery = context.Users.Where(user => user.Interests.Arts > 0);
                    break;
                case 10:
                    usersQuery = context.Users.Where(user => user.Interests.Life > 0);
                    break;
                case 11:
                    usersQuery = context.Users.Where(user => user.Interests.Languages > 0);
                    break;
                case 12:
                    usersQuery = context.Users.Where(user => user.Interests.Entertainment > 0);
                    break;
                case 13:
                    usersQuery = context.Users.Where(user => user.Interests.Faith > 0);
                    break;

                default:
                    usersQuery = context.Users;
                    break;
            }
            #endregion

            #region Find users
            List<AppUser> users = await usersQuery
                .Where(user =>
                    name == null || ((user.FirstName != null && user.FirstName.Contains(name))) ||
                    (user.LastName != null && user.LastName.Contains(name)) ||
                    user.UserName.Contains(name))
                .OrderByDescending(x => x.Followers.Count).Skip((pagination.PageNumber - 1) * (pagination.PageSize)).Take(pagination.PageSize).ToListAsync();

            #endregion

            #region update image links
            foreach (var user in users)
                user.ImageLink = await minIOService.GenerateUrl(user.Id, user.ImageName);
            #endregion

            #region convert to viewModel
            var usersViewModel = users.Select(x => new AppUserSearchViewModel(x)).ToList();
            #endregion

            #region return
            return Ok(usersViewModel);
            #endregion
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<ICollection<AppUserSearchViewModel>>> Item(string name, int? category, int item, [FromQuery] PaginationParameters pagination)
        {

            #region check input

            if (category == null)
                return BadRequest("category can't be null");

            if (category < 0 || category > 13)
                return BadRequest("category must be between 0 to 13");

            if (item <= 0)
                return BadRequest("item must be positive");


            #endregion

            #region create users Query
            IQueryable<AppUser> usersQuery;
            switch (category)
            {
                case 0:
                    usersQuery = context.Users.Where(user => ((int)user.Interests.Wellness & item) > 0);
                    break;
                case 1:
                    usersQuery = context.Users.Where(user => ((int)user.Interests.Identity & item) > 0);
                    break;
                case 2:
                    usersQuery = context.Users.Where(user => ((int)user.Interests.Places & item) > 0);
                    break;
                case 3:
                    usersQuery = context.Users.Where(user => ((int)user.Interests.WorldAffairs & item) > 0);
                    break;
                case 4:
                    usersQuery = context.Users.Where(user => ((int)user.Interests.Tech & item) > 0);
                    break;
                case 5:
                    usersQuery = context.Users.Where(user => ((int)user.Interests.HangingOut & item) > 0);
                    break;
                case 6:
                    usersQuery = context.Users.Where(user => ((int)user.Interests.KnowLedge & item) > 0);
                    break;
                case 7:
                    usersQuery = context.Users.Where(user => ((int)user.Interests.Hustle & item) > 0);
                    break;
                case 8:
                    usersQuery = context.Users.Where(user => ((int)user.Interests.Sports & item) > 0);
                    break;
                case 9:
                    usersQuery = context.Users.Where(user => ((int)user.Interests.Arts & item) > 0);
                    break;
                case 10:
                    usersQuery = context.Users.Where(user => ((int)user.Interests.Life & item) > 0);
                    break;
                case 11:
                    usersQuery = context.Users.Where(user => ((int)user.Interests.Languages & item) > 0);
                    break;
                case 12:
                    usersQuery = context.Users.Where(user => ((int)user.Interests.Entertainment & item) > 0);
                    break;
                case 13:
                    usersQuery = context.Users.Where(user => ((int)user.Interests.Faith & item) > 0);
                    break;

                default:
                    usersQuery = context.Users;
                    break;
            }
            #endregion

            #region Find users
            List<AppUser> users = await usersQuery
                .Where(user =>
                    name == null || ((user.FirstName != null && user.FirstName.Contains(name))) ||
                    (user.LastName != null && user.LastName.Contains(name)) ||
                    user.UserName.Contains(name))
                .OrderByDescending(x => x.Followers.Count).Skip((pagination.PageNumber - 1) * (pagination.PageSize)).Take(pagination.PageSize).ToListAsync();
            #endregion

            #region update image links
            foreach (var user in users)
                user.ImageLink = await minIOService.GenerateUrl(user.Id, user.ImageName);
            #endregion

            #region convert to viewModel
            var usersViewModel = users.Select(x => new AppUserSearchViewModel(x)).ToList();
            #endregion

            #region return
            return Ok(usersViewModel);
            #endregion
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<ICollection<RoomSearchViewModel>>> RoomCategory(string name, int? category, [FromQuery] PaginationParameters pagination)
        {
            #region check input

            if (category < 0 || category > 13)
                return BadRequest("category is not supported");

            #endregion

            #region create Rooms Query

            IQueryable<Room> roomsQuery;
            switch (category)
            {
                case 0:
                    roomsQuery = context.Rooms.Where(room => room.Interests.Wellness > 0);
                    break;
                case 1:
                    roomsQuery = context.Rooms.Where(room => room.Interests.Identity > 0);
                    break;
                case 2:
                    roomsQuery = context.Rooms.Where(room => room.Interests.Places > 0);
                    break;
                case 3:
                    roomsQuery = context.Rooms.Where(room => room.Interests.WorldAffairs > 0);
                    break;
                case 4:
                    roomsQuery = context.Rooms.Where(room => room.Interests.Tech > 0);
                    break;
                case 5:
                    roomsQuery = context.Rooms.Where(room => room.Interests.HangingOut > 0);
                    break;
                case 6:
                    roomsQuery = context.Rooms.Where(room => room.Interests.KnowLedge > 0);
                    break;
                case 7:
                    roomsQuery = context.Rooms.Where(room => room.Interests.Hustle > 0);
                    break;
                case 8:
                    roomsQuery = context.Rooms.Where(room => room.Interests.Sports > 0);
                    break;
                case 9:
                    roomsQuery = context.Rooms.Where(room => room.Interests.Arts > 0);
                    break;
                case 10:
                    roomsQuery = context.Rooms.Where(room => room.Interests.Life > 0);
                    break;
                case 11:
                    roomsQuery = context.Rooms.Where(room => room.Interests.Languages > 0);
                    break;
                case 12:
                    roomsQuery = context.Rooms.Where(room => room.Interests.Entertainment > 0);
                    break;
                case 13:
                    roomsQuery = context.Rooms.Where(room => room.Interests.Faith > 0);
                    break;

                default:
                    roomsQuery = context.Rooms;
                    break;
            }
            #endregion

            #region Find Rooms
            List<Room> rooms = await roomsQuery
                .Where(room => room.EndDate > DateTime.Now)
                .Where(room => name == null ||
                               ((room.Name != null && room.Name.Contains(name))) ||
                               (room.Description != null && room.Description.Contains(name)))
                .OrderByDescending(x => x.Members.Count).Skip((pagination.PageNumber - 1) * (pagination.PageSize)).Take(pagination.PageSize).ToListAsync();

            #endregion

            #region convert to viewModel
            var roomsViewModel = rooms.Select(x => new RoomSearchViewModel(x)).ToList();
            #endregion

            #region return
            return Ok(roomsViewModel);
            #endregion
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<ICollection<AppUserSearchViewModel>>> RoomItem(string name, int? category, int item, [FromQuery] PaginationParameters pagination)
        {

            #region check input

            if (category == null)
                return BadRequest("category can't be null");

            if (category < 0 || category > 13)
                return BadRequest("category must be between 0 to 13");

            if (item <= 0)
                return BadRequest("item must be positive");

            #endregion

            #region create users Query
            IQueryable<Room> roomsQuery;
            switch (category)
            {
                case 0:
                    roomsQuery = context.Rooms.Where(room => ((int)room.Interests.Wellness & item) > 0);
                    break;
                case 1:
                    roomsQuery = context.Rooms.Where(room => ((int)room.Interests.Identity & item) > 0);
                    break;
                case 2:
                    roomsQuery = context.Rooms.Where(room => ((int)room.Interests.Places & item) > 0);
                    break;
                case 3:
                    roomsQuery = context.Rooms.Where(room => ((int)room.Interests.WorldAffairs & item) > 0);
                    break;
                case 4:
                    roomsQuery = context.Rooms.Where(room => ((int)room.Interests.Tech & item) > 0);
                    break;
                case 5:
                    roomsQuery = context.Rooms.Where(room => ((int)room.Interests.HangingOut & item) > 0);
                    break;
                case 6:
                    roomsQuery = context.Rooms.Where(room => ((int)room.Interests.KnowLedge & item) > 0);
                    break;
                case 7:
                    roomsQuery = context.Rooms.Where(room => ((int)room.Interests.Hustle & item) > 0);
                    break;
                case 8:
                    roomsQuery = context.Rooms.Where(room => ((int)room.Interests.Sports & item) > 0);
                    break;
                case 9:
                    roomsQuery = context.Rooms.Where(room => ((int)room.Interests.Arts & item) > 0);
                    break;
                case 10:
                    roomsQuery = context.Rooms.Where(room => ((int)room.Interests.Life & item) > 0);
                    break;
                case 11:
                    roomsQuery = context.Rooms.Where(room => ((int)room.Interests.Languages & item) > 0);
                    break;
                case 12:
                    roomsQuery = context.Rooms.Where(room => ((int)room.Interests.Entertainment & item) > 0);
                    break;
                case 13:
                    roomsQuery = context.Rooms.Where(room => ((int)room.Interests.Faith & item) > 0);
                    break;

                default:
                    roomsQuery = context.Rooms;
                    break;
            }
            #endregion

            #region Find Rooms
            List<Room> rooms = await roomsQuery
                .Where(room => room.EndDate > DateTime.Now)
                .Where(room => name == null ||
                               ((room.Name != null && room.Name.Contains(name))) ||
                               (room.Description != null && room.Description.Contains(name)))
                .OrderByDescending(x => x.Members.Count).Skip((pagination.PageNumber - 1) * (pagination.PageSize)).Take(pagination.PageSize).ToListAsync();

            #endregion

            #region convert to viewModel
            var roomsViewModel = rooms.Select(x => new RoomSearchViewModel(x)).ToList();
            #endregion

            #region return
            return Ok(roomsViewModel);
            #endregion
        }


        [HttpGet]
        [Authorize]
        public async Task<ActionResult<ICollection<AppUserSearchViewModel>>> RoomSuggestByInterests([FromQuery] PaginationParameters pagination)
        {
            #region Find user by token
            AppUser user = await jWTService.FindUserByTokenAsync(Request, context);
            #endregion

            #region create users Query

            var roomsQuery = context.Rooms.Where(room =>
                                                    (room.Interests.Wellness & user.Interests.Wellness) > 0 ||
                                                    (room.Interests.Identity & user.Interests.Identity) > 0 ||
                                                    (room.Interests.Places & user.Interests.Places) > 0 ||
                                                    (room.Interests.WorldAffairs & user.Interests.WorldAffairs) > 0 ||
                                                    (room.Interests.Tech & user.Interests.Tech) > 0 ||
                                                    (room.Interests.HangingOut & user.Interests.HangingOut) > 0 ||
                                                    (room.Interests.KnowLedge & user.Interests.KnowLedge) > 0 ||
                                                    (room.Interests.Hustle & user.Interests.Hustle) > 0 ||
                                                    (room.Interests.Sports & user.Interests.Sports) > 0 ||
                                                    (room.Interests.Arts & user.Interests.Arts) > 0 ||
                                                    (room.Interests.Life & user.Interests.Life) > 0 ||
                                                    (room.Interests.Languages & user.Interests.Languages) > 0 ||
                                                    (room.Interests.Entertainment & user.Interests.Entertainment) > 0 ||
                                                    (room.Interests.Faith & user.Interests.Faith) > 0);
            #endregion

            #region Find Rooms
            List<Room> rooms = await roomsQuery
                .Distinct()
                .Where(room => !room.Members.Contains(user))
                .Where(room => room.Creator != user)
                .Where(room => (room.EndDate > DateTime.Now) && (room.StartDate < DateTime.Now))
                .OrderByDescending(x => x.Members.Count).Skip((pagination.PageNumber - 1) * (pagination.PageSize)).Take(pagination.PageSize).ToListAsync();

            #endregion

            #region convert to viewModel
            var roomsViewModel = rooms.Select(x => new RoomSearchViewModel(x)).ToList();
            #endregion

            #region return
            return Ok(roomsViewModel);
            #endregion

        }


        [HttpGet]
        [Authorize]
        public async Task<ActionResult<ICollection<AppUserSearchViewModel>>> RoomSuggestByFollowings([FromQuery] PaginationParameters pagination)
        {
            #region Find user by token
            AppUser user = await jWTService.FindUserByTokenAsync(Request, context);
            #endregion

            #region Find Rooms

            List<Room> rooms = user.Followings.Select(x => x.InRooms.Union(x.CreatedRooms)).SelectMany(x => x)
                .Distinct()
                .Where(room => !room.Members.Contains(user))
                .Where(room => room.Creator != user)
                .Where(room => (room.EndDate > DateTime.Now) && (room.StartDate < DateTime.Now))
                .OrderByDescending(x => x.Members.Count).Skip((pagination.PageNumber - 1) * (pagination.PageSize)).Take(pagination.PageSize).ToList();

            #endregion

            #region match each room by user

            List<List<AppUser>> suggestBy = new List<List<AppUser>>();
            foreach (var room in rooms)
                suggestBy.Add(user.Followings.Where(x => x.InRooms.Contains(room) || x.CreatedRooms.Contains(room)).ToList());

            #endregion


            #region update image links
            foreach (var u in suggestBy.SelectMany(x => x).ToList())
                user.ImageLink = await minIOService.GenerateUrl(u.Id, u.ImageName);
            #endregion

            #region convert to viewModel
            var roomsViewModel = rooms.Select((x, i) => new RoomSearchByFollowingsViewModel(x, suggestBy[i])).ToList();
            #endregion

            #region return
            return Ok(roomsViewModel);
            #endregion

        }
    }
}
