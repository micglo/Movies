using System.Collections.Generic;
using System.Threading.Tasks;
using AspNet.Identity.MongoDB;
using Movies.Model.Common;
using Movies.Service.Common;

namespace Movies.Service.Role
{
    public interface IRoleService : IServiceBase
    {
        Task<IEnumerable<string>> GetAllNames();
        Task<IdentityRole> GetByName(string name);
        


        Task<ICommonDto> CreateRole(string name);
        Task<ICommonDto> UpdateRole(string id, string name);
        Task DeleteRole(string id);
    }
}