namespace Identity_Role_Management.ViewModels
{
    public class ManageUserRolesViewModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }

        public List<RoleCheckboxItem> Roles { get; set; }
    }
    public class RoleCheckboxItem
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public bool IsSelected { get; set; }
    }
}
