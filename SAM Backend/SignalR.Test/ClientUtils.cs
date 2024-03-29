﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalR.Test
{
    public static class ClientUtils
    {
        public const string HubsRoute = "http://127.0.0.1:13524/Hubs";
        public const string ChatRoomHubRoute = HubsRoute + "/ChatRoom";

        #region define test JWT strings
        // For DEMO
        public const string token1 = "Bearer eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiJ4eHgiLCJVc2VySWQiOiJhM2YyMTFiZS1kYTVkLTQ4ZGUtODY3OS0xNWU4MWZhZjg2NjUiLCJFbWFpbCI6Inh4eEBleGFtcGxlLmNvbSIsIm5iZiI6MTYyMzU2NzQxMSwiZXhwIjoxNjU1MTAzNDExLCJpYXQiOjE2MjM1Njc0MTF9.pf7E-deseO1DEeQ7tqE94cD6hqDklJ_NIJS_7u-9xog0qzW6tmNbMTM0vicT6a9hTsnNXZB3cr2kwTILTdAkBw";

        public const string token2 = "Bearer eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiJzc3MiLCJVc2VySWQiOiI0NmI4OTFjMS0wYmQ4LTRhNTEtOTk1ZS1kN2FkODRmZjNjZWUiLCJFbWFpbCI6InNzc0BleGFtcGxlLmNvbSIsIm5iZiI6MTYyMjg4NDg2MCwiZXhwIjoxNjU0NDIwODYwLCJpYXQiOjE2MjI4ODQ4NjB9.aY9LuTlt9ztVo6V9TloQDrpwXbntg5ftqxr6j1rrlB9qnNJu4-PWu6c8YL0rmJO6I7nAmFhtw1HqpqjW2C04aw";
        #endregion
    }
}
