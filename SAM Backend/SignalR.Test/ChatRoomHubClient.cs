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
            connection.On<string>("ReceiveMessage", (message) =>
            {
                Console.WriteLine(message);
            });
        }

        public void SendMessage(string user, string message)
        {
            try
            {
                connection.InvokeAsync("SendMessage", user, message).Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        #endregion Methods
    }
}
