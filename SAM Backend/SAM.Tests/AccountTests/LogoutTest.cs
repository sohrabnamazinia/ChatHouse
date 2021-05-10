using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SAM_Backend.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SAM.Tests
{
    [TestClass]
    public class LogoutTest : TestBase
    {
        [TestMethod]
        public void Logout_Successful_200()
        {
            #region setup
            var trueCode = 200;
            #endregion

            #region create controller
            var controllerContext = new ControllerContext()
            {
                HttpContext = mockContext.Object
            };
            var controller = new AccountController(mockUserManager.Object, mockSigninManager.Object, mockAccountLogger.Object, mockJWTHandler.Object, mockDPProvider.Object, context, MockMinio.Object)
            {
                ControllerContext = controllerContext,
                Url = mockUrl.Object
            };
            #endregion

            #region test model
            #endregion

            #region logout
            ActionResult result = controller.Logout();
            int code = getResponse_200(result);
            #endregion

            #region Assertion
            Assert.AreEqual(trueCode, code);
            #endregion

        }

        #region get responses
        private int getResponse_200(ActionResult result)
        {
            OkResult actionResult = (OkResult)result;
            var code = actionResult.StatusCode;
            return code;
        }

        #endregion
    }
}
