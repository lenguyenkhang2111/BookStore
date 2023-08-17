using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BookStore.Models;
using BookStore.Data;
using BookStore.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Controllers;

public class StoreOwnerController : Controller
{
    private StoreDbContext _context;
    public int PageSize = 6;

    public StoreOwnerController(StoreDbContext context)
    {
        _context = context;
    }
    public ViewResult Index(int? categoryId, string? sortby, int bookPage = 1)
    {
        IQueryable<Book> booksQuery = _context.Books
        .Include(b => b.Category)
        .Where(b => categoryId == null || b.CategoryId == categoryId);
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
            Categories = _context.Categories.Include(c => c.Books),
            CurrentSortby = sortby,
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




