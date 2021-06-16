using Microsoft.AspNetCore.Http;
using SAM_Backend.Models;
using SAM_Backend.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAM_Backend.Services
{
    public interface IMinIOService
    {
        public Task<MinIOResponseModel> UpdateUserImage(IFormFileCollection fileCollection, AppUser user);
        public Task<string> GenerateUrlUserImage(string id, string fileName);
        public Task<string> GenerateUrlRoomImageMessage(string roomId, string fileName);
        public Task<int> RemoveImage(AppUser user);
        public Task<MinIOResponseModel> UploadRoomImageMessage(IFormFileCollection fileCollection, AppUser user, Room room, int parentId);
    }
}
