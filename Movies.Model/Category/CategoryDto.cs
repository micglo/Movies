using Movies.Model.Common;

namespace Movies.Model.Category
{
    public class CategoryDto : ICommonDto
    {
        public string Name { get; set; }
        public Link Link { get; set; }
    }
}