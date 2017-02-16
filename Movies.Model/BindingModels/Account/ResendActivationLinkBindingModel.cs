using System.ComponentModel.DataAnnotations;

namespace Movies.Model.BindingModels.Account
{
    public class ResendActivationLinkBindingModel
    {
        [Required(ErrorMessage = "Email is required")]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress, ErrorMessage = "This is not valid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}