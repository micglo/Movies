using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Movies.Model.BindingModels.Movie
{
    public class CategoriesBindingModel
    {
        [Required]
        public List<string> Categories { get; set; }
    }
}