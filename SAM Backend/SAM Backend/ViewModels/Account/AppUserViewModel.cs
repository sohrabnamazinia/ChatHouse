using SAM_Backend.Models;
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
        }
        public string Email { get; set; }
        public string Username { get; set; }
    }
}
