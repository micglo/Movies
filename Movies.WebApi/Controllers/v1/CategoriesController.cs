using System.Threading.Tasks;
using System.Web.Http;
using Movies.Service.Movie;
using Movies.WebApi.Utility.Filter;
using Movies.WebApi.Utility.VersioningPrefix;

namespace Movies.WebApi.Controllers.v1
{
    [ApiVersion1RoutePrefix("categories")]
    public class CategoriesController : BaseApiController
    {
        private readonly IMovieService _movieService;
        public CategoriesController(IMovieService movieService)
        {
            _movieService = movieService;
        }


        [Route("", Name = "GetCategories")]
        [HttpGet]
        [CheckPaginationQuery]
        public async Task<IHttpActionResult> GetCategories(int page = 1, int pageSize = 20)
            => Ok(await _movieService.GetAllCategories(page, pageSize, "GetCategories"));



        [Route("{name}/Movies", Name = "GetMoviesByCategory")]
        [HttpGet]
        [CheckPaginationQuery]
        public async Task<IHttpActionResult> GetMoviesByCategory(string name, int page = 1, int pageSize = 20)
            => Ok(await _movieService.GetMoviesByCategory(name, page, pageSize, "GetMoviesByCategory"));
    }
}