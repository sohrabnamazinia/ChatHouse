using SAM_Backend.Models;
using SAM_Backend.ViewModels.Room;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAM_Backend.ViewModels.Account
{
    public class AppUserViewModel
    {
        public AppUserViewModel(AppUser user)
        {
            Email = user.Email;
            Username = user.UserName;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Bio = user.Bio;
            Interests = InterestsService.ConvertInterestsToLists(user.Interests);
            Followers = new List<FollowerFollowingViewModel>();
            Followings = new List<FollowerFollowingViewModel>();
            foreach (var f in user.Followings)
            {
                Followings.Add(new FollowerFollowingViewModel() 
                {
                    FirstName = f.FirstName,
                    LastName = f.LastName,
                    Username = f.UserName,
                    ImageLink = f.ImageLink
                });
            }
            foreach (var f in user.Followers)
            {
                Followers.Add(new FollowerFollowingViewModel()
                {
                    FirstName = f.FirstName,
                    LastName = f.LastName,
                    Username = f.UserName,
                    ImageLink = f.ImageLink
                });
            }
            CreatedRooms = user.CreatedRooms.Select(x => new RoomViewModel(x)).ToList();
            InRooms = user.InRooms.Select(x => new RoomViewModel(x)).ToList();
        }

        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Bio { get; set; }
        public List<List<int>> Interests { get; set; }
        public string Username { get; set; }
        public List<FollowerFollowingViewModel> Followers { get; set; }
        public List<FollowerFollowingViewModel> Followings { get; set; }
        public List<RoomViewModel> InRooms { get; set; }
        public List<RoomViewModel> CreatedRooms { get; set; }
    }
}
