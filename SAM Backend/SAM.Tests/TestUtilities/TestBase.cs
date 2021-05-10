using Castle.Core.Configuration;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using SAM.Tests.MockedServices;
using SAM_Backend.Controllers;
using SAM_Backend.Models;
using SAM_Backend.Services;
using SAM_Backend.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SAM.Tests
{
    public abstract class TestBase
    {
        #region mock
        public Mock<FakeUserManager> mockUserManager = new Mock<FakeUserManager>();
        public Mock<FakeSignInManager> mockSigninManager = new Mock<FakeSignInManager>();
        public Mock<ILogger<AccountController>> mockAccountLogger = new Mock<ILogger<AccountController>>();
        public Mock<IJWTService> mockJWTHandler = new Mock<IJWTService>();
        public Mock<HttpContext> mockContext = new Mock<HttpContext>();
        public AppDbContext context = new AppDbContext(new DbContextOptions<AppDbContext>());
        public Mock<IDataProtectionProvider> mockDPProvider = new Mock<IDataProtectionProvider>();
        public Mock<DataProtectionPurposeStrings> mockDPPurposeStrings = new Mock<DataProtectionPurposeStrings>();
        public Mock<IMinIOService> MockMinio = new Mock<IMinIOService>();
        public Mock<IWebHostEnvironment> MockWebHostEnvironment = new Mock<IWebHostEnvironment>();
        public Mock<IConfiguration> Mockconfiguration = new Mock<IConfiguration>();
        public Mock<IUrlHelper> mockUrl = new Mock<IUrlHelper>();

        #endregion
    }
}
