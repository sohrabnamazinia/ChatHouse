using Microsoft.AspNetCore.SignalR.Client;
using SignalR.Test;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace SignalRClient
{
    public class ChatRoomHubClient 
    {
        private readonly string token;
        public HubConnection connection;
        public ChatRoomHubClient(string token)
        {
            #region init connection
            this.token = token;
            connection = new HubConnectionBuilder()
               .WithUrl(ClientUtils.ChatRoomHubRoute, options =>
               {
                   options.AccessTokenProvider = () => Task.FromResult(this.token);
               })
               .WithAutomaticReconnect()
               .Build();
            #endregion
        }

        #region Methods
        public void Connect()
        {
            try
            {
                connection.StartAsync().Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void DefineMethods()
        {
            connection.On<MessageModel>("ReceiveRoomMessage", (messageModel) =>
            {
                int RoomId = messageModel.RoomId;
                int messageType = (int) messageModel.MessageType;
                string message = messageModel.Message.ToString();
                string senderFirstName = messageModel.UserModel.FirstName;
                string senderLastName = messageModel.UserModel.LastName;
                string senderUsername = messageModel.UserModel.Username;
                string senderImageLink = messageModel.UserModel.ImageLink;
                Console.WriteLine("New Message From " + senderFirstName + " " + senderLastName + " To  Room number #" + RoomId + ":");
                Console.WriteLine("\t" + message);

            });

            connection.On<ReceiveRoomNotification>("ReceiveRoomNotification", (ReceiveRoomNotification) =>
            {
                if (ReceiveRoomNotification.Notification == RoomNotification.Join)
                {
                    Console.WriteLine(ReceiveRoomNotification.UserModel.Username + " Joined room number " + ReceiveRoomNotification.RoomId);
                }
                else
                {
                    Console.WriteLine(ReceiveRoomNotification.UserModel.Username + " Left room number " + ReceiveRoomNotification.RoomId);
                }
            });
        }

        public void SendMessageToRoom(MessageModel messageModel)
        {
            try
            {
                connection.InvokeAsync("SendMessageToRoom", messageModel).Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void JoinRoom(JoinRoomViewModel model)
        {
            try
            {
                connection.InvokeAsync("JoinRoom", model).Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void LeaveRoom(LeaveRoomViewModel model)
        {
            try
            {
                connection.InvokeAsync("LeaveRoom", model).Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        #endregion Methods
    }
}
