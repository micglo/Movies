using System;
using System.ComponentModel.DataAnnotations;
using Movies.Model.Common;
using Movies.Model.ModelValidation.Account;

namespace Movies.Model.BindingModels.Account
{
    public class UpdateMyAccountBindingModel : ICommonDto
    {
        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress, ErrorMessage = "This is not valid email address")]
        public string Email { get; set; }

        [Display(Name = "FirstName")]
        [DataType(DataType.Text)]
        public string FirstName { get; set; }

        [Display(Name = "LastName")]
        [DataType(DataType.Text)]
        public string LastName { get; set; }

        [Display(Name = "Gender")]
        [DataType(DataType.Text)]
        [GenderValidation(ErrorMessage = "Female or male")]
        public string Gender { get; set; }

        public string UserName { get; set; }
        public DateTime? BirthDate { get; set; }
    }
}