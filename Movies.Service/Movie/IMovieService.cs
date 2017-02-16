using System.Threading.Tasks;
using Movies.Model.BindingModels.Movie;
using Movies.Model.Common;
using Movies.Service.Common;

namespace Movies.Service.Movie
{
    public interface IMovieService : IServiceBase
    {
        #region Movies

        Task<CommonDto> CreateMovie(CreateMovieBindingModel model);
        Task<ICommonDto> UpdateMovie(string id, UpdateMovieBindingModel model);
        Task Delete(string id);

        Task<ICommonDto> AddActors(string id, PersonBindingModel model);
        Task<ICommonDto> RemoveActors(string id, PersonBindingModel model);
        Task<ICommonDto> AddDirectors(string id, PersonBindingModel model);
        Task<ICommonDto> RemoveDirectors(string id, PersonBindingModel model);
        Task<ICommonDto> AddCategories(string id, CategoriesBindingModel model);
        Task<ICommonDto> RemoveCategories(string id, CategoriesBindingModel model);

        #endregion

        #region Categories

        Task<PagedItems> GetAllCategories(int page, int pageSize, string urlLink);
        Task<PagedItems> GetMoviesByCategory(string name, int page, int pageSize, string urlLink);

        #endregion

        #region Persons

        PagedItems GetAllPersons(int page, int pageSize, string urlLink, string person);
        Task<PagedItems> GetMoviesByActor(string lastName, int page, int pageSize, string urlLink);
        Task<PagedItems> GetMoviesByDirector(string lastName, int page, int pageSize, string urlLink);

        #endregion
    }
}