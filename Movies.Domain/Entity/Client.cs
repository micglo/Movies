using System;
using Movies.Domain.Common;

namespace Movies.Domain.Entity
{
    public class Client : CommonEntity
    {
        public string Secret { get; set; }
        public string UserId { get; set; }
        public ApplicationType ApplicationType { get; set; }
        public bool Active { get; set; }
        public string AllowedOrigin { get; set; }
        public string RefreshTokenId { get; set; }
        public string ProtectedTicket { get; set; }
        public int RefreshTokenLifeTime { get; set; }
    }
}