using System;
using System.Collections.Generic;
using System.Linq;
using AspNet.Identity.MongoDB;
using Movies.Domain.Common;
using Movies.Domain.Entity;
using Movies.Mapper.Factory.Common;
using Movies.Model.BindingModels.Account;
using Movies.Model.Common;
using Movies.Model.User;
using Movies.WebApi.Utility.RequestMessageProvider;

namespace Movies.Mapper.Factory.User
{
    public class UserFactory : FactoryBase, IUserFactory
    {
        public UserFactory(IRequestMessageProvider requestMessageProvider) : base(requestMessageProvider)
        {
        }

        public ICommonEntity GetModel(ICommonDto dtoModel)
        {
            if (TypesEqual<AccountRegisterBindingModel>(dtoModel))
            {
                var userDto = (AccountRegisterBindingModel) dtoModel;
                return new Domain.Entity.User
                {
                    Email = userDto.Email,
                    UserName = string.IsNullOrEmpty(userDto.UserName) ? userDto.Email : userDto.UserName,
                    FirstName = userDto.FirstName,
                    LastName = userDto.LastName,
                    Gender = GetGender(userDto.Gender),
                    BirthDate = userDto.BirthDate,
                    JoinDate = DateTime.Now,
                    IsBanned = false,
                    IsActive = true,
                    Roles = new List<string>{ "User" }
                };
            }
            return null;
        }

        public ICommonDto GetModel<TDto>(ICommonEntity domainEntity) where TDto : CommonDto
        {
            if (TypesEqual<TDto, RegisteredUserDto>())
            {
                var user = (Domain.Entity.User)domainEntity;
                return new RegisteredUserDto
                {
                    Id = user.Id,
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
                            Rel = "Resend activation link",
                            Href = Url.Link("ResendActivationLink", null),
                            Method = "POST"
                        },
                        new Link
                        {
                            Rel = "Add new client",
                            Href = Url.Link("UserCreateClient", null),
                            Method = "POST"
                        },
                        new Link
                        {
                            Rel = "Change password",
                            Href = Url.Link("ChangePassword", null),
                            Method = "POST"
                        },
                        new Link
                        {
                            Rel = "Update my account",
                            Href = Url.Link("UpdateMyAccount", null),
                            Method = "PUT"
                        },
                        new Link
                        {
                            Rel = "Delete account",
                            Href = Url.Link("DeleteAccount", null),
                            Method = "GET"
                        }
                    }
                };
            }
            if (TypesEqual<TDto, MyAccountDto>())
            {
                var user = (Domain.Entity.User)domainEntity;
                return new MyAccountDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    UserName = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Gender = GetGenderString(user.Gender),
                    BirthDate = user.BirthDate,
                    JoinDate = user.JoinDate,
                    Roles = user.Roles,
                    Links = new List<Link>
                    {
                        new Link
                        {
                            Rel = "Self",
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
                            Rel = "My clients",
                            Href = Url.Link("MyClients", null),
                            Method = "GET"
                        },
                        new Link
                        {
                            Rel = "Resend activation link",
                            Href = Url.Link("ResendActivationLink", null),
                            Method = "POST"
                        },
                        new Link
                        {
                            Rel = "Change password",
                            Href = Url.Link("ChangePassword", null),
                            Method = "POST"
                        },
                        new Link
                        {
                            Rel = "Update my account",
                            Href = Url.Link("UpdateMyAccount", null),
                            Method = "PUT"
                        },
                        new Link
                        {
                            Rel = "Delete account",
                            Href = Url.Link("DeleteAccount", null),
                            Method = "GET"
                        }
                    }
                };
            }
            if (TypesEqual<TDto, UserDto>())
            {
                var user = (Domain.Entity.User)domainEntity;
                return new UserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    UserName = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Gender = GetGenderString(user.Gender),
                    BirthDate = user.BirthDate,
                    JoinDate = user.JoinDate,
                    AccessFailedCount = user.AccessFailedCount,
                    LockoutEndDateUtc = user.LockoutEndDateUtc,
                    PhoneNumber = user.PhoneNumber,
                    IsActive = user.IsActive,
                    IsBanned = user.IsBanned,
                    EmailConfirmed = user.EmailConfirmed,
                    PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                    TwoFactorEnabled = user.TwoFactorEnabled,
                    LockoutEnabled = user.LockoutEnabled,
                    Links = new List<Link>
                    {
                        new Link
                        {
                            Rel = "Self",
                            Href = Url.Link("GetUser", new { id = user.Id }),
                            Method = "GET"
                        },
                        new Link
                        {
                            Rel = "User roles",
                            Href = Url.Link("GetUserRoles", new { id = user.Id }),
                            Method = "GET"
                        },
                        new Link
                        {
                            Rel = "User clients",
                            Href = Url.Link("GetUserClients", new { id = user.Id }),
                            Method = "GET"
                        },
                        new Link
                        {
                            Rel = "Add roles to user",
                            Href = Url.Link("AddRolesToUser", new { id = user.Id }),
                            Method = "POST"
                        },
                        new Link
                        {
                            Rel = "Remove roles from user",
                            Href = Url.Link("RemoveRolesFromUser", new { id = user.Id }),
                            Method = "POST"
                        }
                    }
                };
            }
            return null;
        }

        public UserRolesDto GetModel(string userId, List<IdentityRole> roles)
        {
            var model = new UserRolesDto
            {
                UserId = userId,
                Roles = roles,
                Links = new List<Link>
                {
                    new Link
                    {
                        Rel = "Get user",
                        Href = Url.Link("GetUser", new {id = userId}),
                        Method = "GET"
                    }
                }
            };
            model.Links.AddRange(AddUserRolesLinks(roles));
            return model;
        }

        #region Helpers

        private static Gender GetGender(string gender)
            => gender.ToUpper().Equals("FEMALE") ? Gender.Female : Gender.Male;

        private static string GetGenderString(Gender gender)
            => gender == Gender.Female ? "female" : "male";

        private IEnumerable<Link> AddUserRolesLinks(IEnumerable<IdentityRole> roles)
        {
            return roles.Select(role => new Link
            {
                Rel = "Get role " + role.Name,
                Href = Url.Link("GetRole", new {id = role.Id}),
                Method = "GET"
            }).ToList();
        }

        #endregion
    }
}