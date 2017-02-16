using System;

namespace Movies.Model.BindingModels.Movie
{
    public class UpdateMovieBindingModel
    {
        public string Title { get; set; }
        public DateTime? PremiereDate { get; set; }
        public string ProductionCompany { get; set; }
    }
}