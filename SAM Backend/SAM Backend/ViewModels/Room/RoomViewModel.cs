using SAM_Backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAM_Backend.ViewModels.Room
{
    public class RoomViewModel
    {
        public RoomViewModel(Models.Room room)
        {
            Id = room.Id;
            Creator = new AppUserRoomViewModel()
            {
                FirstName = room.Creator.FirstName,
                LastName = room.Creator.LastName,
                Username = room.Creator.UserName,
                ImageLink = room.Creator.ImageLink,
            };
            Members = room.Members.Select(x => new AppUserRoomViewModel() 
            {
                FirstName = x.FirstName,
                LastName = x.LastName,
                ImageLink = x.ImageLink,
                Username = x.ImageLink
            }).ToList();
            StartDate = room.StartDate;
            EndDate = room.EndDate;
            Interests = InterestsService.ConvertInterestsToLists(room.Interests);
            Name = room.Name;
            Description = room.Description;
        }
        public int Id { get; set; }
        public AppUserRoomViewModel Creator { get; set; }
        public List<AppUserRoomViewModel> Members { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<List<int>> Interests { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

    }
}
