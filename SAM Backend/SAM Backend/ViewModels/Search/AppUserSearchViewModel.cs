using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SAM_Backend.Models;

namespace SAM_Backend.ViewModels.Search
{
    public class AppUserSearchViewModel
    {
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ImageLink { get; set; }

        public AppUserSearchViewModel(AppUser user)
        {
            this.Username = user.UserName;
            this.FirstName = user.FirstName;
            this.LastName = user.LastName;
            this.ImageLink = user.ImageLink;
        }

        // Image
    }
}
