using Movies.Model.Common;

namespace Movies.Model.Client
{
    public class UpdatedClientDto : ClientDto
    {
        public string Secret { get; set; }
    }
}