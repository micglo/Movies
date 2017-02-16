using Movies.Model.Common;

namespace Movies.Model
{
    public class PersonDto : ICommonDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Link Link { get; set; }
    }
}