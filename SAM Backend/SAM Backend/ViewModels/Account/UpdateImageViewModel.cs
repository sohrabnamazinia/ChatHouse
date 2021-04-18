using SAM_Backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAM_Backend.ViewModels.Account
{
    public class UpdateImageViewModel : AppUserViewModel
    {
        public UpdateImageViewModel(AppUser user) : base(user)
        {
            ImageLink = user.ImageLink;
        }
        public string ImageLink { get; set; }
    }
}
