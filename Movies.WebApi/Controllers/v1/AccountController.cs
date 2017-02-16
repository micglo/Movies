using System;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Movies.Domain.Entity;
using Movies.Mapper.Factory.User;
using Movies.Model.BindingModels.Account;
using Movies.Model.User;
using Movies.WebApi.IdentityConfig;
using Movies.WebApi.Utility.VersioningPrefix;

namespace Movies.WebApi.Controllers.v1
{
    [ApiVersion1RoutePrefix("account")]
    public class AccountController : BaseApiController
    {
        private readonly UserManager _userManager;
        private readonly IUserFactory _userFactory;
        public AccountController(UserManager userManager, IUserFactory userFactory)
        {
            _userManager = userManager;
            _userFactory = userFactory;
        }


        [Route("Register", Name = "Register")]
        [HttpPost]
        public async Task<IHttpActionResult> Register(AccountRegisterBindingModel userModel)
        {
            var userEntity = _userFactory.GetModel(userModel);
            var user = (User)userEntity;
            IdentityResult registerResult = await _userManager.CreateAsync(user, userModel.Password);

            if (!registerResult.Succeeded)
                return GetErrorResult(registerResult);

            await SendEmailConfirmationTokenAsync(user.Id, "Confirm your account.");
            var userDto = _userFactory.GetModel<RegisteredUserDto>(userEntity);
            Uri locationHeader = new Uri(Url.Link("GetUser", new { id = user.Id }));
            return Created(locationHeader, userDto);
        }


        [AllowAnonymous]
        [Route("ResendActivationLink", Name = "ResendActivationLink")]
        [HttpPost]
        public async Task<IHttpActionResult> ResendActivationLink(ResendActivationLinkBindingModel model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
                return NotFound();

            if (user.EmailConfirmed)
                return BadRequest("Email is already confirmed.");

            var result = await _userManager.CheckPasswordAsync(user, model.Password);

            if (result)
            {
                await SendEmailConfirmationTokenAsync(user.Id, "Confirm your account.");
                return Ok();
            }

            return BadRequest("Invalid email or password.");
        }


        [AllowAnonymous]
        [Route("ConfirmEmail", Name = "ConfirmEmail")]
        [HttpGet]
        public async Task<IHttpActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
                return BadRequest("User Id and Code are required.");

            var user = await _userManager.FindByIdAsync(userId);

            if (user.EmailConfirmed)
                return BadRequest($"Email: {user.Email} is already confirmed.");

            var result = await _userManager.ConfirmEmailAsync(userId, code);

            return result.Succeeded ? Ok() : GetErrorResult(result);
        }


        #region Helpers

        private async Task SendEmailConfirmationTokenAsync(string userId, string subject)
        {
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(userId);
            var callbackUrl = new Uri(Url.Link("ConfirmEmail", new { userId, code }));
            await _userManager.SendEmailAsync(userId, subject,
               "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");
        }

        #endregion
    }
}