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
            Creator = room.Creator.UserName;
            Members = room.Members.Select(x => x.UserName).ToList();
            StartDate = room.StartDate;
            EndDate = room.EndDate;
            Interests = InterestsService.ConvertInterestsToLists(room.Interests);
            Name = room.Name;
            Description = room.Description;
        }

        public string Creator { get; set; }
        public List<string> Members { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<List<int>> Interests { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

    }
}
