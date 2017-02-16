using System.ComponentModel.DataAnnotations;

namespace Movies.Model.BindingModels.Account
{
    public class ChangeClientOriginBindingModel
    {
        [Required(ErrorMessage = "AllowedOrigin is required")]
        [DataType(DataType.Url)]
        public string AllowedOrigin { get; set; }
    }
}