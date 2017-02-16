using System.ComponentModel.DataAnnotations;
using Movies.Model.Common;
using Movies.Model.ModelValidation.Client;

namespace Movies.Model.BindingModels.Client
{
    public class CreateClientBindingModel : ICommonDto
    {
        [Required(ErrorMessage = "UserId is required")]
        public string UserId { get; set; }

        [ApplicationTypeValidation(ErrorMessage = "Native confidential or Java Script")]
        public string ApplicationType { get; set; } = "Native confidential";

        [DataType(DataType.Url)]
        public string AllowedOrigin { get; set; } = "*";

        public string Secret { get; set; }
        public bool Active { get; set; } = true;
        public int RefreshTokenLifeTime { get; set; } = 10080;
    }
}