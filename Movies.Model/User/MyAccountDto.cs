using System;
using System.Collections.Generic;
using Movies.Model.Common;

namespace Movies.Model.User
{
    public class MyAccountDto : CommonDto
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime JoinDate { get; set; }
        public string Gender { get; set; }
        public DateTime? BirthDate { get; set; }
        public List<string> Roles { get; set; }
        public ICollection<Link> Links { get; set; }
    }
}