using System.Collections.Generic;
using Movies.Domain.Entity;
using Movies.Model.Common;

namespace Movies.Model.Movie
{
    public class MovieDto : CommonDto
    {
        public string Title { get; set; }
        public string PremiereDate { get; set; }
        public List<Person> Actors { get; set; }
        public List<Person> Directors { get; set; }
        public string ProductionCompany { get; set; }
        public List<string> Categories { get; set; }
        public List<Link> Links { get; set; }
    }
}