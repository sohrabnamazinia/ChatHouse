using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SAM_Backend.ViewModels.Room
{
    public class CreateRoomViewModel
    {
        [Required]
        public string Name { get; set; }

    }
}
