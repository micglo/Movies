using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Movies.Domain.Entity;

namespace Movies.Model.BindingModels.Movie
{
    public class PersonBindingModel
    {
        [Required]
        public List<Person> Persons { get; set; }
    }
}