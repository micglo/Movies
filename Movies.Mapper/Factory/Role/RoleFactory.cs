using System.Collections.Generic;
using AspNet.Identity.MongoDB;
using Movies.Mapper.Factory.Common;
using Movies.Model.Common;
using Movies.Model.Role;
using Movies.WebApi.Utility.RequestMessageProvider;

namespace Movies.Mapper.Factory.Role
{
    public class RoleFactory : FactoryBase, IRoleFactory
    {
        public RoleFactory(IRequestMessageProvider requestMessageProvider) : base(requestMessageProvider)
        {
        }

        public ICommonDto GetModel<TDto>(IdentityRole role) where TDto : CommonDto
        {
            if (TypesEqual<TDto, RoleDto>())
            {
                return new RoleDto
                {
                    Id = role.Id,
                    Name = role.Name,
                    Links = new List<Link>
                    {
                        new Link
                        {
                            Rel = "Self",
                            Href = Url.Link("GetRole", new { id = role.Id }),
                            Method = "GET"
                        },
                        new Link
                        {
                            Rel = "Update role",
                            Href = Url.Link("UpdateRole", new { id = role.Id }),
                            Method = "PUT"
                        },
                        new Link
                        {
                            Rel = "Delete role",
                            Href = Url.Link("DeleteRole", new { id = role.Id }),
                            Method = "DELETE"
                        }
                    }
                };
            }
            if (TypesEqual<TDto, CreatedRole>())
            {
                return new CreatedRole
                {
                    Id = role.Id,
                    Links = new List<Link>
                    {
                        new Link
                        {
                            Rel = "Self",
                            Href = Url.Link("GetRole", new { id = role.Id }),
                            Method = "GET"
                        },
                        new Link
                        {
                            Rel = "Update role",
                            Href = Url.Link("UpdateRole", new { id = role.Id }),
                            Method = "PUT"
                        },
                        new Link
                        {
                            Rel = "Delete role",
                            Href = Url.Link("DeleteRole", new { id = role.Id }),
                            Method = "DELETE"
                        }
                    }
                };
            }
            return null;
        }
    }
}