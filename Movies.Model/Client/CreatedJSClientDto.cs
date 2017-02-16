using System.Collections.Generic;
using Movies.Model.Common;

namespace Movies.Model.Client
{
    public class CreatedClientDto : CommonDto
    {
        public string Secret { get; set; }
        public List<Link> Links { get; set; }
    }
}