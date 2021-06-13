using SAM_Backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAM_Backend.Utility
{
    public class MinIOResponseModel
    {
        public bool Done { get; set; }
        public string Message { get; set; }
        public RoomMessage roomImageMessage { get; set; }
    }
}
