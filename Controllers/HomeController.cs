using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BookStore.Models;
using BookStore.Data;

namespace BookStore.Controllers;

public class HomeController : Controller
{
    private StoreDbContext context;

    public HomeController(StoreDbContext context)
    {
        this.context = context;
    }

    public IActionResult Index()
    {
        return View();
    }
    public IActionResult Help()
    {

        return View();
    }
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
