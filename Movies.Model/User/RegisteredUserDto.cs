using System.Collections.Generic;
using Movies.Model.Common;

namespace Movies.Model.User
{
    public class RegisteredUserDto : CommonDto
    {
        public List<Link> Links { get; set; }
    }
}