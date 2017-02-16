using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNet.Identity.MongoDB;
using MongoDB.Bson;
using MongoDB.Driver;
using Movies.Dal;
using Movies.Mapper.Factory.Role;
using Movies.Model.Common;
using Movies.Model.Role;
using Movies.Service.Common;
using Movies.WebApi.Utility.Exception.CustomException;
using Movies.WebApi.Utility.RequestMessageProvider;

namespace Movies.Service.Role
{
    public class RoleService : ServiceBase, IRoleService
    {
        private readonly IRoleFactory _roleFactory;
        public RoleService(IContext context, IRoleFactory roleFactory, IRequestMessageProvider requestMessageProvider, ICustomException customException) 
            : base(context, requestMessageProvider, customException)
        {
            _roleFactory = roleFactory;
        }

        public override async Task<PagedItems> GetAll(int page, int pageSize, string urlLink)
        {
            var rolesDto = new List<ICommonDto>();
            var skip = pageSize * (page - 1);
            var totalNumberOfRoles = await Context.GetRoleCollection().CountAsync(new BsonDocument());

            using (var cursor = await Context.GetRoleCollection().Find(new BsonDocument()).SortBy(x=>x.Name).Skip(skip).Limit(pageSize).ToCursorAsync())
            {
                while (await cursor.MoveNextAsync())
                {
                    rolesDto.AddRange(cursor.Current.Select(user => _roleFactory.GetModel<RoleDto>(user)));
                }
            }

            return CreatePagedItems(rolesDto, urlLink, page, pageSize, totalNumberOfRoles);
        }

        public override async Task<ICommonDto> GetById(string id)
        {
            var roleExists = await Context.GetRoleCollection().Find(x => x.Id.Equals(id)).AnyAsync();

            if (!roleExists)
                CustomException.ThrowNotFoundException();

            var role = await Context.GetRoleCollection().Find(x => x.Id.Equals(id)).SingleAsync();

            return _roleFactory.GetModel<RoleDto>(role);
        }

        public async Task<IEnumerable<string>> GetAllNames()
        {
            var rolesDto = new List<string>();
            using (var cursor = await Context.GetRoleCollection().Find(new BsonDocument()).ToCursorAsync())
            {
                while (await cursor.MoveNextAsync())
                {
                    rolesDto.AddRange(cursor.Current.Select(r => r.Name));
                }
            }
            return rolesDto;
        }

        public async Task<IdentityRole> GetByName(string name)
            => await Context.GetRoleCollection().Find(x => x.Name.Equals(name)).SingleAsync();

        public async Task<ICommonDto> CreateRole(string name)
        {
            var roleExists = await Context.GetRoleCollection().Find(x => x.Name.Equals(name)).AnyAsync();

            if (roleExists)
                CustomException.ThrowBadRequestException("Role already exists.");

            var newRole = new IdentityRole
            {
                Name = name
            };

            await Context.GetRoleCollection().InsertOneAsync(newRole);

            return _roleFactory.GetModel<CreatedRole>(newRole);
        }

        public async Task<ICommonDto> UpdateRole(string id, string name)
        {
            var roleExists = await Context.GetRoleCollection().Find(x => x.Id.Equals(id)).AnyAsync();

            if (!roleExists)
                CustomException.ThrowNotFoundException();

            var roleNameExists = await Context.GetRoleCollection().Find(x => x.Name.Equals(name)).AnyAsync();

            if (roleNameExists)
                CustomException.ThrowBadRequestException("Name is taken.");

            var update = Builders<IdentityRole>.Update.Set(x => x.Name, name);
            var updateResult = await Context.GetRoleCollection().UpdateOneAsync(x => x.Id.Equals(id), update);

            if (!updateResult.IsAcknowledged)
                CustomException.ThrowBadRequestException("Update failed.");

            var updatedRole = await Context.GetRoleCollection().Find(x => x.Id.Equals(id)).SingleAsync();
            return _roleFactory.GetModel<RoleDto>(updatedRole);
        }

        public async Task DeleteRole(string id)
        {
            var roleExists = await Context.GetRoleCollection().Find(x => x.Id.Equals(id)).AnyAsync();

            if (!roleExists)
                CustomException.ThrowNotFoundException();

            var deleteResult = await Context.GetRoleCollection().DeleteOneAsync(x => x.Id.Equals(id));

            if(!deleteResult.IsAcknowledged)
                CustomException.ThrowBadRequestException("Delete failed.");
        }
    }
}