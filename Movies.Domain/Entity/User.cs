using System;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNet.Identity.MongoDB;
using Microsoft.AspNet.Identity;
using Movies.Domain.Common;

namespace Movies.Domain.Entity
{
    public class User : IdentityUser, ICommonEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime JoinDate { get; set; }
        public Gender Gender { get; set; }
        public DateTime? BirthDate { get; set; }
        public bool IsBanned { get; set; }
        public bool IsActive { get; set; }


        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<User> manager, string authenticationType)
        {
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            return userIdentity;
        }
    }
}