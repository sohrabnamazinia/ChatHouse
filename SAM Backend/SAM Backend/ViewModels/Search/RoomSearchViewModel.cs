using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SAM_Backend.Models;

namespace SAM_Backend.ViewModels.Search
{
    public class RoomSearchViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public List<List<int>> Interests { get; set; }
        public int MembersCount { get; set; }

        public RoomSearchViewModel(Models.Room room)
        {
            this.Id = room.Id;
            this.Name = room.Name;
            this.Description = room.Description;
            this.Interests = InterestsService.ConvertInterestsToLists(room.Interests);
            this.StartDate = room.StartDate;
            this.MembersCount = room.Members.Count;
        }
    }

    public class RoomSearchByFollowingsViewModel : RoomSearchViewModel
    {
        public List<AppUserSearchViewModel> SuggestBy { get; set; }
        public RoomSearchByFollowingsViewModel(Models.Room room, List<AppUser> suggestBy) : base(room)
        {
            this.SuggestBy = suggestBy.Select(user => new AppUserSearchViewModel(user)).ToList();
        }
    }
}
