using System.Collections.Generic;
using Movies.Model.Common;

namespace Movies.Model.Role
{
    public class RoleDto : CommonDto
    {
        public string Name { get; set; }
        public List<Link> Links { get; set; }
    }
}