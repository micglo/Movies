using System.Collections.Generic;
using Movies.Domain.Common;
using Movies.Domain.Entity;
using Movies.Mapper.Factory.Common;
using Movies.Model.BindingModels.Client;
using Movies.Model.Client;
using Movies.Model.Common;
using Movies.WebApi.Utility.RequestMessageProvider;

namespace Movies.Mapper.Factory.Client
{
    public class ClientFactory : FactoryBase, IClientFactory
    {
        public ClientFactory(IRequestMessageProvider requestMessageProvider) : base(requestMessageProvider)
        {
        }

        public ICommonEntity GetModel(ICommonDto dtoModel)
        {
            if (TypesEqual<CreateClientBindingModel>(dtoModel))
            {
                var client = (CreateClientBindingModel) dtoModel;
                return new Domain.Entity.Client
                {
                    Active = client.Active,
                    AllowedOrigin = client.AllowedOrigin,
                    ApplicationType = GetApplicationTypeEnum(client.ApplicationType),
                    RefreshTokenLifeTime = client.RefreshTokenLifeTime,
                    Secret = string.IsNullOrEmpty(client.Secret) ? null : client.Secret,
                    UserId = client.UserId
                };
            }
            return null;
        }

        public ICommonDto GetModel<TDto>(ICommonEntity domainEntity) where TDto : CommonDto
        {
            if (TypesEqual<TDto, ClientDto>())
            {
                var client = (Domain.Entity.Client) domainEntity;
                return new ClientDto
                {
                    Id = client.Id,
                    UserId = client.UserId,
                    Active = client.Active,
                    AllowedOrigin = client.AllowedOrigin,
                    ApplicationType = GetApplicationTypeString(client.ApplicationType),
                    RefreshTokenLifeTime = client.RefreshTokenLifeTime,
                    Links = new List<Link>
                    {
                        new Link
                        {
                            Rel = "Self",
                            Href = Url.Link("GetClient", new { id = client.Id }),
                            Method = "GET"
                        },
                        new Link
                        {
                            Rel = "Get user",
                            Href = Url.Link("GetUser", new { id = client.UserId }),
                            Method = "GET"
                        },
                        new Link
                        {
                            Rel = "Update client",
                            Href = Url.Link("UpdateClient", new { id = client.Id }),
                            Method = "PUT"
                        }
                    }
                };
            }
            if (TypesEqual<TDto, MyClientDto>())
            {
                var client = (Domain.Entity.Client)domainEntity;
                return new MyClientDto
                {
                    Id = client.Id,
                    UserId = client.UserId,
                    Active = client.Active,
                    AllowedOrigin = client.AllowedOrigin,
                    ApplicationType = GetApplicationTypeString(client.ApplicationType),
                    RefreshTokenLifeTime = client.RefreshTokenLifeTime,
                    Links = new List<Link>
                    {
                        new Link
                        {
                            Rel = "My account",
                            Href = Url.Link("MyAccount", null),
                            Method = "GET"
                        },
                        new Link
                        {
                            Rel = "Add new client",
                            Href = Url.Link("UserCreateClient", null),
                            Method = "POST"
                        },
                        new Link
                        {
                            Rel = "Self",
                            Href = Url.Link("MyClient", new { id = client.Id }),
                            Method = "GET"
                        },
                        new Link
                        {
                            Rel = "Generate new client secret",
                            Href = Url.Link("UserGenerateNewClientSecret", new { id = client.Id }),
                            Method = "GET"
                        },
                        new Link
                        {
                            Rel = "Change client origin",
                            Href = Url.Link("ChangeClientOrigin", new { id = client.Id }),
                            Method = "POST"
                        },
                        new Link
                        {
                            Rel = "Delete client",
                            Href = Url.Link("DeleteMyClient", new { id = client.Id }),
                            Method = "DELETE"
                        }
                    }
                };
            }
            if (TypesEqual<TDto, BaseClientDto>())
            {
                var client = (Domain.Entity.Client)domainEntity;
                return new BaseClientDto
                {
                    Id = client.Id,
                    Links = new List<Link>
                    {
                        new Link
                        {
                            Rel = "Self",
                            Href = Url.Link("MyClient", new { id = client.Id }),
                            Method = "GET"
                        },
                        new Link
                        {
                            Rel = "Add new client",
                            Href = Url.Link("UserCreateClient", null),
                            Method = "POST"
                        },
                        new Link
                        {
                            Rel = "Generate new client secret",
                            Href = Url.Link("UserGenerateNewClientSecret", new { id = client.Id }),
                            Method = "GET"
                        },
                        new Link
                        {
                            Rel = "Change client origin",
                            Href = Url.Link("ChangeClientOrigin", new { id = client.Id }),
                            Method = "POST"
                        },
                        new Link
                        {
                            Rel = "Delete client",
                            Href = Url.Link("DeleteMyClient", new { id = client.Id }),
                            Method = "DELETE"
                        }
                    }
                };
            }
            if (TypesEqual<TDto, CreatedClientDto>())
            {
                var client = (Domain.Entity.Client)domainEntity;
                return new CreatedClientDto
                {
                    Id = client.Id,
                    Links = new List<Link>
                    {
                        new Link
                        {
                            Rel = "Self",
                            Href = Url.Link("GetClient", new { id = client.Id }),
                            Method = "GET"
                        },
                        new Link
                        {
                            Rel = "Update client",
                            Href = Url.Link("UpdateClient", new { id = client.Id }),
                            Method = "PUT"
                        }
                    }
                };
            }
            if (TypesEqual<TDto, UpdatedClientDto>())
            {
                var client = (Domain.Entity.Client)domainEntity;
                return new UpdatedClientDto
                {
                    Id = client.Id,
                    UserId = client.UserId,
                    Active = client.Active,
                    AllowedOrigin = client.AllowedOrigin,
                    ApplicationType = GetApplicationTypeString(client.ApplicationType),
                    RefreshTokenLifeTime = client.RefreshTokenLifeTime,
                    Links = new List<Link>
                    {
                        new Link
                        {
                            Rel = "Self",
                            Href = Url.Link("GetClient", new { id = client.Id }),
                            Method = "GET"
                        },
                        new Link
                        {
                            Rel = "Get user",
                            Href = Url.Link("GetUser", new { id = client.UserId }),
                            Method = "GET"
                        },
                        new Link
                        {
                            Rel = "Update client",
                            Href = Url.Link("UpdateClient", new { id = client.Id }),
                            Method = "PUT"
                        }
                    }
                };
            }
            return null;
        }

        #region Helpers

        private static string GetApplicationTypeString(ApplicationType applicationType)
            => applicationType == ApplicationType.JavaScript ? "Java Script" : "Native confidential";

        private static ApplicationType GetApplicationTypeEnum(string applicationType)
            =>
                applicationType.ToUpper().Equals("NATIVE CONFIDENTIAL")
                    ? ApplicationType.NativeConfidential
                    : ApplicationType.JavaScript;

        #endregion
    }
}