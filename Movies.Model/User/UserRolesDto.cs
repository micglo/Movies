using System.Collections.Generic;
using AspNet.Identity.MongoDB;
using Movies.Model.Common;

namespace Movies.Model.User
{
    public class UserRolesDto
    {
        public string UserId { get; set; }
        public List<IdentityRole> Roles { get; set; }
        public List<Link> Links { get; set; }
    }
}