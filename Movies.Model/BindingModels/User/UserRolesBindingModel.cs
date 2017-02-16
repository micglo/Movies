using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Movies.Model.BindingModels.User
{
    public class UserRolesBindingModel
    {
        [Required]
        public List<string> Roles { get; set; }
    }
}