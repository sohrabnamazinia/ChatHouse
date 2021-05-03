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
        public const string token1 = "eyJhbGciOiJIUzUxMiIsInR5cCI6IkpXVCJ9." +
            "eyJuYW1laWQiOiJ4eHgiLCJVc2VySWQiOiJhM2YyMTFiZ" +
            "S1kYTVkLTQ4ZGUtODY3OS0xNWU4MWZhZjg2NjU" +
            "iLCJFbWFpbCI6Inh4eEBleGFtcGxlLmNvbSIsIm5" +
            "iZiI6MTYyMDA0NjQ2OSwiZXhwIjoxNjUxNTgyNDY5LCJpYXQiOjE2MjAwNDY0Njl9." +
            "Li1n958bdbIt2NMWfyem99fTRBP8q9UeAJ1ocN7g" +
            "kJ-7KeFZlDqIN_CkeV7Bq0Kz9BFbHDOA5Z3fRalrXTd2Xw";

        public const string token2 = "abc";
        #endregion
    }
}
