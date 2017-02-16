using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNet.Identity.MongoDB;
using MongoDB.Bson;
using MongoDB.Driver;
using Movies.Dal;
using Movies.Domain.Entity;
using Movies.Mapper.Factory.Role;
using Movies.Mapper.Factory.User;
using Movies.Model.BindingModels.Account;
using Movies.Model.Common;
using Movies.Model.Role;
using Movies.Model.User;
using Movies.Service.Common;
using Movies.Service.Role;
using Movies.WebApi.Utility.Exception.CustomException;
using Movies.WebApi.Utility.RequestMessageProvider;

namespace Movies.Service.User
{
    public class UserService : ServiceBase, IUserService
    {
        private readonly IRoleService _roleService;
        private readonly IUserFactory _userFactory;
        private readonly IRoleFactory _roleFactory;
        public UserService(IContext context, IRequestMessageProvider requestMessageProvider, ICustomException customException, IRoleService roleService,
            IUserFactory userFactory, IRoleFactory roleFactory) 
            : base(context, requestMessageProvider, customException)
        {
            _roleService = roleService;
            _userFactory = userFactory;
            _roleFactory = roleFactory;
        }

        public override async Task<PagedItems> GetAll(int page, int pageSize, string urlLink)
        {
            var usersDto = new List<ICommonDto>();
            var skip = pageSize * (page - 1);
            var totalNumberOfUsers = await Context.GetUserCollection().CountAsync(new BsonDocument());

            using (var cursor = await Context.GetUserCollection().Find(new BsonDocument()).SortBy(x => x.Email).Skip(skip).Limit(pageSize).ToCursorAsync())
            {
                while (await cursor.MoveNextAsync())
                {
                    usersDto.AddRange(cursor.Current.Select(user => _userFactory.GetModel<UserDto>(user)));
                }
            }
            return CreatePagedItems(usersDto, urlLink, page, pageSize, totalNumberOfUsers);
        }

        public override async Task<ICommonDto> GetById(string id)
        {
            await UserExists(id);

            var user = await Context.GetUserCollection().Find(x => x.Id.Equals(id)).SingleAsync();
            return _userFactory.GetModel<UserDto>(user);
        }

        public async Task<ICommonDto> GetMyAccount(string id)
        {
            await UserExists(id);

            var user = await Context.GetUserCollection().Find(x => x.Id.Equals(id)).SingleAsync();

            return _userFactory.GetModel<MyAccountDto>(user);
        }

        public async Task<PagedItems> GetUserRoles(string id, int page, int pageSize, string urlLink)
        {
            await UserExists(id);

            var identityRoleList = new List<IdentityRole>();
            var user = await Context.GetUserCollection().Find(x => x.Id.Equals(id)).SingleAsync();
            var skip = pageSize * (page - 1);

            var userRoles = user.Roles.Skip(skip).Take(pageSize);

            foreach (var roleName in userRoles)
            {
                var role = await _roleService.GetByName(roleName);
                identityRoleList.Add(role);
            }

            var userRolesDto = identityRoleList.Select(_roleFactory.GetModel<RoleDto>);

            return CreatePagedItems(userRolesDto, urlLink, page, pageSize, user.Roles.Count);
        }

        public async Task<ICommonDto> UpdateMyAccount(string id, UpdateMyAccountBindingModel model)
        {
            await UserExists(id);

            var updateResult = await UpdateUser(id, model);

            if (!updateResult.IsAcknowledged)
                CustomException.ThrowBadRequestException("Change origin failed.");

            var user = await Context.GetUserCollection().Find(x => x.Id.Equals(id)).SingleAsync();

            return _userFactory.GetModel<MyAccountDto>(user);
        }

        public async Task<UserRolesDto> AddRolesToUser(string id, List<string> roles)
        {
            await UserExists(id);

            var user = await Context.GetUserCollection().Find(x => x.Id.Equals(id)).SingleAsync();

            var allRoleNames = await _roleService.GetAllNames();
            var rolesNotExists = roles.Except(allRoleNames).ToList();

            if (rolesNotExists.Any())
                CustomException.ThrowBadRequestException($"Roles '{string.Join(",", rolesNotExists)}' does not exixts.");

            var rolesToAdd = roles.Except(user.Roles).ToList();
            user.Roles.AddRange(rolesToAdd);

            var update = Builders<Domain.Entity.User>.Update.Set(x => x.Roles, user.Roles);
            var result = await Context.GetUserCollection().UpdateOneAsync(x => x.Id.Equals(id), update);

            if(!result.IsAcknowledged)
                CustomException.ThrowBadRequestException("Adding roles failed.");

            var roleList = new List<IdentityRole>();

            foreach (var userRole in user.Roles)
            {
                var role = await _roleService.GetByName(userRole);
                roleList.Add(role);
            }

            return _userFactory.GetModel(id, roleList);

        }

        public async Task<UserRolesDto> RemoveRolesFromUser(string id, List<string> roles)
        {
            await UserExists(id);

            var user = await Context.GetUserCollection().Find(x => x.Id.Equals(id)).SingleAsync();

            var allRoleNames = await _roleService.GetAllNames();
            var rolesNotExists = roles.Except(allRoleNames).ToList();

            if (rolesNotExists.Any())
                CustomException.ThrowBadRequestException($"Roles '{string.Join(",", rolesNotExists)}' does not exixts.");

            var newRoles = user.Roles.Except(roles).ToList();

            var update = Builders<Domain.Entity.User>.Update.Set(x => x.Roles, newRoles);
            var result = await Context.GetUserCollection().UpdateOneAsync(x => x.Id.Equals(id), update);

            if (!result.IsAcknowledged)
                CustomException.ThrowBadRequestException("Adding roles failed.");

            var roleList = new List<IdentityRole>();

            foreach (var userRole in newRoles)
            {
                var role = await _roleService.GetByName(userRole);
                roleList.Add(role);
            }

            return _userFactory.GetModel(id, roleList);

        }

        public async Task DeleteMyAccount(string id)
        {
            await UserExists(id);

            var update = Builders<Domain.Entity.User>.Update.Set(x => x.IsActive, false);
            var updateResult = await Context.GetUserCollection().UpdateOneAsync(x => x.Id.Equals(id), update);

            if(!updateResult.IsAcknowledged)
                CustomException.ThrowBadRequestException("Fail.");
        }


        #region Helpers

        private async Task UserExists(string id)
        {
            var userExists = await Context.GetUserCollection().Find(x => x.Id.Equals(id)).AnyAsync();

            if(!userExists)
                CustomException.ThrowNotFoundException();
        }

        private async Task<UpdateResult> UpdateUser(string id, UpdateMyAccountBindingModel model)
        {
            var user = await Context.GetUserCollection().Find(x => x.Id.Equals(id)).SingleAsync();
            var updateBuilder = Builders<Domain.Entity.User>.Update;
            var updateBuilderList = new List<UpdateDefinition<Domain.Entity.User>>();

            if (!string.IsNullOrEmpty(model.FirstName))
                updateBuilderList.Add(updateBuilder.Set(x => x.FirstName, model.FirstName));

            if (!string.IsNullOrEmpty(model.LastName))
                updateBuilderList.Add(updateBuilder.Set(x => x.LastName, model.LastName));

            if (!string.IsNullOrEmpty(model.UserName))
                updateBuilderList.Add(updateBuilder.Set(x => x.UserName, model.UserName));

            if (!string.IsNullOrEmpty(model.Gender))
                updateBuilderList.Add(updateBuilder.Set(x => x.Gender, GetGender(model.Gender)));

            if (model.BirthDate != null)
                updateBuilderList.Add(updateBuilder.Set(x => x.BirthDate, model.BirthDate));

            if (!string.IsNullOrEmpty(model.Email) && !model.Email.Equals(user.Email))
            {
                updateBuilderList.Add(updateBuilder.Set(x => x.Email, model.Email));
                updateBuilderList.Add(updateBuilder.Set(x => x.EmailConfirmed, false));
            }

            var update = updateBuilder.Combine(updateBuilderList);

            return await Context.GetUserCollection().UpdateOneAsync(x => x.Id.Equals(id), update);
        }

        private static Gender GetGender(string gender)
            => gender.ToUpper().Equals("FEMALE") ? Gender.Female : Gender.Male;

        #endregion
    }
}