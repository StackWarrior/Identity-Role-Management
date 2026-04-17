using Identity_Role_Management.Services;
using Identity_Role_Management.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Identity_Role_Management.Controllers
{
   [Authorize(Roles = "Admin")]
    public class RolesController : Controller
    {
        private readonly IRoleService _roleService;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;
        public RolesController(IRoleService roleService, RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
        {
            _roleService = roleService;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var roles = await _roleService.GetAllRolesAsync();
            return View(roles);
        }
        public IActionResult Create() => View(new RoleViewModel());

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RoleViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _roleService.CreateRoleAsync(model.Name);
            if (result.Succeeded)
            {
                TempData["success"] = $"Role '{model.Name}' created successfully.";
                return RedirectToAction(nameof(Index));
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return View(model);

        }
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _roleService.DeleteRoleAsync(id);
            if (result.Succeeded)
            {
                TempData["success"] = "Role deleted successfully.";
            }
            else
            {
                TempData["error"] = result.Errors.FirstOrDefault()?.Description ?? "Error deleting role.";
            }
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> ManageUserRoles(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null) return NotFound();

            var allRoles = await _roleService.GetAllRolesAsync();
            var userRoles = await _roleService.GetUserRolesAsync(user);

            var model = new ManageUserRolesViewModel
            {
                UserId = user.Id,
                UserName = user.UserName ?? string.Empty,
                UserEmail = user.Email ?? string.Empty,
                Roles = allRoles.Select(r => new RoleCheckboxItem
                {
                    RoleId = r.Id,
                    RoleName = r.Name ?? string.Empty,
                    IsSelected = userRoles.Contains(r.Name ?? string.Empty)
                }).ToList()
            };

            return View(model);
        }
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageUserRoles(ManageUserRolesViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user is null) return NotFound();

            var currentRoles = await _roleService.GetUserRolesAsync(user);
            var selectedRoles = model.Roles
                .Where(r => r.IsSelected)
                .Select(r => r.RoleName)
                .ToList();

            
            var errors = new List<string>();

            foreach (var role in selectedRoles.Except(currentRoles))
            {
                var result = await _roleService.AddUserToRoleAsync(user, role);
                if (!result.Succeeded)
                    errors.AddRange(result.Errors.Select(e => e.Description));
            }

            foreach (var role in currentRoles.Except(selectedRoles))
            {
                var result = await _roleService.RemoveUserFromRoleAsync(user, role);
                if (!result.Succeeded)
                    errors.AddRange(result.Errors.Select(e => e.Description));
            }

            if (errors.Count > 0)
            {
                TempData["Error"] = string.Join("; ", errors);
                return RedirectToAction(nameof(ManageUserRoles), new { userId = model.UserId });
            }

            
            await _roleService.RefreshUserSignInAsync(user);

            TempData["Success"] = $"Roles updated for {model.UserName}.";
            return RedirectToAction(nameof(Index), "Users");
        }
    }
}
