using Movies.Mapper.Factory.Common;
using Movies.Model.Category;
using Movies.Model.Common;
using Movies.WebApi.Utility.RequestMessageProvider;

namespace Movies.Mapper.Factory.Category
{
    public class CategoryFactory : FactoryBase, ICategoryFactory
    {
        public CategoryFactory(IRequestMessageProvider requestMessageProvider) : base(requestMessageProvider)
        {
        }

        public ICommonDto GetModel(string name)
        {
            return new CategoryDto
            {
                Name = name,
                Link = new Link
                {
                    Rel = "Movies for category " + name,
                    Href = Url.Link("GetMoviesByCategory", new { name = name }),
                    Method = "GET"
                }
            };
        }
    }
}