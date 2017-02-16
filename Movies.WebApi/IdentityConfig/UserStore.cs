using AspNet.Identity.MongoDB;
using MongoDB.Driver;
using Movies.Domain.Entity;

namespace Movies.WebApi.IdentityConfig
{
    public class UserStore : UserStore<User>
    {
        public UserStore(IMongoCollection<User> users) : base(users)
        {
        }
    }
}