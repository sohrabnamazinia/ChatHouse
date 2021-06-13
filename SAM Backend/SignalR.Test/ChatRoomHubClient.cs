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
                #region get data
                int messageType = (int)messageModel.MessageType;
                int RoomId = messageModel.RoomId;
                string message = messageModel.Message.ToString();
                string senderFirstName = messageModel.UserModel.FirstName;
                string senderLastName = messageModel.UserModel.LastName;
                string senderUsername = messageModel.UserModel.Username;
                string senderImageLink = messageModel.UserModel.ImageLink;
                bool IsMe = messageModel.IsMe;
                #endregion

                #region text
                if (messageModel.MessageType == MessageType.Text)
                {
                    
                    if (IsMe)
                    {
                        Console.WriteLine("Your Message To  Room number #" + RoomId + ":");
                    }
                    else
                    {
                        Console.WriteLine("New Message From " + senderFirstName + " " + senderLastName + " To  Room number #" + RoomId + ":");
                    }
                    Console.WriteLine("\t" + message);
                }
                #endregion text

                #region Image
                else if (messageModel.MessageType == MessageType.ImageFile)
                {
                    if (IsMe)
                    {
                        Console.WriteLine("Your Image Message Link To  Room number #" + RoomId + ":");
                    }
                    else
                    {
                        Console.WriteLine("New Image Message Link From " + senderFirstName + " " + senderLastName + " To  Room number #" + RoomId + ":");
                    }
                    Console.WriteLine("\t" + message);
                }
                #endregion Image

                #region other media type
                else
                {
                    Console.WriteLine("Other media types still not supported");
                }
                #endregion

            });

            connection.On<ReceiveRoomNotification>("ReceiveRoomNotification", (ReceiveRoomNotification) =>
            {
                if (ReceiveRoomNotification.Notification == RoomNotification.Join)
                {
                    if (ReceiveRoomNotification.IsMe)
                    {
                        Console.WriteLine("You joined room number " + ReceiveRoomNotification.RoomId + " (Connection id = " + ReceiveRoomNotification.ConnectionId + " )");
                    }
                    else
                    {
                        Console.WriteLine(ReceiveRoomNotification.UserModel.Username + " Joined room number " + ReceiveRoomNotification.RoomId + " ( connnection id = " + ReceiveRoomNotification.ConnectionId + " )");
                    }
                }
                else
                {
                    if (ReceiveRoomNotification.IsMe)
                    {
                        Console.WriteLine("You left room number " + ReceiveRoomNotification.RoomId + " ( connection id =  " + ReceiveRoomNotification.ConnectionId + " )");
                    }
                    else
                    {
                        Console.WriteLine(ReceiveRoomNotification.UserModel.Username + " Left room number " + ReceiveRoomNotification.RoomId + " ( connection id = " + ReceiveRoomNotification.ConnectionId + " )");
                    }
                }
            });

            connection.On<List<LoadMessageViewModel>>("ReceiveRoomAllMessages", (messages) =>
            {
                Console.WriteLine("Loading Messages:");
                foreach (var x in messages)
                {
                    string sender = "";
                    if (x.IsMe) sender = "You";
                    else sender = x.Sender.Username;
                    if (x.ContetntType == MessageType.Text)
                    {
                        Console.WriteLine(sender + " : " + x.Content + "\t" + x.SentDate.ToString());
                    }
                    else if (x.ContetntType == MessageType.ImageFile)
                    {
                        Console.WriteLine(sender + " : " + x.LinkIfImage + " [Image link] " + "\t" + x.SentDate.ToString());
                    }
                    else if (x.ContetntType == MessageType.JoinNotification)
                    {
                        Console.WriteLine(sender + " Joined room number " + x.RoomId);
                    }
                    else if (x.ContetntType == MessageType.LeftNotification)
                    {
                        Console.WriteLine(sender + " Left room number " + x.RoomId);
                    }
                }
                Console.WriteLine("*********************************");
            });

            connection.On<FinishRoomViewModel>("FinishRoom", (model) => 
            {
                Console.WriteLine("Sorry, Room number #" + model.RoomId + " has been finished!");
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

        public void LoadRoomAllMessages(int RoomId, string Username)
        {
            try
            {
                connection.InvokeAsync("LoadRoomMessages", RoomId, Username).Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        #endregion Methods
    }
}
