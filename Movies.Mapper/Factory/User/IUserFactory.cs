using System.Collections.Generic;
using AspNet.Identity.MongoDB;
using Movies.Mapper.Factory.Common;
using Movies.Model.User;

namespace Movies.Mapper.Factory.User
{
    public interface IUserFactory : IDomainFactory, IModelFactory
    {
        UserRolesDto GetModel(string userId, List<IdentityRole> roles);
    }
}