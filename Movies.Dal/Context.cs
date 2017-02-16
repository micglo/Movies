using System.Configuration;
using AspNet.Identity.MongoDB;
using MongoDB.Driver;
using Movies.Domain.Entity;

namespace Movies.Dal
{
    public class Context : IContext
    {
        private static MongoClient _client;

        public IMongoCollection<Movie> GetMovieCollection()
            => GetMongoDb().GetCollection<Movie>("movies");

        public IMongoCollection<User> GetUserCollection()
            => GetMongoDb().GetCollection<User>("users");

        public IMongoCollection<IdentityUserClaim> GetUserClaimCollection()
            => GetMongoDb().GetCollection<IdentityUserClaim>("userClaims");

        public IMongoCollection<IdentityRole> GetRoleCollection()
            => GetMongoDb().GetCollection<IdentityRole>("roles");

        public IMongoCollection<Client> GetClientCollection()
            => GetMongoDb().GetCollection<Client>("clients");


        private static IMongoDatabase GetMongoDb()
            => GetMongoClient().GetDatabase("moviesDb");

        private static MongoClient GetMongoClient()
            => _client ?? (_client = new MongoClient(ConfigurationManager.AppSettings["MongoConnectionString"]));
    }
}