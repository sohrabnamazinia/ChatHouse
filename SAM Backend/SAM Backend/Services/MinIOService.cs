using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Minio;
using SAM_Backend.Models;
using SAM_Backend.Utility;
using SAM_Backend.ViewModels.ChatRoomHubViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SAM_Backend.Services
{
    public class MinIOService : IMinIOService
    {
        #region Fields
        private MinioClient minio;
        private readonly IConfiguration _config;
        private readonly AppDbContext context;
        #endregion

        public MinIOService(IConfiguration configuration, AppDbContext context)
        {
            #region Instantiation
            _config = configuration;
            this.context = context;
            minio = new MinioClient(Constants.MinIOHostAddress, _config.GetValue<string>("MinIOAccessKey"), _config.GetValue<string>("MinIOSecretKey")); 
            #endregion
        }

        public async Task<MinIOResponseModel> UpdateUserImage(IFormFileCollection fileCollection, AppUser user)
        {
            #region check errors
            var response = new MinIOResponseModel();
            
            if (fileCollection == null)
            {
                response.Message = "file not found!";
                return response;
            }

            if (fileCollection.Count != 1)
            {
                response.Message = "there must be exactly one input file!";
                return response;
            }

            var imageFile = fileCollection[0];

            if (imageFile == null || !(imageFile.Length > 0))
            {
                response.Message = "Image not fonud!";
                return response;
            }

            using (var ms = new MemoryStream())
            {
                imageFile.CopyTo(ms);
                var fileBytes = ms.ToArray();
                if (!FileService.FileFormatChecker(fileBytes) || !FileService.CheckFileNameExtension(Path.GetExtension(imageFile.FileName)))
                {
                    response.Message = "file format is invalid";
                    return response;
                }
            }

            if (imageFile.Length > Constants.MaxUserImageSizeByte)
            {
                response.Message = "image size exceeds the limitation\nthe limitation is : " + Constants.MaxUserImageSizeByte + "Bytes";
                return response;
            }

            #endregion check errors

            #region remove previous image
            if (user.ImageLink != null)
            {
                await minio.RemoveObjectAsync(Constants.MinIOBucketUsers, user.Id);
                user.ImageLink = null;
            }
            #endregion remove previous image

            #region upload
            var ImageName = FileService.CreateObjectName(imageFile.FileName, user.Id);
            await minio.PutObjectAsync(Constants.MinIOBucketUsers, ImageName, imageFile.OpenReadStream(), imageFile.Length);
            var link = await GenerateUrlUserImage(user.Id, ImageName);
            if (link == null)
            {
                response.Message = "image link is created null";
                return response;
            }
            user.ImageLink = link;
            user.ImageName = ImageName;
            #endregion upload

            #region return 
            context.SaveChanges();
            response.Done = true;
            return response;
            #endregion return
        }

        public async Task<string> GenerateUrlUserImage(string id, string fileName)
        {
            #region check input
            if (id == null || fileName == null) return null;
            #endregion

            #region return
            return await minio.PresignedGetObjectAsync(Constants.MinIOBucketUsers, FileService.CreateObjectName(fileName, id), Constants.PresignedGetObjectExpirationPeriod);
            #endregion
        }

        public async Task<string> GenerateUrlRoomImageMessage(string roomId, string fileName)
        {
            #region check input
            if (roomId == null || fileName == null) return null;
            #endregion

            #region return
            return await minio.PresignedGetObjectAsync(roomId, fileName, Constants.PresignedGetObjectExpirationPeriod);
            #endregion
        }

        public async Task<int> RemoveImage(AppUser user)
        {
            await minio.RemoveObjectAsync(Constants.MinIOBucketUsers, user.ImageName);
            user.ImageName = null;
            user.ImageLink = null;
            context.SaveChanges();
            return 1;
        }

        public async Task<MinIOResponseModel> UploadRoomImageMessage(IFormFileCollection fileCollection, AppUser user, Room room, int parentId)
        {
            #region check failed cases
            var response = new MinIOResponseModel();
            if (fileCollection == null)
            {
                response.Message = "file not found!";
                return response;
            }

            if (fileCollection.Count != 1)
            {
                response.Message = "there must be exactly one input file!";
                return response;
            }

            var file = fileCollection[0];

            if (user == null)
            {
                response.Message = "Room not found";
                return response;
            }

            if (file == null)
            {
                response.Message = "File not found";
                return response;
            }

            if (!(file.Length > 0))
            {
                response.Message = "Image Size = 0!";
                return response;
            }

            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                var fileBytes = ms.ToArray();
                if (!FileService.FileFormatChecker(fileBytes) || !FileService.CheckFileNameExtension(Path.GetExtension(file.FileName)))
                {
                    response.Message = "file format is invalid";
                    return response;
                }
            }

            if (file.Length > Constants.MaxUserImageSizeByte)
            {
                response.Message = "image size exceeds the limitation\nthe limitation is : " + Constants.MaxUserImageSizeByte + "Bytes";
                return response;
            }
            #endregion

            #region minio & Db
            var ImageName = FileService.CreateRoomFileMessageName(file.FileName);
            if (!await minio.BucketExistsAsync(GetRoomBucketName(room.Id)))
            {
                await minio.MakeBucketAsync(GetRoomBucketName(room.Id));
            }
            await minio.PutObjectAsync(GetRoomBucketName(room.Id), ImageName, file.OpenReadStream(), file.Length);
            var link = await GenerateUrlRoomImageMessage(GetRoomBucketName(room.Id), ImageName);
            if (link == null)
            {
                response.Message = "image link is created null";
                return response;
            }

            #region find message parent
            RoomMessage Parent = await context.RoomsMessages.FindAsync(parentId);
            #endregion

            RoomMessage message = new RoomMessage()
            {
                // content = imageName if message is image
                Content = ImageName,
                LinkIfImage = link,
                ContentType = MessageType.ImageFile,
                Parent = Parent,
                Room = room,
                Sender = user,
                SentDate = DateTime.Now,
            };
            response.roomImageMessage = message;
            context.RoomsMessages.Add(message);
            #endregion

            #region return
            context.SaveChanges();
            response.Done = true;
            return response;
            #endregion
        }

        public string GetRoomBucketName(int id)
        {
            return "room-" + id.ToString();
        }
    }
}
