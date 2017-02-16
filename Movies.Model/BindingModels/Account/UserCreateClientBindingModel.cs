using System.ComponentModel.DataAnnotations;
using Movies.Model.Common;
using Movies.Model.ModelValidation.Client;

namespace Movies.Model.BindingModels.Account
{
    public class UserCreateClientBindingModel : ICommonDto
    {
        [ApplicationTypeValidation(ErrorMessage = "Native confidential or Java Script")]
        [Required(ErrorMessage = "ApplicationType is required")]
        public string ApplicationType { get; set; }

        [DataType(DataType.Url)]
        public string AllowedOrigin { get; set; }
    }
}