using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BookStore.Models;
using BookStore.Data;
using BookStore.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Webp;

namespace BookStore.Controllers;

[Authorize(Roles = "StoreOwner")]
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
    public ViewResult CategoryRequest()
    {
        return View();
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CategoryRequest([FromForm] CategoryRequest categoryRequest)
    {
        if (!ModelState.IsValid)
        {
            return View(categoryRequest);
        }
        string Name = categoryRequest.CategoryName;
        if (_context.Categories.Any(c => c.CategoryName == Name) || _context.CategoryRequests.Any(c => c.CategoryName == Name))
        {
            return RedirectWithReturnUrl("Failed!", "You request an existed category!", "error");
        }
        if (User.Identity != null && User.Identity.IsAuthenticated)
        {
            User? user = await _userManager.GetUserAsync(User);
            categoryRequest.StoreOwnerId = user?.Id;
            categoryRequest.Status = "Pending";
        }
        _context.CategoryRequests.Add(categoryRequest);
        await _context.SaveChangesAsync();
        return RedirectWithReturnUrl("Goodjob!", "You requested a new category", "success");

    }

    [HttpGet]
    public ViewResult OrderManage()
    {
        var orders = _context.Orders.Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Book).ToList();


        return View(orders);
    }


    public IActionResult OrderApprove(int orderId, string status)
    {
        var order = _context.Orders.FirstOrDefault(o => o.Id == orderId);

        if (order != null)
        {
            order.Status = status;
            _context.SaveChanges();
        }

        return RedirectWithReturnUrl("Goodjob!", "You processed an order", "success");
    }




    private Task<string?> UploadImageAsync(IFormFile? imageFile)
    {
        if (imageFile != null && imageFile.Length > 0)
        {
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + imageFile.FileName;
            string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\upload", "images", uniqueFileName);

            using (var image = Image.Load(imageFile.OpenReadStream()))
            {
                int targetWidth = 200;
                int targetHeight = 200;
                image.Mutate(c => c.Resize(targetWidth, targetHeight));
                image.Save(imagePath);
                // Return the unique file name
                return Task.FromResult<string?>(uniqueFileName);
            }
        }
        return Task.FromResult<string?>(null);
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

