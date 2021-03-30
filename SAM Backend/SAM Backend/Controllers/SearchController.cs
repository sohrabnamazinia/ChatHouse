using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SAM_Backend.Models;
using SAM_Backend.Services;
using SAM_Backend.Utility;
using SAM_Backend.ViewModels.Account;

namespace SAM_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class SearchController : ControllerBase
    {
        #region Fields

        private readonly UserManager<AppUser> userManager;
        private readonly SignInManager<AppUser> signInManager;
        private readonly ILogger<SearchController> logger;
        private readonly IJWTService jWTService;
        private readonly IDataProtectionProvider dataProtectionProvider;
        private readonly AppDbContext context;
        private readonly IDataProtector protector;

        #endregion

        public SearchController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ILogger<SearchController> logger, IJWTService jWTService, IDataProtectionProvider dataProtectionProvider, AppDbContext context)
        {
            #region Instantiation

            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
            this.jWTService = jWTService;
            this.dataProtectionProvider = dataProtectionProvider;
            this.context = context;
            this.protector = dataProtectionProvider.CreateProtector(DataProtectionPurposeStrings.UserIdQueryString);

            #endregion
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<ICollection<AppUserViewModel>>> Suggest([FromQuery] PaginationParameters pagination)
        {
            List<AppUser> suggestedUsers = await context.Users.OrderByDescending(x => x.Followers.Count).Skip((pagination.PageNumber - 1) * (pagination.PageSize)).Take(pagination.PageSize).ToListAsync();

            return suggestedUsers.Select(x => new AppUserViewModel(x)).ToList();
        }
    }
}
