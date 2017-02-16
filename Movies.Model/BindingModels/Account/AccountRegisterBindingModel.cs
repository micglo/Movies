using System;
using System.ComponentModel.DataAnnotations;
using Movies.Model.Common;
using Movies.Model.ModelValidation.Account;

namespace Movies.Model.BindingModels.Account
{
    public class AccountRegisterBindingModel : ICommonDto
    {
        [Required(ErrorMessage = "Email is required")]
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress, ErrorMessage = "This is not valid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "FirstName is required")]
        [Display(Name = "FirstName")]
        [DataType(DataType.Text)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "LastName is required")]
        [Display(Name = "LastName")]
        [DataType(DataType.Text)]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        [Display(Name = "Gender")]
        [DataType(DataType.Text)]
        [GenderValidation(ErrorMessage = "Female or male")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required(ErrorMessage = "ConfirmPassword is required")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string UserName { get; set; }
        public DateTime? BirthDate { get; set; }
    }
}