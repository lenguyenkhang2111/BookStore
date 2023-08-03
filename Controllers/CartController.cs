using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BookStore.Models;

namespace BookStore.Controllers;

public class CartController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}

