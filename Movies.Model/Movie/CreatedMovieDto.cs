using System.Collections.Generic;
using Movies.Model.Common;

namespace Movies.Model.Movie
{
    public class CreatedMovieDto : CommonDto
    {
        public List<Link> Links { get; set; }
    }
}