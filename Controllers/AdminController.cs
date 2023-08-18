using BookStore.Models;
using BookStore.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace BookStore;

public class AdminController : Controller
{
    private UserManager<User> _userManager;

    public AdminController(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    public IActionResult Index()
    {
        return View(_userManager.Users.ToList());
    }

}
