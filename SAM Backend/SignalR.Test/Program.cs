using Microsoft.AspNetCore.SignalR.Client;
using SignalR.Test;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SignalRClient
{
    class Program
    {
        public static void Main(string[] args)
        {
            //#region Set delay for main project setup
            //Thread.Sleep(5000);
            //#endregion

            #region Test playGorund
            var token1 = ClientUtils.token1;
            var client = new ChatRoomHubClient(token1);
            client.Connect();
            client.DefineMethods();
            client.SendMessage("Sohrab", "Hi!");
            var client2 = new ChatRoomHubClient(token1);
            client2.Connect();
            client2.DefineMethods();
            client2.SendMessage("Sohrab", "Hello!");
            #endregion Test playGorund
        }

    }
}
