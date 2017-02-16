using System;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Movies.Model.BindingModels.Account;
using Movies.Service.Client;
using Movies.Service.User;
using Movies.WebApi.IdentityConfig;
using Movies.WebApi.Utility.Filter;
using Movies.WebApi.Utility.VersioningPrefix;

namespace Movies.WebApi.Controllers.v1
{
    [ApiVersion1RoutePrefix("myAccount")]
    [MyAuthorize]
    public class MyAccountController : BaseApiController
    {
        private readonly UserManager _userManager;
        private readonly IUserService _userService;
        private readonly IClientService _clientService;
        public MyAccountController(UserManager userManager, IClientService clientService, IUserService userService)
        {
            _userManager = userManager;
            _userService = userService;
            _clientService = clientService;
        }


        [Route("", Name = "MyAccount")]
        [HttpGet]
        public async Task<IHttpActionResult> GetMyAccount()
        {
            var userId = User.Identity.GetUserId();
            return Ok(await _userService.GetMyAccount(userId));
        }


        [Route("Clients", Name = "MyClients")]
        [HttpGet]
        [CheckPaginationQuery]
        public async Task<IHttpActionResult> GetMyClients(int page = 1, int pageSize = 20)
        {
            var userId = User.Identity.GetUserId();
            var clients = await _clientService.GetMyClients(userId, page, pageSize, "MyClients");

            return Ok(clients);
        }


        [Route("Clients/{id}", Name = "MyClient")]
        [HttpGet]
        public async Task<IHttpActionResult> GetMyClient(string id)
        {
            var userId = User.Identity.GetUserId();
            var client = await _clientService.GetMyClient(id, userId);
            return Ok(client);
        }


        [Route("AddNewClient", Name = "UserCreateClient")]
        [HttpPost]
        public async Task<IHttpActionResult> UserCreateClient(UserCreateClientBindingModel model)
        {
            var userId = User.Identity.GetUserId();

            var result = await _clientService.UserCreateClient(userId, model);

            await _userManager.SendEmailAsync(userId, "New client", $"{result.Message}");

            var newClient = await _clientService.GetUserCreatedClient(result.Id);
            Uri locationHeader = new Uri(Url.Link("MyClient", new { id = result.Id }));
            return Created(locationHeader, newClient);
        }


        [Route("Clients/{id}/ChangeOrigin", Name = "ChangeClientOrigin")]
        [HttpPost]
        public async Task<IHttpActionResult> ChangeClientOrigin(string id, [FromBody]ChangeClientOriginBindingModel model)
        {
            var userId = User.Identity.GetUserId();

            return Ok(await _clientService.ChangeOrigin(id, userId, model.AllowedOrigin));
        }


        [Route("Clients/{id}/GenerateNewClientSecret", Name = "UserGenerateNewClientSecret")]
        [HttpGet]
        public async Task<IHttpActionResult> UserGenerateNewClientSecret(string id)
        {
            var userId = User.Identity.GetUserId();

            var result = await _clientService.GenerateNewClientSecret(id, userId);

            await _userManager.SendEmailAsync(userId, "Client secret changed", $"{result}");

            return Ok(await _clientService.GetUserCreatedClient(id));
        }


        [Route("Clients/{id}", Name = "DeleteMyClient")]
        [HttpDelete]
        public async Task<IHttpActionResult> DeleteMyClient(string id)
        {
            var userId = User.Identity.GetUserId();

            await _clientService.ClientBelongsToUser(id, userId);
            await _clientService.Delete(id);

            return Ok();
        }


        [Route("ChangePassword", Name = "ChangePassword")]
        [HttpPost]
        public async Task<IHttpActionResult> ChangePassword(ChangePasswordBindingModel model)
        {
            IdentityResult result = await _userManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword,
                model.NewPassword);

            if(!result.Succeeded)
                GetErrorResult(result);

            var userId = User.Identity.GetUserId();
            return Ok(await _userService.GetMyAccount(userId));
        }


        [Route("Update", Name = "UpdateMyAccount")]
        [HttpPut]
        public async Task<IHttpActionResult> UpdateMyAccount(UpdateMyAccountBindingModel model)
        {
            var userId = User.Identity.GetUserId();
            return Ok(await _userService.UpdateMyAccount(userId, model));
        }


        [Route("Delete", Name = "DeleteAccount")]
        [HttpGet]
        public async Task<IHttpActionResult> DeleteAccount()
        {
            var userId = User.Identity.GetUserId();
            await _userService.DeleteMyAccount(userId);

            return Ok();
        }
    }
}