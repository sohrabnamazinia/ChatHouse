using Microsoft.VisualStudio.TestTools.UnitTesting;
using SAM_Backend.Models;
using SAM.Tests;
using Moq;
using Microsoft.AspNetCore.Mvc;
using SAM_Backend.Controllers;
using SAM_Backend.ViewModels.Account;
using System.Threading.Tasks;
using System;

namespace SAM.Tests
{
    [TestClass]
    public class LoginTest : TestBase
    {
        [TestMethod]
        public void Login_Successful_200()
        {
            #region Setup
            mockUserManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new AppUser());
            mockSigninManager.Setup(x => x.PasswordSignInAsync(It.IsAny<AppUser>(), It.IsAny<string>(), It.IsAny<bool>(), true)).ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);
            mockJWTHandler.Setup(x => x.GenerateToken(It.IsAny<AppUser>())).Returns("JWT");
            var trueCode = 200;
            #endregion Setup

            #region create controller
            var controllerContext = new ControllerContext()
            {
                HttpContext = mockContext.Object
            };
            var controller = new AccountController(mockUserManager.Object, mockSigninManager.Object, mockAccountLogger.Object, mockJWTHandler.Object, mockDPProvider.Object, context, MockMinio.Object, MockEmail.Object)
            {
                ControllerContext = controllerContext,
                Url = mockUrl.Object
            };
            #endregion

            #region test model
            var model = new LoginViewModel()
            {
                Identifier = "testemail@yahoo.com",
                Password = "testpassword",
                IsEmail = true
            };
            #endregion

            #region login
            Task<ActionResult> result  = controller.Login(model);
            int code = getResponse_200(result);
            #endregion

            #region Assertion
            Assert.AreEqual(trueCode, code);
            #endregion
        }

        [TestMethod]
        public void Login_UserNotFound_404()
        {
            #region Setup
            mockUserManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(null, TimeSpan.FromMilliseconds(1));
            var trueCode = 404;
            #endregion Setup

            #region create controller
            var controllerContext = new ControllerContext()
            {
                HttpContext = mockContext.Object
            };
            var controller = new AccountController(mockUserManager.Object, mockSigninManager.Object, mockAccountLogger.Object, mockJWTHandler.Object, mockDPProvider.Object, context, MockMinio.Object, MockEmail.Object)
            {
                ControllerContext = controllerContext,
                Url = mockUrl.Object
            };
            #endregion

            #region test model
            var model = new LoginViewModel()
            {
                Identifier = "noUser@yahoo.com",
                Password = "thisUserDoesNotExist",
                IsEmail = true
            };
            #endregion

            #region login
            Task<ActionResult> result = controller.Login(model);
            int code = getResponse_404(result);
            #endregion

            #region Assertion
            Assert.AreEqual(trueCode, code);
            #endregion
        }

        [TestMethod]
        public void Login_InvalidModelState_400()
        {
            #region Setup
            mockUserManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new AppUser());
            mockSigninManager.Setup(x => x.PasswordSignInAsync(It.IsAny<AppUser>(), It.IsAny<string>(), It.IsAny<bool>(), true)).ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);
            mockJWTHandler.Setup(x => x.GenerateToken(It.IsAny<AppUser>())).Returns("JWT");
            var trueCode = 400;
            #endregion Setup

            #region create controller
            var controllerContext = new ControllerContext()
            {
                HttpContext = mockContext.Object
            };
            var controller = new AccountController(mockUserManager.Object, mockSigninManager.Object, mockAccountLogger.Object, mockJWTHandler.Object, mockDPProvider.Object, context, MockMinio.Object, MockEmail.Object)
            {
                ControllerContext = controllerContext,
                Url = mockUrl.Object
            };
            controller.ModelState.AddModelError(string.Empty, "Invalid Model State");
            #endregion

            #region test model
            var model = new LoginViewModel()
            {
                // Invalid email format
                Identifier = "testemail.com",
                Password = "testpassword",
                IsEmail = true
            };
            #endregion

            #region login
            Task<ActionResult> result = controller.Login(model);
            int code = getResponse_400(result);
            #endregion

            #region Assertion
            Assert.AreEqual(trueCode, code);
            #endregion
        }

        [TestMethod]
        public void Login_NotConfirmedEmail_401()
        {
            #region Setup
            mockUserManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new AppUser());
            mockSigninManager.Setup(x => x.PasswordSignInAsync(It.IsAny<AppUser>(), It.IsAny<string>(), It.IsAny<bool>(), true)).ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.NotAllowed);
            mockJWTHandler.Setup(x => x.GenerateToken(It.IsAny<AppUser>())).Returns("JWT");
            var trueCode = 401;
            #endregion Setup

            #region create controller
            var controllerContext = new ControllerContext()
            {
                HttpContext = mockContext.Object
            };
            var controller = new AccountController(mockUserManager.Object, mockSigninManager.Object, mockAccountLogger.Object, mockJWTHandler.Object, mockDPProvider.Object, context, MockMinio.Object, MockEmail.Object)
            {
                ControllerContext = controllerContext,
                Url = mockUrl.Object
            };
            #endregion

            #region test model
            var model = new LoginViewModel()
            {
                // Email not confirmed
                Identifier = "testemail@yahoo.com",
                Password = "testpassword",
                IsEmail = true
            };
            #endregion

            #region login
            Task<ActionResult> result = controller.Login(model);
            int code = getResponse_401(result);
            #endregion

            #region Assertion
            Assert.AreEqual(trueCode, code);
            #endregion
        }

        [TestMethod]
        public void Login_LockedoutAccount_423()
        {
            #region Setup
            mockUserManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new AppUser());
            mockSigninManager.Setup(x => x.PasswordSignInAsync(It.IsAny<AppUser>(), It.IsAny<string>(), It.IsAny<bool>(), true)).ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.LockedOut);
            mockJWTHandler.Setup(x => x.GenerateToken(It.IsAny<AppUser>())).Returns("JWT");
            var trueCode = 423;
            #endregion Setup

            #region create controller
            var controllerContext = new ControllerContext()
            {
                HttpContext = mockContext.Object
            };
            var controller = new AccountController(mockUserManager.Object, mockSigninManager.Object, mockAccountLogger.Object, mockJWTHandler.Object, mockDPProvider.Object, context, MockMinio.Object, MockEmail.Object)
            {
                ControllerContext = controllerContext,
                Url = mockUrl.Object
            };
            #endregion

            #region test model
            var model = new LoginViewModel()
            {
                // User is locked out
                Identifier = "LockedoutUser@yahoo.com",
                Password = "testPassword",
                IsEmail = true
            };
            #endregion

            #region login
            Task<ActionResult> result = controller.Login(model);
            int code = getResponse_423(result);
            #endregion

            #region Assertion
            Assert.AreEqual(trueCode, code);
            #endregion
        }






        #region get responses
        public int getResponse_200(Task<ActionResult> result)
        {
            OkObjectResult actionResult = (OkObjectResult)result.Result;
            var code = actionResult.StatusCode.Value;
            return code;
        }

        public int getResponse_404(Task<ActionResult> result)
        {
            NotFoundObjectResult actionResult = (NotFoundObjectResult)result.Result;
            var code = actionResult.StatusCode;
            return code.Value;
        }

        public int getResponse_400(Task<ActionResult> result)
        {
            BadRequestObjectResult actionResult = (BadRequestObjectResult)result.Result;
            var code = actionResult.StatusCode;
            return code.Value;
        }

        public int getResponse_401(Task<ActionResult> result)
        {
            UnauthorizedObjectResult actionResult = (UnauthorizedObjectResult)result.Result;
            var code = actionResult.StatusCode;
            return code.Value;
        }

        public int getResponse_423(Task<ActionResult> result)
        {
            ObjectResult actionResult = (ObjectResult)result.Result;
            var code = actionResult.StatusCode;
            return code.Value;
        }

        #endregion

    }
}
