using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAM_Backend.Controllers
{

    [ApiController]
    [AllowAnonymous]
    [Route("api/[controller]/[action]")]
    public class AccountController : ControllerBase
    {
        [HttpPost]
        public string Login()
        {
            return "Hello";
        }

        [HttpPost]
        public string Signup()
        {
            return "Hello";
        }

        [HttpPost]
        [Authorize]
        public string Logout()
        {
            return "Hello";
        }
    }
}
