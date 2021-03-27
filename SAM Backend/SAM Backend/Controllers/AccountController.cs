using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using SAM_Backend.Models;
using SAM_Backend.Services;
using SAM_Backend.Utility;
using SAM_Backend.ViewModels.Account;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAM_Backend.Controllers
{

    [ApiController]
    [Route("api/[controller]/[action]")]
    public class AccountController : ControllerBase
    {
        #region Fields
        private readonly UserManager<AppUser> userManager;
        private readonly SignInManager<AppUser> signInManager;
        private readonly ILogger<AccountController> logger;
        private readonly IJWTService jWTService;
        private readonly IDataProtectionProvider dataProtectionProvider;
        private readonly AppDbContext context;
        private readonly IDataProtector protector;
        #endregion

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ILogger<AccountController> logger, IJWTService jWTService, IDataProtectionProvider dataProtectionProvider, AppDbContext context)
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

        [HttpPost]
        public async Task<ActionResult> Signup(SignupViewModel model)
        {
            #region check email/username
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user != null) return BadRequest("There is already an account with this email address");
            user = await userManager.FindByNameAsync(model.Username);
            if (user != null) return BadRequest("Username is not available");
            if (!Constants.IsAllowedUsername(model.Username)) return BadRequest("Username contains unallowed characters");
            #endregion

            #region Signup attempt
            var newUser = new AppUser() { Email = model.Email, UserName = model.Username };
            var result = await userManager.CreateAsync(newUser, model.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors) ModelState.AddModelError(string.Empty, error.Description);
                return BadRequest(ModelState);
            }
            #endregion

            #region EmailConfirmation Link
            var EmailConfirmationTokoen = await userManager.GenerateEmailConfirmationTokenAsync(newUser);
            var EncryptedId = protector.Protect(newUser.Id);
            var ConfirmationLink = Url.Action(nameof(ConfirmEmail), "Account", new { id = EncryptedId, token = EmailConfirmationTokoen }, Request.Scheme);
            // TODO: Send email
            logger.LogInformation("EmailConfirmation Link: ${}", ConfirmationLink);
            return Ok(new AppUserViewModel(newUser));
            #endregion
        }

        [HttpGet]
        public async Task<ActionResult> ConfirmEmail(string id, string token)
        {
            #region Check inputs
            if (id == null)
            {
                return NotFound(Constants.UserNotFoundError);
            }
            if (token == null)
            {
                return BadRequest("Token not Found");
            }
            id = protector.Unprotect(id);
            #endregion

            #region Fetch user
            var user = await userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound(Constants.UserNotFoundError);
            }
            #endregion

            #region Confirm email attempt
            var result = await userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded) return BadRequest(Constants.EmailFailedToConfirmedMessage);

            return Ok("Email is confirmed Successfully");
            #endregion
        }

        [HttpPost]
        public async Task<ActionResult> Login(LoginViewModel model)
        {
            #region Find user
            AppUser user = model.IsEmail ? await userManager.FindByEmailAsync(model.Identifier) : await userManager.FindByNameAsync(model.Identifier);
            if (user == null) return NotFound(Constants.UserNotFoundError);
            #endregion

            #region Attempt Signin
            var result = await signInManager.PasswordSignInAsync(user, model.Password, true, true);
            if (result.IsNotAllowed)
            {
                ModelState.AddModelError(string.Empty, "Email is not confirmed");
                return Unauthorized(ModelState);
            }
            if (result.IsLockedOut)
            {
                return StatusCode(423, "Too many Failed attempts! please try later.");
            }

            if (!result.Succeeded)
            {
                return BadRequest("Inavlid login attempt");
            }
            #endregion

            #region Send JWT
            // TODO: remove expired tokens
            var token = jWTService.GenerateToken(user);
            return Ok(token);
            #endregion
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult> GetProfile(string username)
        {
            #region Find user
            var user = await userManager.FindByNameAsync(username);
            if (user == null) return NotFound(Constants.UserNotFoundError);
            #endregion Find user

            #region Set IsMe
            var model = new GetProfileViewModel(user);
            AppUser requester = await jWTService.FindUserByTokenAsync(Request, context);
            model.IsMe = (requester == user) ? true : false;
            #endregion Set IsMe

            #region Return model
            return Ok(model);
            #endregion Return model
        }


        [HttpPost]
        [Authorize]
        public ActionResult Logout()
        {
            // TODO: insert token to black list
            return Ok();
        }
    }
}
