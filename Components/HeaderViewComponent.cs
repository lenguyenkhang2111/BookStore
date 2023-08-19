using BookStore.Data;
using BookStore.Models;
using BookStore.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Components;

public class HeaderViewComponent : ViewComponent
{
    private StoreDbContext _context;
    private UserManager<User> _userManager;

    public HeaderViewComponent(StoreDbContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;

    }
    public IViewComponentResult Invoke(string viewName = "")
    {
        HeaderViewModel model = new HeaderViewModel
        {
            SearchAspController = "Book"
        };
        if (User.Identity != null && User.Identity.IsAuthenticated)
        {
            var userId = _userManager.GetUserId((System.Security.Claims.ClaimsPrincipal)User);
            int itemCount = _context.Carts
                     .Include(cart => cart.CartItems)
                     .FirstOrDefault(cart => cart.UserId == userId)
                      ?.CartItems?.Count() ?? 0;
            model.SearchAspController = "Book";
            if (User.IsInRole("Admin") || User.IsInRole("StoreOwner"))
            {
                model.SearchAspController = "StoreOwner";
            }
            model.CartCount = itemCount;
        }


        if (!string.IsNullOrEmpty(viewName))
        {
            return View(viewName, model);
        }
        return View(model);
    }

}



