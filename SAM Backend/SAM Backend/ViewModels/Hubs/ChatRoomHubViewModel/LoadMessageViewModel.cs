using SAM_Backend.Models;
using SAM_Backend.ViewModels.ChatRoomHubViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAM_Backend.ViewModels.Hubs.ChatRoomHubViewModel
{
    public class LoadMessageViewModel
    {
        public LoadMessageViewModel(RoomMessage message)
        {
            Id = message.Id;
            ParentId = message.Parent != null ? message.Parent.Id : -1;
            Content = message.Content;
            LinkIfImage = message.LinkIfImage;
            ContetntType = message.ContentType;
            SentDate = message.SentDate;
            RoomId = message.Room.Id;
            Sender = new ChatRoomHubUserViewModel()
            {
                FirstName = message.Sender.FirstName,
                LastName = message.Sender.LastName,
                Username = message.Sender.UserName,
                ImageLink = message.Sender.ImageLink,
            };
        }
        public int Id { get; set; }
        public int ParentId { get; set; }
        public string Content { get; set; }
        public MessageType ContetntType { get; set; }
        public DateTime SentDate { get; set; }
        public ChatRoomHubUserViewModel Sender { get; set; }
        public bool IsMe { get; set; }
        public int RoomId { get; set; }
        public string LinkIfImage { get; set; }
    }
}
