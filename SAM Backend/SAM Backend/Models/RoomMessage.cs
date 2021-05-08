using SAM_Backend.ViewModels.ChatRoomHubViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SAM_Backend.Models
{
    public class RoomMessage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public virtual Room Room { get; set; }
        public virtual AppUser Sender { get; set; }
        public virtual MessageType ContentType { get; set; }
        public string Content { get; set; }
        public virtual RoomMessage Parent { get; set; }
        public virtual DateTime SentDate { get; set; }
    }
}
