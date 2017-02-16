using System;
using System.Threading.Tasks;
using System.Web.Http;
using Movies.Model.BindingModels.Movie;
using Movies.Service.Movie;
using Movies.WebApi.Utility.Filter;
using Movies.WebApi.Utility.VersioningPrefix;

namespace Movies.WebApi.Controllers.v1
{
    [ApiVersion1RoutePrefix("movies")]
    public class MoviesController : BaseApiController
    {
        private readonly IMovieService _movieService;
        public MoviesController(IMovieService movieService)
        {
            _movieService = movieService;
        }


        [Route("", Name = "GetMovies")]
        [HttpGet]
        [CheckPaginationQuery]
        public async Task<IHttpActionResult> GetMovies(int page = 1, int pageSize = 20)
            => Ok(await _movieService.GetAll(page, pageSize, "GetMovies"));


        [Route("{id}", Name = "GetMovie")]
        [HttpGet]
        public async Task<IHttpActionResult> GetMovie(string id)
            => Ok(await _movieService.GetById(id));


        [MyAuthorize(Roles = "Admin")]
        [Route("", Name = "CreateMovie")]
        [HttpPost]
        public async Task<IHttpActionResult> CreateMovie(CreateMovieBindingModel model)
        {
            var newMovie = await _movieService.CreateMovie(model);
            Uri locationHeader = new Uri(Url.Link("GetMovie", new { id = newMovie.Id }));
            return Created(locationHeader, newMovie);
        }


        [MyAuthorize(Roles = "Admin")]
        [Route("{id}", Name = "UpdateMovie")]
        [HttpPut]
        public async Task<IHttpActionResult> UpdateMovie(string id, UpdateMovieBindingModel model)
            => Ok(await _movieService.UpdateMovie(id, model));


        [MyAuthorize(Roles = "Admin")]
        [Route("{id}", Name = "DeleteMovie")]
        [HttpDelete]
        public async Task<IHttpActionResult> DeleteMovie(string id)
        {
            await _movieService.Delete(id);
            return Ok();
        }


        [MyAuthorize(Roles = "Admin")]
        [Route("{id}/AddActors", Name = "AddActors")]
        [HttpPost]
        public async Task<IHttpActionResult> AddActors(string id, PersonBindingModel model)
            => Ok(await _movieService.AddActors(id, model));


        [MyAuthorize(Roles = "Admin")]
        [Route("{id}/RemoveActors", Name = "RemoveActors")]
        [HttpPost]
        public async Task<IHttpActionResult> RemoveActors(string id, PersonBindingModel model)
            => Ok(await _movieService.RemoveActors(id, model));


        [MyAuthorize(Roles = "Admin")]
        [Route("{id}/AddDirectors", Name = "AddDirectors")]
        [HttpPost]
        public async Task<IHttpActionResult> AddDirectors(string id, PersonBindingModel model)
            => Ok(await _movieService.AddDirectors(id, model));


        [MyAuthorize(Roles = "Admin")]
        [Route("{id}/RemoveDirectors", Name = "RemoveDirectors")]
        [HttpPost]
        public async Task<IHttpActionResult> RemoveDirectors(string id, PersonBindingModel model)
            => Ok(await _movieService.RemoveDirectors(id, model));


        [MyAuthorize(Roles = "Admin")]
        [Route("{id}/AddCategories", Name = "AddCategories")]
        [HttpPost]
        public async Task<IHttpActionResult> AddCategories(string id, CategoriesBindingModel model)
            => Ok(await _movieService.AddCategories(id, model));


        [MyAuthorize(Roles = "Admin")]
        [Route("{id}/RemoveCategories", Name = "RemoveCategories")]
        [HttpPost]
        public async Task<IHttpActionResult> RemoveCategories(string id, CategoriesBindingModel model)
            => Ok(await _movieService.RemoveCategories(id, model));
    }
}