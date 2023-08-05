using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BookStore.Models;
using System.Web; 
namespace BookStore.Controllers;

public class CartController : Controller
{
    public class Cart
    {
        
    }
    public IActionResult Index()
    {
    
        return View();
    }
}

