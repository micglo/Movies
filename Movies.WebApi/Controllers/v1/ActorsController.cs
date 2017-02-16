using System.Threading.Tasks;
using System.Web.Http;
using Movies.Service.Movie;
using Movies.WebApi.Utility.Filter;
using Movies.WebApi.Utility.VersioningPrefix;

namespace Movies.WebApi.Controllers.v1
{
    [ApiVersion1RoutePrefix("actors")]
    public class ActorsController : BaseApiController
    {
        private readonly IMovieService _movieService;

        public ActorsController(IMovieService movieService)
        {
            _movieService = movieService;
        }


        [Route("", Name = "GetActors")]
        [HttpGet]
        [CheckPaginationQuery]
        public IHttpActionResult GetActors(int page = 1, int pageSize = 20)
            => Ok(_movieService.GetAllPersons(page, pageSize, "GetActors", "actor"));



        [Route("{lastName}/Movies", Name = "GetMoviesByActor")]
        [HttpGet]
        [CheckPaginationQuery]
        public async Task<IHttpActionResult> GetMoviesByActor(string lastName, int page = 1, int pageSize = 20)
            => Ok(await _movieService.GetMoviesByActor(lastName, page, pageSize, "GetMoviesByActor"));
    }
}