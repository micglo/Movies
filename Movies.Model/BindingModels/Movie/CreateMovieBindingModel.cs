using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Movies.Domain.Entity;
using Movies.Model.Common;

namespace Movies.Model.BindingModels.Movie
{
    public class CreateMovieBindingModel : ICommonDto
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public DateTime PremiereDate { get; set; }

        [Required]
        public List<Person> Actors { get; set; }

        [Required]
        public List<Person> Directors { get; set; }

        [Required]
        public string ProductionCompany { get; set; }

        [Required]
        public List<string> Categories { get; set; }
    }
}