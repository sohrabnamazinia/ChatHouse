using SAM_Backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAM_Backend.ViewModels.Account
{
    public class GetProfileViewModel : AppUserViewModel
    {
        public GetProfileViewModel(AppUser user) : base(user)
        {
                
        }
        public bool IsMe { get; set; }

    }
}
