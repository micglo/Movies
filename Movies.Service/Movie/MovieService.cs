using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using Movies.Dal;
using Movies.Domain.Entity;
using Movies.Mapper.Factory.Category;
using Movies.Mapper.Factory.Movie;
using Movies.Mapper.Factory.Person;
using Movies.Model.BindingModels.Movie;
using Movies.Model.Common;
using Movies.Model.Movie;
using Movies.Service.Common;
using Movies.WebApi.Utility.Exception.CustomException;
using Movies.WebApi.Utility.RequestMessageProvider;

namespace Movies.Service.Movie
{
    public class MovieService : ServiceBase, IMovieService
    {
        private readonly IMovieFactory _movieFactory;
        private readonly ICategoryFactory _categoryFactory;
        private readonly IPersonFactory _personFactory;
        public MovieService(IContext context, IRequestMessageProvider requestMessageProvider, ICustomException customException,
            IMovieFactory movieFactory, ICategoryFactory categoryFactory, IPersonFactory personFactory) 
            : base(context, requestMessageProvider, customException)
        {
            _movieFactory = movieFactory;
            _categoryFactory = categoryFactory;
            _personFactory = personFactory;
        }

        #region Movies

        public override async Task<PagedItems> GetAll(int page, int pageSize, string urlLink)
        {
            var moviesDto = new List<ICommonDto>();
            var skip = pageSize * (page - 1);
            var totalNumberOfMovies = await Context.GetMovieCollection().CountAsync(new BsonDocument());

            using (
                var cursor =
                    await Context.GetMovieCollection()
                        .Find(new BsonDocument())
                        .SortBy(x => x.Title)
                        .ThenBy(x => x.Id)
                        .Skip(skip)
                        .Limit(pageSize)
                        .ToCursorAsync())
            {
                while (await cursor.MoveNextAsync())
                {
                    moviesDto.AddRange(cursor.Current.Select(movie => _movieFactory.GetModel<MovieDto>(movie)));
                }
            }
            return CreatePagedItems(moviesDto, urlLink, page, pageSize, totalNumberOfMovies);
        }

        public override async Task<ICommonDto> GetById(string id)
        {
            await MovieExists(id);
            var movie = await Context.GetMovieCollection().Find(x => x.Id.Equals(id)).SingleAsync();
            return _movieFactory.GetModel<MovieDto>(movie);
        }

        public async Task<CommonDto> CreateMovie(CreateMovieBindingModel model)
        {
            var newMovie = (Domain.Entity.Movie)_movieFactory.GetModel(model);
            newMovie.Id = ObjectId.GenerateNewId().ToString();

            await Context.GetMovieCollection().InsertOneAsync(newMovie);

            return (CommonDto)_movieFactory.GetModel<CreatedMovieDto>(newMovie);
        }

        public async Task<ICommonDto> UpdateMovie(string id, UpdateMovieBindingModel model)
        {
            await MovieExists(id);

            var updateResult = await UpdateMovieAction(id, model);

            if (!updateResult.IsAcknowledged)
                CustomException.ThrowBadRequestException("Client update failed.");

            var updatedMovie = await Context.GetMovieCollection().Find(x => x.Id.Equals(id)).SingleAsync();

            return _movieFactory.GetModel<MovieDto>(updatedMovie);
        }

        public async Task Delete(string id)
        {
            await MovieExists(id);

            var deleteResult = await Context.GetMovieCollection().DeleteOneAsync(x => x.Id.Equals(id));

            if (!deleteResult.IsAcknowledged)
                CustomException.ThrowBadRequestException("Deleting movie failed.");
        }

        public async Task<ICommonDto> AddActors(string id, PersonBindingModel model)
        {
            await MovieExists(id);

            var updateBuilder = Builders<Domain.Entity.Movie>.Update.AddToSetEach(x => x.Actors, model.Persons);

            return await ProcessUpdateArrayElements(id, updateBuilder);
        }

        public async Task<ICommonDto> RemoveActors(string id, PersonBindingModel model)
        {
            await MovieExists(id);

            var updateBuilder = Builders<Domain.Entity.Movie>.Update.PullAll(x => x.Actors, model.Persons);
            return await ProcessUpdateArrayElements(id, updateBuilder);
        }

        public async Task<ICommonDto> AddDirectors(string id, PersonBindingModel model)
        {
            await MovieExists(id);

            var updateBuilder = Builders<Domain.Entity.Movie>.Update.AddToSetEach(x => x.Directors, model.Persons);
            return await ProcessUpdateArrayElements(id, updateBuilder);
        }

        public async Task<ICommonDto> RemoveDirectors(string id, PersonBindingModel model)
        {
            await MovieExists(id);

            var updateBuilder = Builders<Domain.Entity.Movie>.Update.PullAll(x => x.Directors, model.Persons);
            return await ProcessUpdateArrayElements(id, updateBuilder);
        }

        public async Task<ICommonDto> AddCategories(string id, CategoriesBindingModel model)
        {
            await MovieExists(id);

            var updateBuilder = Builders<Domain.Entity.Movie>.Update.AddToSetEach(x => x.Categories, model.Categories);
            return await ProcessUpdateArrayElements(id, updateBuilder);
        }

        public async Task<ICommonDto> RemoveCategories(string id, CategoriesBindingModel model)
        {
            await MovieExists(id);

            var updateBuilder = Builders<Domain.Entity.Movie>.Update.PullAll(x => x.Categories, model.Categories);
            return await ProcessUpdateArrayElements(id, updateBuilder);
        }

        #endregion


        #region Categories

        public async Task<PagedItems> GetAllCategories(int page, int pageSize, string urlLink)
        {
            var categories = new List<string>();
            var skip = pageSize * (page - 1);
            long totalNumberOfCategories = 0;
            using (
                var cursor =
                    await Context.GetMovieCollection().DistinctAsync<string>("Categories", new BsonDocument()))
            {
                while (await cursor.MoveNextAsync())
                {
                    totalNumberOfCategories = cursor.Current.Count();
                    var cursorAsList = cursor.Current.ToList();
                    cursorAsList.Sort();
                    var sliced = cursorAsList.Skip(skip).Take(pageSize);
                    categories.AddRange(sliced);
                }
            }

            var categoriesDto = categories.Select(_categoryFactory.GetModel);
            return CreatePagedItems(categoriesDto, urlLink, page, pageSize, totalNumberOfCategories);
        }

        public async Task<PagedItems> GetMoviesByCategory(string name, int page, int pageSize, string urlLink)
        {
            var moviesDto = new List<ICommonDto>();
            var skip = pageSize * (page - 1);
            var totalNumberOfMovies = await Context.GetMovieCollection().CountAsync(x => x.Categories.Contains(name));

            using (
                var cursor =
                    await Context.GetMovieCollection()
                        .Find(x=>x.Categories.Contains(name))
                        .SortBy(x => x.Title)
                        .ThenBy(x => x.Id)
                        .Skip(skip)
                        .Limit(pageSize)
                        .ToCursorAsync())
            {
                while (await cursor.MoveNextAsync())
                {
                    moviesDto.AddRange(cursor.Current.Select(movie => _movieFactory.GetModel<MovieDto>(movie)));
                }
            }
            return CreatePagedItems(moviesDto, urlLink, page, pageSize, totalNumberOfMovies);
        }

        #endregion


        #region Persons

        public PagedItems GetAllPersons(int page, int pageSize, string urlLink, string person)
        {
            var skip = pageSize * (page - 1);
            var persons = GetPersons(person, skip, pageSize);

            var personsDto = persons.Select(x => _personFactory.GetModel(x.FirstName, x.LastName, person));
            return CreatePagedItems(personsDto, urlLink, page, pageSize, persons.Count);
        }

        public async Task<PagedItems> GetMoviesByActor(string lastName, int page, int pageSize, string urlLink)
        {
            var moviesDto = new List<ICommonDto>();
            var skip = pageSize * (page - 1);
            var totalNumberOfMovies = await Context.GetMovieCollection().CountAsync(x => x.Actors.Any(a => a.LastName.Equals(lastName)));

            using (
                var cursor =
                    await Context.GetMovieCollection()
                        .Find(x => x.Actors.Any(a => a.LastName.Equals(lastName)))
                        .SortBy(x => x.Title)
                        .ThenBy(x => x.Id)
                        .Skip(skip)
                        .Limit(pageSize)
                        .ToCursorAsync())
            {
                while (await cursor.MoveNextAsync())
                {
                    moviesDto.AddRange(cursor.Current.Select(movie => _movieFactory.GetModel<MovieDto>(movie)));
                }
            }
            return CreatePagedItems(moviesDto, urlLink, page, pageSize, totalNumberOfMovies);
        }


        public async Task<PagedItems> GetMoviesByDirector(string lastName, int page, int pageSize, string urlLink)
        {
            var moviesDto = new List<ICommonDto>();
            var skip = pageSize * (page - 1);
            var totalNumberOfMovies = await Context.GetMovieCollection().CountAsync(x => x.Directors.Any(a => a.LastName.Equals(lastName)));

            using (
                var cursor =
                    await Context.GetMovieCollection()
                        .Find(x => x.Directors.Any(a => a.LastName.Equals(lastName)))
                        .SortBy(x => x.Title)
                        .ThenBy(x => x.Id)
                        .Skip(skip)
                        .Limit(pageSize)
                        .ToCursorAsync())
            {
                while (await cursor.MoveNextAsync())
                {
                    moviesDto.AddRange(cursor.Current.Select(movie => _movieFactory.GetModel<MovieDto>(movie)));
                }
            }
            return CreatePagedItems(moviesDto, urlLink, page, pageSize, totalNumberOfMovies);
        }

        #endregion


        #region Helpers

        private async Task MovieExists(string id)
        {
            var movieExists = await Context.GetMovieCollection().Find(x => x.Id.Equals(id)).AnyAsync();

            if (!movieExists)
                CustomException.ThrowNotFoundException();
        }

        private async Task<UpdateResult> UpdateMovieAction(string id, UpdateMovieBindingModel model)
        {
            var updateBuilder = Builders<Domain.Entity.Movie>.Update;
            var updateBuilderList = new List<UpdateDefinition<Domain.Entity.Movie>>();

            if (!string.IsNullOrEmpty(model.Title))
                updateBuilderList.Add(updateBuilder.Set(x => x.Title, model.Title));

            if (model.PremiereDate != null)
                updateBuilderList.Add(updateBuilder.Set(x => x.PremiereDate, model.PremiereDate));

            if (!string.IsNullOrEmpty(model.ProductionCompany))
                updateBuilderList.Add(updateBuilder.Set(x => x.ProductionCompany, model.ProductionCompany));

            var update = updateBuilder.Combine(updateBuilderList);

            return await Context.GetMovieCollection().UpdateOneAsync(x => x.Id.Equals(id), update);
        }

        private async Task<ICommonDto> ProcessUpdateArrayElements(string id, UpdateDefinition<Domain.Entity.Movie> updateBuilder)
        {
            var updateResult = await Context.GetMovieCollection().UpdateOneAsync(x => x.Id.Equals(id), updateBuilder);

            if (!updateResult.IsAcknowledged)
                CustomException.ThrowBadRequestException("Update failed.");

            var updatedMovie = await Context.GetMovieCollection().Find(x => x.Id.Equals(id)).SingleAsync();
            return _movieFactory.GetModel<MovieDto>(updatedMovie);
        }

        private List<Person> GetPersons(string person, int skip, int take)
        {
            var persons = new List<Person>();
            if(person.ToUpper().Equals("ACTOR"))
                return Context.GetMovieCollection().AsQueryable().SelectMany(x => x.Actors)
                    .Distinct()
                    .OrderBy(x => x.LastName)
                    .Skip(skip)
                    .Take(take).ToList();

            return Context.GetMovieCollection().AsQueryable().SelectMany(x => x.Directors)
                .Distinct()
                .OrderBy(x => x.LastName)
                .Skip(skip)
                .Take(take).ToList();
        }
        #endregion
    }
}