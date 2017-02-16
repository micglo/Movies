using AspNet.Identity.MongoDB;
using Movies.Model.Common;

namespace Movies.Mapper.Factory.Role
{
    public interface IRoleFactory
    {
        ICommonDto GetModel<TDto>(IdentityRole role) where TDto : CommonDto;
    }
}