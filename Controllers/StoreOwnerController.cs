using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BookStore.Models;
using BookStore.Data;
using BookStore.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace BookStore.Controllers;

public class StoreOwnerController : Controller
{
    private StoreDbContext _context;
    private readonly UserManager<User> _userManager;
    public int PageSize = 6;

    public StoreOwnerController(StoreDbContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
    public ViewResult Index(int? categoryId, string? sortby, string? searchQuery, int bookPage = 1)
    {
        IQueryable<Book> booksQuery = _context.Books
        .Include(b => b.Category)
        .Where(b => categoryId == null || b.CategoryId == categoryId);

        if (!string.IsNullOrEmpty(searchQuery))
        {
            searchQuery = searchQuery.ToLower();

            booksQuery = booksQuery.Where(b => b.Title.ToLower().Contains(searchQuery));
        }

        booksQuery = sortby switch
        {
            "Name" => booksQuery.OrderBy(b => b.Title),
            "Year" => booksQuery.OrderBy(b => b.YearPublished),
            "Id" => booksQuery.OrderBy(b => b.Id),
            _ => booksQuery.OrderBy(b => b.Title),
        };


        return View(new BookListViewModel
        {
            Books = booksQuery
            .Skip((bookPage - 1) * PageSize)
            .Take(PageSize),
            PagingInfo = new PagingInfo
            {
                CurrentPage = bookPage,
                ItemsPerPage = PageSize,
                TotalItems = booksQuery.Count(),
            },
            CurrentCategoryId = categoryId,
            CurrentCategoryName = _context.Categories.Find(categoryId)?.CategoryName,
            Categories = _context.Categories.Include(c => c.Books),
            CurrentSortby = sortby,
            CurrentSearchQuery = searchQuery
        });
    }


    [HttpGet]
    public ViewResult Create()
    {
        TempData["ReturnUrl"] = HttpContext.Request.Headers["Referer"].ToString();
        return View("BookEditor", new BookEditorViewModel
        {
            Categories = _context.Categories
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromForm] Book book)
    {
        if (ModelState.IsValid)
        {
            string? uploadedFileName = await UploadImageAsync(book.ImageFile);

            if (!string.IsNullOrEmpty(uploadedFileName))
            {
                book.ImageUrl = uploadedFileName;
            }

            _context.Books.Add(book);
            await _context.SaveChangesAsync();
            return RedirectWithReturnUrl("Goodjob!", "You added a book!", "success");
        }
        return View("BookEditor", ViewModelFactory.Create(book, _context.Categories));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int bookId)
    {
        Book? book = await _context.Books.FindAsync(bookId);

        if (book != null)
        {
            BookEditorViewModel model = ViewModelFactory.Edit(book, _context.Categories);
            TempData["ReturnUrl"] = HttpContext.Request.Headers["Referer"].ToString();
            return View("BookEditor", model);
        }
        return RedirectWithReturnUrl("Failed", "Cannot edit this book!", "error");
    }



    [HttpPost]
    public async Task<IActionResult> Edit([FromForm] Book book)
    {
        if (ModelState.IsValid)
        {
            string? uploadedFileName = await UploadImageAsync(book.ImageFile);
            if (!string.IsNullOrEmpty(uploadedFileName))
            {
                // Delete existing image
                if (!string.IsNullOrEmpty(book.ImageUrl))
                {
                    string oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\upload", "images", book.ImageUrl);
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }
                book.ImageUrl = uploadedFileName;
            }

            _context.Books.Update(book);
            await _context.SaveChangesAsync();
            return RedirectWithReturnUrl("Goodjob!", "You updated a book!", "success");
        }
        return View("BookEditor", ViewModelFactory.Edit(book, _context.Categories));
    }



    public async Task<IActionResult> Delete(int bookId)
    {
        Book? book = await _context.Books.FindAsync(bookId);
        if (book != null)
        {
            TempData["ReturnUrl"] = HttpContext.Request.Headers["Referer"].ToString();
            _context.Books.Remove(book);
            await _context.SaveChangesAsync();
            return RedirectWithReturnUrl("Goodjob!", "You deleted a book!", "success");
        }
        return RedirectWithReturnUrl("Failed!", "Cannot delete this book because error!", "error");
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> CategoryRequest()
    {
        if (User.Identity != null && User.Identity.IsAuthenticated)
        {
            User? user = await _userManager.FindByNameAsync(User!.Identity!.Name!);
        }
        return View();
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CategoryRequest([FromForm] CategoryRequest categoryRequest)
    {
        string Name = categoryRequest.CategoryName;
        if (_context.Categories.Any(c => c.CategoryName == Name) || _context.CategoryRequests.Any(c => c.CategoryName == Name))
        {
            return RedirectWithReturnUrl("Failed!", "You request an existed category!", "error");
        }
        _context.CategoryRequests.Add(categoryRequest);
        await _context.SaveChangesAsync();
        return RedirectWithReturnUrl("Goodjob!", "You requested a new category", "success");

    }

    [HttpGet]
    public ViewResult OrderManage()
    {
        var orders = _context.Orders.ToList();

        return View(orders);
    }

    [HttpPost]
    public IActionResult OrderManage(int orderId, string status)
    {
        var order = _context.Orders.FirstOrDefault(o => o.Id == orderId);

        if (order != null)
        {
            order.Status = status;
            _context.SaveChanges();
        }

        return RedirectToAction("OrderManage");
    }




    private async Task<string?> UploadImageAsync(IFormFile? imageFile)
    {
        if (imageFile != null && imageFile.Length > 0)
        {
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
            string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\upload", "images", uniqueFileName);
            using (var stream = new FileStream(imagePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }
            return uniqueFileName;
        }
        return null;
    }



    private IActionResult RedirectWithReturnUrl(string title, string description, string state)
    {
        string? returnUrl = TempData["ReturnUrl"] as string;

        TempData.Remove("ReturnUrl");
        TempData["SweetAlert_Title"] = title;
        TempData["SweetAlert_Description"] = description;
        TempData["SweetAlert_State"] = state;
        if (!string.IsNullOrEmpty(returnUrl))
        {
            return Redirect(returnUrl);
        }
        return RedirectToAction("Index");
    }
}




