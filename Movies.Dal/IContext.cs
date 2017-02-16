using AspNet.Identity.MongoDB;
using MongoDB.Driver;
using Movies.Domain.Entity;

namespace Movies.Dal
{
    public interface IContext
    {
        IMongoCollection<Movie> GetMovieCollection();

        IMongoCollection<User> GetUserCollection();

        IMongoCollection<IdentityUserClaim> GetUserClaimCollection();

        IMongoCollection<IdentityRole> GetRoleCollection();

        IMongoCollection<Client> GetClientCollection();
    }
}