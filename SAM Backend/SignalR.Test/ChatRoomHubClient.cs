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
            connection.On<MessageSenderModel, MessageModel>("ReceiveMessage", (senderModel, messageModel) =>
            {
                Console.WriteLine(senderModel.FirstName + " " + senderModel.LastName + " : " + messageModel.Message.ToString());
            });
        }

        public void SendMessage(MessageSenderModel senderModel, MessageModel messageModel)
        {
            try
            {
                connection.InvokeAsync("SendMessage", senderModel, messageModel).Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        #endregion Methods
    }
}
