using Identity_Role_Management.Services;
using Identity_Role_Management.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Identity_Role_Management.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly IRoleService _roleService;
        private readonly UserManager<IdentityUser> _userManager;

        public UsersController(IRoleService roleService, UserManager<IdentityUser> userManager)
        {
            _roleService = roleService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _roleService.GetAllUsersAsync();
            var viewModel = new List<UserViewModel>();
            foreach (var user in users)
            {
                var roles = await _roleService.GetUserRolesAsync(user);
                viewModel.Add(new UserViewModel
                {
                    Id = user.Id,
                    UserName = user.UserName ?? string.Empty,
                    Email = user.Email ?? string.Empty,
                    Roles = roles.ToList()
                });
            }
            return View(viewModel);
        }
    }
}
