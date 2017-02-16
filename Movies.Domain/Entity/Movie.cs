using System;
using System.Collections.Generic;
using Movies.Domain.Common;

namespace Movies.Domain.Entity
{
    public class Movie : CommonEntity
    {
        public string Title { get; set; }
        public DateTime PremiereDate { get; set; }
        public List<Person> Actors { get; set; }
        public List<Person> Directors { get; set; }
        public string ProductionCompany { get; set; }
        public List<string> Categories { get; set; }
    }
}