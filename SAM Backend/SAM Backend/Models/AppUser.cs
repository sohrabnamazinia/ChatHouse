﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SAM_Backend.Models
{
    public class AppUser : IdentityUser
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public string Bio { get; set; }
        public string ImageLink { get; set; }
        public string ImageName { get; set; }
        public virtual ICollection<AppUser> Followers { get; set; }
        public virtual ICollection<AppUser> Followings { get; set; }
        public virtual Interests Interests { get; set; }
        public virtual ICollection<Room> InRooms { get; set; }
        public virtual ICollection<Room> CreatedRooms { get; set; }
    }
}
