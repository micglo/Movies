using System.Collections.Generic;
using System.Threading.Tasks;
using Movies.Model.BindingModels.Account;
using Movies.Model.Common;
using Movies.Model.User;
using Movies.Service.Common;

namespace Movies.Service.User
{
    public interface IUserService : IServiceBase
    {
        Task<ICommonDto> GetMyAccount(string id);
        Task<PagedItems> GetUserRoles(string id, int page, int pageSize, string urlLink);


        Task<ICommonDto> UpdateMyAccount(string id, UpdateMyAccountBindingModel model);
        Task<UserRolesDto> AddRolesToUser(string id, List<string> roles);
        Task<UserRolesDto> RemoveRolesFromUser(string id, List<string> roles);
        Task DeleteMyAccount(string id);
    }
}