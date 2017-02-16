using System.Threading.Tasks;
using Movies.Model.BindingModels.Account;
using Movies.Model.BindingModels.Client;
using Movies.Model.Client;
using Movies.Model.Common;
using Movies.Service.Common;

namespace Movies.Service.Client
{
    public interface IClientService : IServiceBase
    {
        Task<PagedItems> GetUserClients(string userId, int page, int pageSize, string urlLink);
        Task<PagedItems> GetMyClients(string userId, int page, int pageSize, string urlLink);
        Task<ICommonDto> GetMyClient(string id, string userId);
        Task<ICommonDto> GetUserCreatedClient(string id);


        Task<CommonDto> CreateClient(CreateClientBindingModel clientMode);
        Task<UserCreateClientModel> UserCreateClient(string userId, UserCreateClientBindingModel model);
        Task<ICommonDto> UpdateClient(string id, UpdateClientBindingModel clientModel);
        Task<ICommonDto> ChangeOrigin(string id, string userId, string origin);
        Task<string> GenerateNewClientSecret(string id, string userId);
        Task Delete(string id);

        Task ClientBelongsToUser(string id, string userId);
        string GetHash(string value);


        Domain.Entity.Client GetForOath(string id);
        Task<Domain.Entity.Client> GetByRefreshToken(string refreshTokenId);
        Task<bool> UpdateRefreshToken(string id, string refreshTokenId, string protectedTicket);
        Task<bool> RemoveRefreshToken(string id);
    }
}