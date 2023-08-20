using BookStore.Models;
using BookStore.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace BookStore;
[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private UserManager<User> _userManager;
    private RoleManager<IdentityRole> _roleManager;

    public AdminController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public IActionResult Index(string? account)
    {

        if (account == "StoreOwner")
        {
            var storeowner = _userManager.GetUsersInRoleAsync("StoreOwner").Result.ToList();
            ViewBag.Header = "Store Owner";
            return View(storeowner);
        }
        else if (account == "Customer")
        {
            var customers = _userManager.GetUsersInRoleAsync("Customer").Result.ToList();
            ViewBag.Header = "Customer";
            return View(customers);
        }
        var customersAll = _userManager.GetUsersInRoleAsync("Customer").Result.ToList();
        var storeownerAll = _userManager.GetUsersInRoleAsync("StoreOwner").Result.ToList();

        var usersInRoles = customersAll.Concat(storeownerAll).ToList();
        ViewBag.Header = "All Users";

        return View(usersInRoles);

    }

    public IActionResult StoreOwner()
    {
        var storeowner = _userManager.GetUsersInRoleAsync("StoreOwner").Result.ToList();

        return View(storeowner);
    }


    public IActionResult Create()
    {
        return View();
    }



    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateByAdmin model)
    {
        if (ModelState.IsValid)
        {
            var user = new User { UserName = model.Email, Email = model.Email, FullName = model.FullName, HomeAddress = model.HomeAddress };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // You might want to assign admin role here, assuming you have a role named "Admin"
                var adminRoleResult = await _userManager.AddToRoleAsync(user, "Customer");

                if (adminRoleResult.Succeeded)
                {
                    return RedirectToAction("Customer", "Admin"); // Redirect to admin-related page
                }

                foreach (var error in adminRoleResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        return View(model);
    }



    [HttpGet]
    public async Task<IActionResult> Edit(string id)
    {

        User? user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        return View(new EditViewModel
        {
            UserId = id
        }
        );
    }


    [HttpPost]
    public async Task<IActionResult> Edit(EditViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);

            // Xóa vai trò cũ của người dùng
            var userRoles = await _userManager.GetRolesAsync(user);
            if (userRoles.Any())
            {
                await _userManager.RemoveFromRolesAsync(user, userRoles);
            }

            // Thêm vai trò mới cho người dùng
            var role = await _roleManager.FindByNameAsync(model.RoleName);
            if (role != null)
            {
                await _userManager.AddToRoleAsync(user, model.RoleName);
            }
            if (!string.IsNullOrWhiteSpace(model.changePassword))
            {
                // Xử lý đổi mật khẩu nếu cần
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, token, model.changePassword);

                if (!result.Succeeded)
                {
                    // Handle password change failure
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(model);
                }
            }

            // Password changed successfully or no password change requested
            return RedirectToAction("Customer");
        }
        else
        {
            // Handle user not found or other model validation errors
            ModelState.AddModelError("", "Invalid input.");
            return View(model);
        }


    }


    public async Task<IActionResult> Delete(string id)
    {
        User user = await _userManager.FindByIdAsync(id);

        if (user != null)
        {
            await _userManager.DeleteAsync(user);
        }

        return RedirectToAction("Customer");
    }


}

/*IdentityResult result = await _userManager.UpdateAsync(identityUser);

            if (result.Succeeded && !String.IsNullOrEmpty(user.Password))
            {
                await _userManager.RemovePasswordAsync(identityUser);
                result = await _userManager.AddPasswordAsync(identityUser, user.Password);
            }

            if (result.Succeeded)
            {
                return RedirectToAction("Index");
            }

            foreach (IdentityError error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }*/

/*
[HttpPost]
    public async Task<IActionResult> Edit(EditViewModel model)
    {
        if (ModelState.IsValid)
        {
            var identityUser = await _userManager.FindByIdAsync(model.UserId);

            // Xóa vai trò cũ của người dùng
            var userRoles = await _userManager.GetRolesAsync(identityUser);
            if (userRoles.Any())
            {
                await _userManager.RemoveFromRolesAsync(identityUser, userRoles);
            }

            // Thêm vai trò mới cho người dùng
            var role = await _roleManager.FindByNameAsync(model.RoleName);
            if (role != null)
            {
                await _userManager.AddToRoleAsync(identityUser, model.RoleName);
            }

            // Xử lý đổi mật khẩu nếu cần
            IdentityResult result1 = IdentityResult.Success;
            if (!string.IsNullOrEmpty(model.changePassword))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(identityUser);
                result1 = await _userManager.ResetPasswordAsync(identityUser, token, model.changePassword);
            }

            if (result1 == IdentityResult.Success)
            {
                return RedirectToAction("Customer");
            }
        }

        return View(model);
    }*/