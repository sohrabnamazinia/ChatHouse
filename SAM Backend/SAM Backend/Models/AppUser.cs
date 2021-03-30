using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAM_Backend.Models
{
    public class AppUser : IdentityUser
    {
        public virtual List<AppUser> Followers { get; set; }
        public virtual List<AppUser> Followings { get; set; }
        public virtual Interests Interests { get; set; }
    }
}
