using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAM_Backend.Models
{
    public class AppUser : IdentityUser
    {
        public virtual Interests Interests { get; set; }
    }
}
