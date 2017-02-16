using System;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using Movies.Dal;
using Movies.Domain.Entity;
using Movies.Mapper.Factory.Client;
using Movies.Model.BindingModels.Account;
using Movies.Model.BindingModels.Client;
using Movies.Model.Client;
using Movies.Model.Common;
using Movies.Service.Common;
using Movies.WebApi.Utility.Exception.CustomException;
using Movies.WebApi.Utility.RequestMessageProvider;

namespace Movies.Service.Client
{
    public class ClientService : ServiceBase, IClientService
    {
        private readonly IClientFactory _clientFactory;

        public ClientService(IContext context, IRequestMessageProvider requestMessageProvider, ICustomException customException,
            IClientFactory clientFactory)
            : base(context, requestMessageProvider, customException)
        {
            _clientFactory = clientFactory;
        }

        public override async Task<PagedItems> GetAll(int page, int pageSize, string urlLink)
        {
            var clientsDto = new List<ICommonDto>();
            var skip = pageSize * (page - 1);
            var totalNumberOfClients = await Context.GetClientCollection().CountAsync(new BsonDocument());

            using (
                var cursor =
                    await Context.GetClientCollection()
                        .Find(new BsonDocument())
                        .SortBy(x => x.UserId)
                        .ThenBy(x => x.Id)
                        .Skip(skip)
                        .Limit(pageSize)
                        .ToCursorAsync())
            {
                while (await cursor.MoveNextAsync())
                {
                    clientsDto.AddRange(cursor.Current.Select(client => _clientFactory.GetModel<ClientDto>(client)));
                }
            }
            return CreatePagedItems(clientsDto, urlLink, page, pageSize, totalNumberOfClients);
        }

        public override async Task<ICommonDto> GetById(string id)
        {
            await ClientExists(id);
            var client = await Context.GetClientCollection().Find(x => x.Id.Equals(id)).SingleAsync();
            return _clientFactory.GetModel<ClientDto>(client);
        }


        public async Task<PagedItems> GetUserClients(string userId, int page, int pageSize, string urlLink)
        {
            var userExists = await Context.GetUserCollection().Find(x => x.Id.Equals(userId)).AnyAsync();
            if (!userExists)
                CustomException.ThrowNotFoundException();

            var clientsDto = new List<ICommonDto>();
            var skip = pageSize * (page - 1);
            var totalNumberOfClients = await Context.GetClientCollection().CountAsync(x => x.UserId.Equals(userId));

            using (
                var cursor =
                    await Context.GetClientCollection()
                        .Find(x => x.UserId.Equals(userId))
                        .SortBy(x => x.Id)
                        .Skip(skip)
                        .Limit(pageSize)
                        .ToCursorAsync())
            {
                while (await cursor.MoveNextAsync())
                {
                    clientsDto.AddRange(cursor.Current.Select(client => _clientFactory.GetModel<ClientDto>(client)));
                }
            }
            return CreatePagedItems(clientsDto, urlLink, page, pageSize, totalNumberOfClients);
        }

        public async Task<PagedItems> GetMyClients(string userId, int page, int pageSize, string urlLink)
        {
            var clientsDto = new List<ICommonDto>();
            var skip = pageSize * (page - 1);
            var totalNumberOfClients = await Context.GetClientCollection().CountAsync(x => x.UserId.Equals(userId));

            using (
                var cursor =
                    await Context.GetClientCollection()
                        .Find(x => x.UserId.Equals(userId))
                        .SortBy(x => x.Id)
                        .Skip(skip)
                        .Limit(pageSize)
                        .ToCursorAsync())
            {
                while (await cursor.MoveNextAsync())
                {
                    clientsDto.AddRange(cursor.Current.Select(client => _clientFactory.GetModel<MyClientDto>(client)));
                }
            }
            return CreatePagedItems(clientsDto, urlLink, page, pageSize, totalNumberOfClients);
        }


        public async Task<ICommonDto> GetMyClient(string id, string userId)
        {
            await ClientExists(id);
            await ClientBelongsToUser(id, userId);

            var client = await Context.GetClientCollection().Find(x => x.Id.Equals(id)).SingleAsync();
            return _clientFactory.GetModel<MyClientDto>(client);
        }

        public async Task<ICommonDto> GetUserCreatedClient(string id)
        {
            var client = await Context.GetClientCollection().Find(x => x.Id.Equals(id)).SingleAsync();
            return _clientFactory.GetModel<BaseClientDto>(client);
        }

        public async Task<CommonDto> CreateClient(CreateClientBindingModel clientModel)
        {
            var newClientDomain = (Domain.Entity.Client)_clientFactory.GetModel(clientModel);
            newClientDomain.Id = ObjectId.GenerateNewId().ToString();

            if (clientModel.ApplicationType.ToUpper().Equals("JAVA SCRIPT"))
            {
                if (clientModel.AllowedOrigin.Equals("*"))
                    CustomException.ThrowBadRequestException("Provide AllowedOrigin for cors support");

                var secret = newClientDomain.Secret;
                if (string.IsNullOrEmpty(secret))
                    secret = GenerateClientSecret();

                newClientDomain.Secret = GetHash(secret);
                await Context.GetClientCollection().InsertOneAsync(newClientDomain);
                var newClientDto = (CreatedClientDto)_clientFactory.GetModel<CreatedClientDto>(newClientDomain);
                newClientDto.Secret = secret;
                return newClientDto;
            }

            newClientDomain.Secret = null;
            newClientDomain.AllowedOrigin = "*";
            await Context.GetClientCollection().InsertOneAsync(newClientDomain);
            return (CreatedClientDto)_clientFactory.GetModel<CreatedClientDto>(newClientDomain);
        }

        public async Task<UserCreateClientModel> UserCreateClient(string userId, UserCreateClientBindingModel model)
        {
            var newClient = new Domain.Entity.Client
            {
                Id = ObjectId.GenerateNewId().ToString(),
                Active = true,
                ApplicationType = GetApplicationTypeEnum(model.ApplicationType),
                UserId = userId,
                AllowedOrigin = "*",
                RefreshTokenLifeTime = 10080
            };
            var result = new UserCreateClientModel
            {
                Id = newClient.Id,
                Message = "client_id: " + newClient.Id
            };

            if (model.ApplicationType.ToUpper().Equals("JAVA SCRIPT"))
            {
                if(string.IsNullOrEmpty(model.AllowedOrigin) || model.AllowedOrigin.Equals("*"))
                    CustomException.ThrowBadRequestException("Provide AllowedOrigin for cors support");

                var secret = GenerateClientSecret();
                newClient.Secret = GetHash(secret);
                newClient.AllowedOrigin = model.AllowedOrigin;
                result.Message += Environment.NewLine + "client_secret: " + secret;
            }

            await Context.GetClientCollection().InsertOneAsync(newClient);
            return result;
        }

        public async Task<ICommonDto> UpdateClient(string id, UpdateClientBindingModel clientModel)
        {
            await ClientExists(id);

            if (!string.IsNullOrEmpty(clientModel.ApplicationType) && clientModel.ApplicationType.ToUpper().Equals("JAVA SCRIPT"))
            {
                if (string.IsNullOrEmpty(clientModel.AllowedOrigin) || clientModel.AllowedOrigin.Equals("*"))
                    CustomException.ThrowBadRequestException("Provide AllowedOrigin for cors support");

                if (string.IsNullOrEmpty(clientModel.Secret))
                    clientModel.Secret = GenerateClientSecret();
            }
            else
            {
                clientModel.AllowedOrigin = "*";
                clientModel.Secret = null;
            }

            var updateResult = await UpdateClientAction(id, clientModel);

            if (!updateResult.IsAcknowledged)
                CustomException.ThrowBadRequestException("Client update failed.");

            var updatedClient = await Context.GetClientCollection().Find(x => x.Id.Equals(id)).SingleAsync();

            var updatedClientDto = (UpdatedClientDto)_clientFactory.GetModel<UpdatedClientDto>(updatedClient);
            updatedClientDto.Secret = clientModel.Secret;
            return updatedClientDto;
        }

        public async Task<ICommonDto> ChangeOrigin(string id, string userId, string origin)
        {
            await ClientExists(id);
            await ClientBelongsToUser(id, userId);
            await IsJavaScriptClient(id);

            var update = Builders<Domain.Entity.Client>.Update.Set(x => x.AllowedOrigin, origin);
            var updateResult = await Context.GetClientCollection().UpdateOneAsync(x => x.Id.Equals(id), update);

            if (!updateResult.IsAcknowledged)
                CustomException.ThrowBadRequestException("Change origin failed.");

            return await GetUserCreatedClient(id);
        }

        public async Task<string> GenerateNewClientSecret(string id, string userId)
        {
            await ClientExists(id);
            await ClientBelongsToUser(id, userId);
            await IsJavaScriptClient(id);

            var newSecret = GenerateClientSecret();
            var update = Builders<Domain.Entity.Client>.Update.Set(x => x.Secret, GetHash(newSecret));
            var result = await Context.GetClientCollection().UpdateOneAsync(x => x.Id.Equals(id), update);

            if (!result.IsAcknowledged)
                CustomException.ThrowBadRequestException("Generating new client_id failed.");

            return "client_id: " + id + Environment.NewLine + "client_secret: " + newSecret;
        }

        public async Task Delete(string id)
        {
            await ClientExists(id);

            var deleteResult = await Context.GetClientCollection().DeleteOneAsync(x => x.Id.Equals(id));

            if (!deleteResult.IsAcknowledged)
                CustomException.ThrowBadRequestException("Deleting client failed.");
        }

        public async Task ClientBelongsToUser(string id, string userId)
        {
            var clientBelongsToUser = await Context.GetClientCollection().Find(x => x.Id.Equals(id) && x.UserId.Equals(userId)).AnyAsync();

            if (!clientBelongsToUser)
                CustomException.ThrowBadRequestException("You are not the owner of this client.");
        }

        public string GetHash(string value)
        {
            using (var hashAlgorithm = new SHA256CryptoServiceProvider())
            {
                var byteValue = Encoding.UTF8.GetBytes(value);
                var byteHash = hashAlgorithm.ComputeHash(byteValue);

                return Convert.ToBase64String(byteHash);
            }
        }

        #region RefreshTokenManagment

        public Domain.Entity.Client GetForOath(string id)
            => Context.GetClientCollection().Find(x => x.Id.Equals(id)).Single();

        public async Task<Domain.Entity.Client> GetByRefreshToken(string refreshTokenId)
            => await Context.GetClientCollection().Find(x => x.RefreshTokenId.Equals(refreshTokenId)).SingleAsync();

        public async Task<bool> UpdateRefreshToken(string id, string refreshTokenId, string protectedTicket)
        {
            var updateBuilder = Builders<Domain.Entity.Client>.Update.Set(x=>x.RefreshTokenId, refreshTokenId)
                .Set(x=>x.ProtectedTicket, protectedTicket);

            var updateResult = await Context.GetClientCollection().UpdateOneAsync(x => x.Id.Equals(id), updateBuilder);

            return updateResult.IsAcknowledged;
        }

        public async Task<bool> RemoveRefreshToken(string id)
        {
            var updateBuilder = Builders<Domain.Entity.Client>.Update.Set(x => x.RefreshTokenId, null);

            var updateResult = await Context.GetClientCollection().UpdateOneAsync(x => x.Id.Equals(id), updateBuilder);

            return updateResult.IsAcknowledged;
        }

        #endregion

        #region Helpers

        private async Task<UpdateResult> UpdateClientAction(string id, UpdateClientBindingModel model)
        {
            var updateBuilder = Builders<Domain.Entity.Client>.Update;
            var updateBuilderList = new List<UpdateDefinition<Domain.Entity.Client>>();

            if (!string.IsNullOrEmpty(model.ApplicationType))
                updateBuilderList.Add(updateBuilder.Set(x => x.ApplicationType, GetApplicationTypeEnum(model.ApplicationType)));

            if (model.Active != null)
                updateBuilderList.Add(updateBuilder.Set(x => x.Active, model.Active));

            if (model.RefreshTokenLifeTime != null)
                updateBuilderList.Add(updateBuilder.Set(x => x.RefreshTokenLifeTime, model.RefreshTokenLifeTime));

            if (!string.IsNullOrEmpty(model.UserId))
                updateBuilderList.Add(updateBuilder.Set(x => x.UserId, model.UserId));

            if (!string.IsNullOrEmpty(model.Secret))
                updateBuilderList.Add(updateBuilder.Set(x => x.Secret, GetHash(model.Secret)));

            var update = updateBuilder.Combine(updateBuilderList);

            return await Context.GetClientCollection().UpdateOneAsync(x => x.Id.Equals(id), update);
        }

        private async Task ClientExists(string id)
        {
            var clientExists = await Context.GetClientCollection().Find(x => x.Id.Equals(id)).AnyAsync();

            if(!clientExists)
                CustomException.ThrowNotFoundException();
        }

        private async Task IsJavaScriptClient(string id)
        {
            var isJsClient = await Context.GetClientCollection()
                    .Find(x => x.Id.Equals(id) && x.ApplicationType == ApplicationType.JavaScript)
                    .AnyAsync();

            if(!isJsClient)
                CustomException.ThrowBadRequestException("This is not Java Script client");
        }

        private ApplicationType GetApplicationTypeEnum(string appType)
            => appType.ToUpper().Equals("JAVA SCRIPT") ? ApplicationType.JavaScript : ApplicationType.NativeConfidential;

        private string GenerateClientSecret()
        {
            using (var cryptoRandomDataGenerator = new RNGCryptoServiceProvider())
            {
                var buffer = new byte[26];
                cryptoRandomDataGenerator.GetBytes(buffer);
                return Convert.ToBase64String(buffer);
            }
        }

        #endregion

    }
}