﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR.Client;
using SAM_Backend.Models;
using SignalR.Test;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SignalRClient
{
    class Program
    {
        public static void Main(string[] args)
        {
            #region Set delay for main project setup & room id
            int RoomId = 4;
            //Thread.Sleep(20000);
            #endregion

            #region create clients
            var token1 = ClientUtils.token1;
            var token2 = ClientUtils.token2;
            var client1 = new ChatRoomHubClient(token1);
            client1.Connect();
            client1.DefineMethods();
            var client2 = new ChatRoomHubClient(token2);
            client2.Connect();
            client2.DefineMethods();
            #endregion create clients

            #region build test models
            ChatRoomHubUserViewModel UserModel1 = new ChatRoomHubUserViewModel()
            {
                FirstName = "Cristiano",
                LastName = "Ronaldo",
                Username = "xxx",
                ImageLink = "ImageLink"
            };

            MessageModel messageModel1 = new MessageModel()
            {
                MessageType = MessageType.Text,
                Message = "Hummmmm",
                UserModel = UserModel1,
                RoomId = RoomId
            };

            JoinRoomViewModel joinRoomViewModel1 = new JoinRoomViewModel()
            {
                RoomId = RoomId,
                UserModel = UserModel1
            };

            LeaveRoomViewModel leaveRoomViewModel1 = new LeaveRoomViewModel()
            {
                RoomId = RoomId,
                UserModel = UserModel1
            };

            ChatRoomHubUserViewModel UserModel2 = new ChatRoomHubUserViewModel()
            {
                FirstName = "Lionel",
                LastName = "Messi",
                Username = "sss",
                ImageLink = "ImageLink2"
            };

            MessageModel messageModel2 = new MessageModel()
            {
                MessageType = MessageType.Text,
                Message = "Hullllll",
                UserModel = UserModel2,
                RoomId = RoomId
            };

            JoinRoomViewModel joinRoomViewModel2 = new JoinRoomViewModel()
            {
                RoomId = RoomId,
                UserModel = UserModel2
            };

            LeaveRoomViewModel leaveRoomViewModel2 = new LeaveRoomViewModel()
            {
                RoomId = RoomId,
                UserModel = UserModel2
            };
            #endregion

            #region run tests
            client1.JoinRoom(joinRoomViewModel1);
            client2.JoinRoom(joinRoomViewModel2);

            //client1.SendMessageToRoom(messageModel1);
            //client2.SendMessageToRoom(messageModel2);

            //client1.LeaveRoom(leaveRoomViewModel1);
            //client2.LeaveRoom(leaveRoomViewModel2);

            client1.LoadRoomAllMessages(RoomId, UserModel1.Username);
            #endregion run tests

            #region not close
            while (true);
            #endregion
        }
    }
}
