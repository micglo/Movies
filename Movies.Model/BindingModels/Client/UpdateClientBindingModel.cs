using System.ComponentModel.DataAnnotations;
using Movies.Model.ModelValidation.Client;

namespace Movies.Model.BindingModels.Client
{
    public class UpdateClientBindingModel
    {
        public string UserId { get; set; }

        [ApplicationTypeValidation(ErrorMessage = "Native confidential or Java Script")]
        public string ApplicationType { get; set; }

        [DataType(DataType.Url)]
        public string AllowedOrigin { get; set; }

        public string Secret { get; set; }
        public bool? Active { get; set; }
        public int? RefreshTokenLifeTime { get; set; }
    }
}