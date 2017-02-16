using Movies.Mapper.Factory.Common;
using Movies.Model;
using Movies.Model.Common;
using Movies.WebApi.Utility.RequestMessageProvider;

namespace Movies.Mapper.Factory.Person
{
    public class PersonFactory : FactoryBase, IPersonFactory
    {
        public PersonFactory(IRequestMessageProvider requestMessageProvider) : base(requestMessageProvider)
        {
        }

        public ICommonDto GetModel(string firstName, string lastName, string personType = "actor")
        {
            return new PersonDto
            {
                FirstName = firstName,
                LastName = lastName,
                Link = new Link
                {
                    Rel = "Movies for " + firstName + " " + lastName,
                    Href = personType.ToLower().Equals("actor") ? Url.Link("GetMoviesByActor", new { lastName = lastName }) : 
                    Url.Link("GetMoviesByDirector", new { lastName = lastName }),
                    Method = "GET"
                }
            };
        }
    }
}