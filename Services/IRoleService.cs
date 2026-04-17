using Microsoft.AspNetCore.Identity;

namespace Identity_Role_Management.Services
{
    public interface IRoleService
    {
        Task<IEnumerable<IdentityRole>> GetAllRolesAsync();
        Task<IdentityResult> CreateRoleAsync(string roleName);
        Task<IdentityResult> DeleteRoleAsync(string roleId);
        Task<IEnumerable<IdentityUser>> GetAllUsersAsync();
        Task<IList<string>> GetUserRolesAsync(IdentityUser user);
        Task<IdentityResult> AddUserToRoleAsync(IdentityUser user, string role);
        Task<IdentityResult> RemoveUserFromRoleAsync(IdentityUser user, string role);
        Task RefreshUserSignInAsync(IdentityUser user);

    }
}
