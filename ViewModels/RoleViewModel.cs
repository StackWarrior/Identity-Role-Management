using System.ComponentModel.DataAnnotations;

namespace Identity_Role_Management.ViewModels
{
    public class RoleViewModel
    {
        public string? Id { get; set; }
        [Required(ErrorMessage = "Role name is required.")]
        [Display(Name = "Role Name")]
        public string Name { get; set; }

    }
}
