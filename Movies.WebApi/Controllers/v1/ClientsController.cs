using System;
using System.Threading.Tasks;
using System.Web.Http;
using Movies.Model.BindingModels.Client;
using Movies.Service.Client;
using Movies.WebApi.Utility.Filter;
using Movies.WebApi.Utility.VersioningPrefix;

namespace Movies.WebApi.Controllers.v1
{
    [ApiVersion1RoutePrefix("clients")]
    [MyAuthorize(Roles = "Admin")]
    public class ClientsController : BaseApiController
    {
        private readonly IClientService _clientService;
        public ClientsController(IClientService clientService)
        {
            _clientService = clientService;
        }


        [Route("", Name = "GetClients")]
        [HttpGet]
        [CheckPaginationQuery]
        public async Task<IHttpActionResult> GetClients(int page = 1, int pageSize = 20)
            => Ok(await _clientService.GetAll(page, pageSize, "GetClients"));


        [Route("{id}", Name = "GetClient")]
        [HttpGet]
        public async Task<IHttpActionResult> GetClient(string id)
            => Ok(await _clientService.GetById(id));


        [Route("", Name = "CreateClient")]
        [HttpPost]
        public async Task<IHttpActionResult> CreateClient(CreateClientBindingModel clientModel)
        {
            var newClient = await _clientService.CreateClient(clientModel);
            Uri locationHeader = new Uri(Url.Link("GetClient", new { id = newClient.Id }));
            return Created(locationHeader, newClient);
        }


        [Route("{id}", Name = "UpdateClient")]
        [HttpPut]
        public async Task<IHttpActionResult> UpdateClient(string id, UpdateClientBindingModel clientModel)
            => Ok(await _clientService.UpdateClient(id, clientModel));


        [Route("{id}", Name = "DeleteClient")]
        [HttpDelete]
        public async Task<IHttpActionResult> DeleteClient(string id)
        {
            await _clientService.Delete(id);
            return Ok();
        }
    }
}