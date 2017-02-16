using System.Collections.Generic;
using Movies.Domain.Common;
using Movies.Mapper.Factory.Common;
using Movies.Model.BindingModels.Movie;
using Movies.Model.Common;
using Movies.Model.Movie;
using Movies.WebApi.Utility.RequestMessageProvider;

namespace Movies.Mapper.Factory.Movie
{
    public class MovieFactory : FactoryBase, IMovieFactory
    {
        public MovieFactory(IRequestMessageProvider requestMessageProvider) : base(requestMessageProvider)
        {
        }

        public ICommonEntity GetModel(ICommonDto dtoModel)
        {
            if (TypesEqual<CreateMovieBindingModel>(dtoModel))
            {
                var movie = (CreateMovieBindingModel)dtoModel;
                return new Domain.Entity.Movie
                {
                    Title = movie.Title,
                    PremiereDate = movie.PremiereDate,
                    Actors = movie.Actors,
                    Directors = movie.Directors,
                    ProductionCompany = movie.ProductionCompany,
                    Categories = movie.Categories
                };
            }
            return null;
        }

        public ICommonDto GetModel<TDto>(ICommonEntity domainEntity) where TDto : CommonDto
        {
            if (TypesEqual<TDto, MovieDto>())
            {
                var movie = (Domain.Entity.Movie)domainEntity;
                return new MovieDto
                {
                    Id = movie.Id,
                    Title = movie.Title,
                    PremiereDate = movie.PremiereDate.ToString("d"),
                    Actors = movie.Actors,
                    Directors = movie.Directors,
                    ProductionCompany = movie.ProductionCompany,
                    Categories = movie.Categories,
                    Links = new List<Link>
                    {
                        new Link
                        {
                            Rel = "Self",
                            Href = Url.Link("GetMovie", new { id = movie.Id }),
                            Method = "GET"
                        },
                        new Link
                        {
                            Rel = "Update movie",
                            Href = Url.Link("UpdateMovie", new { id = movie.Id }),
                            Method = "PUT"
                        },
                        new Link
                        {
                            Rel = "Delete movie",
                            Href = Url.Link("DeleteMovie", new { id = movie.Id }),
                            Method = "DELETE"
                        },new Link
                        {
                            Rel = "Add actors",
                            Href = Url.Link("AddActors", new { id = movie.Id }),
                            Method = "POST"
                        },
                        new Link
                        {
                            Rel = "Add directors",
                            Href = Url.Link("AddDirectors", new { id = movie.Id }),
                            Method = "POST"
                        },
                        new Link
                        {
                            Rel = "Add categories",
                            Href = Url.Link("AddCategories", new { id = movie.Id }),
                            Method = "POST"
                        },new Link
                        {
                            Rel = "Remove actors",
                            Href = Url.Link("RemoveActors", new { id = movie.Id }),
                            Method = "POST"
                        },
                        new Link
                        {
                            Rel = "Remove directors",
                            Href = Url.Link("RemoveDirectors", new { id = movie.Id }),
                            Method = "POST"
                        },
                        new Link
                        {
                            Rel = "Remove categories",
                            Href = Url.Link("RemoveCategories", new { id = movie.Id }),
                            Method = "POST"
                        }
                    }
                };
            }
            if (TypesEqual<TDto, CreatedMovieDto>())
            {
                var movie = (Domain.Entity.Movie)domainEntity;
                return new CreatedMovieDto
                {
                    Id = movie.Id,
                    Links = new List<Link>
                    {
                        new Link
                        {
                            Rel = "Self",
                            Href = Url.Link("GetMovie", new { id = movie.Id }),
                            Method = "GET"
                        }
                    }
                };
            }
            return null;
        }
    }
}