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
            FirstName = user.FirstName;
            LastName = user.LastName;
            Bio = user.Bio;
            Interesets = user.Interests;
        }

        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Bio { get; set; }
        public Interests Interesets { get; set; }
        public string Username { get; set; }
    }
}
