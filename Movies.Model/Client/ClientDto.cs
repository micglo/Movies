using System.Collections.Generic;
using Movies.Model.Common;

namespace Movies.Model.Client
{
    public class ClientDto : CommonDto
    {
        public string UserId { get; set; }
        public string ApplicationType { get; set; }
        public bool Active { get; set; }
        public string AllowedOrigin { get; set; }
        public int RefreshTokenLifeTime { get; set; }
        public List<Link> Links { get; set; }
    }
}