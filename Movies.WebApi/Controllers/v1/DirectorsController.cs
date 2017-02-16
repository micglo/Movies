using System.Threading.Tasks;
using System.Web.Http;
using Movies.Service.Movie;
using Movies.WebApi.Utility.Filter;
using Movies.WebApi.Utility.VersioningPrefix;

namespace Movies.WebApi.Controllers.v1
{
    [ApiVersion1RoutePrefix("directors")]
    public class DirectorsController : BaseApiController
    {
        private readonly IMovieService _movieService;

        public DirectorsController(IMovieService movieService)
        {
            _movieService = movieService;
        }


        [Route("", Name = "GetDirectors")]
        [HttpGet]
        [CheckPaginationQuery]
        public IHttpActionResult GetActors(int page = 1, int pageSize = 20)
            => Ok(_movieService.GetAllPersons(page, pageSize, "GetDirectors", "director"));



        [Route("{lastName}/Movies", Name = "GetMoviesByDirector")]
        [HttpGet]
        [CheckPaginationQuery]
        public async Task<IHttpActionResult> GetMoviesByDirector(string lastName, int page = 1, int pageSize = 20)
            => Ok(await _movieService.GetMoviesByDirector(lastName, page, pageSize, "GetMoviesByDirector"));
    }
}