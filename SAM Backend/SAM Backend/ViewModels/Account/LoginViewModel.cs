using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SAM_Backend.ViewModels.Account
{
    public class LoginViewModel
    {
        [Required]
        public string Identifier { get; set; }
        [Required]
        public string Password { get; set; }
        // If not Email => Username
        [Required]
        public bool IsEmail { get; set; }
    }
}
