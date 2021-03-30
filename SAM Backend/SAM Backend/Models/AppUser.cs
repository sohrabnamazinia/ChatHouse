using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAM_Backend.Models
{
    public class AppUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Bio { get; set; }
        public virtual ICollection<AppUser> Followers { get; set; }
        public virtual ICollection<AppUser> Followings { get; set; }
        public virtual Interests Interests { get; set; }
    }
}
