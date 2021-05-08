using System;
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
        public const string token1 = "Bearer eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9.eyJuYW1laWQiOiJ4eH" +
            "giLCJVc2VySWQiOiJhM2YyMTFiZS1kYTVkLTQ4ZGUtODY3OS0xNWU4MWZhZjg2NjUiLCJFbWFpb" +
            "CI6Inh4eEBleGFtcGxlLmNvbSIsIm5iZiI6MTYyMDQxNjY5OCwiZXhwIjoxNjUxOTUyNjk4L" +
            "CJpYXQiOjE2MjA0MTY2OTh9.U9EzbdOGPHxGOEhy0dSm-pVMVZ7Kb0VPfUUFt2s69qnev9zZ2dK" +
            "NYEY1b6e4cST01TB1_PGo4F_NsQvGdlDI-Q";

        public const string token2 = "Bearer eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9" +
            ".eyJuYW1laWQiOiJzc3MiLCJVc2VySWQiOiI0NmI4OTFjMS0w" +
            "YmQ4LTRhNTEtOTk1ZS1kN2FkODRmZjNjZWUiLCJFbWFpbCI6InNzc0BleGFtcGxl" +
            "LmNvbSIsIm5iZiI6MTYyMDQxNjY1OCwiZXhwIjoxNjUxOTUyNjU4LCJpYXQiOjE2MjA0MTY" +
            "2NTh9.GMlCU52iNoXs9PnbPNj9XKT6T8P2z21FVXhLDN62Gy_657ze3Dl3msyn627nQjdtXY2kmrAotcw423O7iUiUOA";
        #endregion
    }
}
