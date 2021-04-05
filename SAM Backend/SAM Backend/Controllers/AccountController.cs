using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
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
        private readonly IMinIOService minIOService;
        private readonly IDataProtector protector;
        #endregion

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ILogger<AccountController> logger, IJWTService jWTService, IDataProtectionProvider dataProtectionProvider, AppDbContext context, IMinIOService minIOService)
        {
            #region Instantiation
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
            this.jWTService = jWTService;
            this.dataProtectionProvider = dataProtectionProvider;
            this.context = context;
            this.minIOService = minIOService;
            this.protector = dataProtectionProvider.CreateProtector(DataProtectionPurposeStrings.UserIdQueryString);
            this.minIOService = minIOService;
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
            if (!model.Username.IsAllowedUsername()) return BadRequest("Username contains unallowed characters");
            #endregion

            #region Signup attempt
            var newUser = new AppUser() { Email = model.Email, UserName = model.Username, Followers = new List<AppUser>(), Followings = new List<AppUser>(), Interests = new Interests() };
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


        [HttpPost]
        [Authorize]
        public ActionResult Logout()
        {
            // TODO: insert token to black list
            return Ok();
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
            user.ImageLink = await minIOService.GenerateUrl(user.Id, user.ImageName);
            model.ImageLink = user.ImageLink;
            #endregion Set IsMe

            #region Return model
            context.SaveChanges();
            return Ok(model);
            #endregion Return model
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> Follow(string username)
        {
            #region Find users
            var follower = await jWTService.FindUserByTokenAsync(Request, context);
            var followed = await userManager.FindByNameAsync(username);
            if (followed == null) return NotFound(Constants.UserNotFoundError);
            if (follower == followed) return BadRequest("Users can not follow themselves!");
            if (follower.Followings.Contains(followed)) return BadRequest("This following relationship already exists!");
            #endregion Find users

            #region Follow
            follower.Followings.Add(followed);
            followed.Followers.Add(follower);
            #endregion Follow

            #region Return
            context.SaveChanges();
            return Ok(new AppUserViewModel(follower));
            #endregion Return
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> UnFollow(string username)
        {
            #region Find users
            var unFollower = await jWTService.FindUserByTokenAsync(Request, context);
            var unFollowed = await userManager.FindByNameAsync(username);
            if (unFollowed == null) return NotFound(Constants.UserNotFoundError);
            if (unFollower == unFollowed) return BadRequest("Users can not unfollow themselves!");
            if (!unFollower.Followings.Contains(unFollowed)) return BadRequest("There is not such following relationship");
            #endregion Find users

            #region Unfollow
            unFollower.Followings.Remove(unFollowed);
            unFollowed.Followers.Remove(unFollower);
            #endregion Unfollow

            #region Return
            context.SaveChanges();
            return Ok(new AppUserViewModel(unFollower));
            #endregion Return
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> UpdateProfile(UpdateProfileViewModel model)
        {
            #region find user
            var user = await jWTService.FindUserByTokenAsync(Request, context);
            #endregion find user

            #region check username
            if (model.Username != null)
            {
                var isFree = IsFreeUsername(model.Username).Result.StatusCode;
                if (isFree != Constants.OKStatuseCode) return BadRequest("Username is taken");
                if (!model.Username.IsAllowedUsername()) return BadRequest("Username contains not allowed characters");
                user.UserName = model.Username;
            }
            #endregion check username

            #region Interests
            if (model.Interests != null)
            {
                var updatedInterests = model.Interests;
                if (updatedInterests.Count != Constants.InterestCategoriesCount) return BadRequest("List does not contain 14 inner lists!");
                InterestsService.SetInterests(updatedInterests, user);
            }
            #endregion Interests

            #region Name 
            user.FirstName = model.FirstName != null ? model.FirstName : user.FirstName;
            user.LastName = model.LastName != null ? model.LastName : user.LastName;
            #endregion Name 

            #region bio
            user.Bio = model.Bio != null ? model.Bio : user.Bio;
            #endregion bio

            #region return
            await userManager.UpdateAsync(user);
            return Ok(new AppUserViewModel(user));
            #endregion return
        }

        [HttpPost]
        [Authorize]
        [FileUploadOperation.FileContentType]
        public async Task<ActionResult> UpdateImage(IFormFile fileUpload)
        {
            #region find user
            var user = await jWTService.FindUserByTokenAsync(Request, context);
            #endregion find user

            #region minio
            var files = Request.Form.Files;
            MinIOResponseModel minioResponse = await minIOService.UpdateUserImage(files, user);
            if (!minioResponse.Done) return BadRequest(minioResponse.Message);
            #endregion minio

            #region return
            context.SaveChanges();
            return Ok(new UpdateImageViewModel(user));
            #endregion return
        }

        #region TODO After Deploy

        [HttpGet]
        public async Task<ObjectResult> IsFreeUsername(string username)
        {
            #region Check Db
            var user = await userManager.FindByNameAsync(username);
            if (user == null) return Ok("Username is free!");
            return BadRequest("Username is already occupied!");
            #endregion Check Db
        }

        #endregion TODO
    }
}
