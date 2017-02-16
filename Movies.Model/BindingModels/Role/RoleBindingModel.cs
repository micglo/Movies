using System.ComponentModel.DataAnnotations;

namespace Movies.Model.BindingModels.Role
{
    public class RoleBindingModel
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
    }
}