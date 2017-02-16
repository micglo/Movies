using System.Collections.Generic;
using Movies.Model.Common;

namespace Movies.Model.Client
{
    public class BaseClientDto : CommonDto
    {
        public List<Link> Links { get; set; }
    }
}