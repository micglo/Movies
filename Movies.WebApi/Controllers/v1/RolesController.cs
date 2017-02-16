using System.Threading.Tasks;
using System.Web.Http;
using Movies.Model.BindingModels.Role;
using Movies.Service.Role;
using Movies.WebApi.Utility.Filter;
using Movies.WebApi.Utility.VersioningPrefix;

namespace Movies.WebApi.Controllers.v1
{
    [ApiVersion1RoutePrefix("roles")]
    [MyAuthorize(Roles = "Admin")]
    public class RolesController : BaseApiController
    {
        private readonly IRoleService _roleService;
        public RolesController(IRoleService roleService)
        {
            _roleService = roleService;
        }


        [Route("", Name = "GetRoles")]
        [HttpGet]
        [CheckPaginationQuery]
        public async Task<IHttpActionResult> GetRoles(int page = 1, int pageSize = 20)
            => Ok(await _roleService.GetAll(page, pageSize, "GetRoles"));


        [Route("{id}", Name = "GetRole")]
        [HttpGet]
        public async Task<IHttpActionResult> GetRole(string id)
            => Ok(await _roleService.GetById(id));


        [Route("", Name = "CreateRole")]
        [HttpPost]
        public async Task<IHttpActionResult> CreateRole(RoleBindingModel model)
            => Ok(await _roleService.CreateRole(model.Name));


        [Route("{id}", Name = "UpdateRole")]
        [HttpPut]
        public async Task<IHttpActionResult> UpdateRole(string id, [FromBody]RoleBindingModel model)
            => Ok(await _roleService.UpdateRole(id, model.Name));

        
        [Route("{id}", Name = "DeleteRole")]
        [HttpDelete]
        public async Task<IHttpActionResult> DeleteRole(string id)
        {
            await _roleService.DeleteRole(id);

            return Ok();
        }
    }
}