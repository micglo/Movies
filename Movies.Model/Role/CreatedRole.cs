using System.Collections.Generic;
using Movies.Model.Common;

namespace Movies.Model.Role
{
    public class CreatedRole : CommonDto
    {
        public List<Link> Links { get; set; }
    }
}