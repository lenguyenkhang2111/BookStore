﻿using BookStore.Data;
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
    private readonly StoreDbContext _context;
    private UserManager<User> _userManager;
    private RoleManager<IdentityRole> _roleManager;

    public AdminController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, StoreDbContext context)
    {
        _context = context;
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
                var adminRoleResult = await _userManager.AddToRoleAsync(user, "Customer");

                if (adminRoleResult.Succeeded)
                {
                    return RedirectToAction("Index", "Admin");
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
            var userRoles = await _userManager.GetRolesAsync(user);
            if (userRoles.Any())
            {
                await _userManager.RemoveFromRolesAsync(user, userRoles);
            }
            var role = await _roleManager.FindByNameAsync(model.RoleName);
            if (role != null)
            {
                await _userManager.AddToRoleAsync(user, model.RoleName);
            }
            if (!string.IsNullOrWhiteSpace(model.changePassword))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, token, model.changePassword);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                    return View(model);
                }
            }
            return RedirectToAction("Index", "Admin");
        }
        else
        {
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

        return RedirectToAction("Index", "Admin");
    }

    [HttpGet]
    public ViewResult CategoryRequestManage()
    {
        var orders = _context.CategoryRequests.ToList();
        return View(orders);
    }
    public IActionResult CategoryRequestApprove(int catId, string status)
    {
        var cat = _context.CategoryRequests.FirstOrDefault(o => o.Id == catId);

        if (cat != null)
        {
            cat.Status = status;
            if (cat.Status == "Accept")
            {
                Category newCategory = new Category
                {
                    CategoryName = cat.CategoryName
                };
                _context.Categories.Add(newCategory);
            }
            _context.SaveChanges();
        }

        return RedirectToAction("CategoryRequestManage");
    }

}

