using System.Threading.Tasks;
using Movies.Model.Common;

namespace Movies.Service.Common
{
    public interface IServiceBase
    {
        Task<PagedItems> GetAll(int page, int pageSize, string urlLink);
        Task<ICommonDto> GetById(string id);
    }
}