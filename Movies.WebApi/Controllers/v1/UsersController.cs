using System.Threading.Tasks;
using System.Web.Http;
using Movies.Model.BindingModels.User;
using Movies.Service.Client;
using Movies.Service.User;
using Movies.WebApi.Utility.Filter;
using Movies.WebApi.Utility.VersioningPrefix;

namespace Movies.WebApi.Controllers.v1
{
    [ApiVersion1RoutePrefix("users")]
    [MyAuthorize(Roles = "Admin")]
    public class UsersController : BaseApiController
    {
        private readonly IUserService _userService;
        private readonly IClientService _clientService;
        public UsersController(IUserService userService, IClientService clientService)
        {
            _userService = userService;
            _clientService = clientService;
        }


        [Route("", Name = "GetUsers")]
        [HttpGet]
        [CheckPaginationQuery]
        public async Task<IHttpActionResult> GetUsers(int page = 1, int pageSize = 20)
            => Ok(await _userService.GetAll(page, pageSize, "GetUsers"));


        [Route("{id}", Name = "GetUser")]
        [HttpGet]
        public async Task<IHttpActionResult> GetUser(string id)
            => Ok(await _userService.GetById(id));


        [Route("{id}/Roles", Name = "GetUserRoles")]
        [HttpGet]
        [CheckPaginationQuery]
        public async Task<IHttpActionResult> GetUserRoles(string id, int page = 1, int pageSize = 20)
            => Ok(await _userService.GetUserRoles(id, page, pageSize, "GetUserRoles"));


        [Route("{id}/Clients", Name = "GetUserClients")]
        [HttpGet]
        [CheckPaginationQuery]
        public async Task<IHttpActionResult> GetUserClients(string id, int page = 1, int pageSize = 20)
            => Ok(await _clientService.GetUserClients(id, page, pageSize, "GetUserClients"));


        [Route("{id}/AddRoles", Name = "AddRolesToUser")]
        [HttpPost]
        public async Task<IHttpActionResult> AddRolesToUser(string id, UserRolesBindingModel model)
            => Ok(await _userService.AddRolesToUser(id, model.Roles));


        [Route("{id}/RemoveRoles", Name = "RemoveRolesFromUser")]
        [HttpPost]
        public async Task<IHttpActionResult> RemoveRolesFromUser(string id, UserRolesBindingModel model)
            => Ok(await _userService.RemoveRolesFromUser(id, model.Roles));
    }
}