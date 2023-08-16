using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BookStore.Models;
using BookStore.Data;
using BookStore.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Controllers;

public class StoreOwnerController : Controller
{
    private StoreDbContext context;
    public int PageSize = 6;

    public StoreOwnerController(StoreDbContext context)
    {
        this.context = context;
    }
    public ViewResult Index(int? categoryId, string? sortby, int bookPage = 1)
    {
        IQueryable<Book> booksQuery = context.Books
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
            Categories = context.Categories.Include(c => c.Books),
            CurrentSortby = sortby,
        });
    }



    [HttpGet]
    public ViewResult Create()
    {
        TempData["ReturnUrl"] = HttpContext.Request.Headers["Referer"].ToString();
        return View("BookEditor", new BookEditorViewModel
        {
            Categories = context.Categories
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromForm] Book book)
    {
        if (ModelState.IsValid)
        {
            context.Books.Add(book);
            await context.SaveChangesAsync();
            return RedirectWithReturnUrl("Goodjob!", "You added a book!", "success");
        }
        return View("BookEditor", ViewModelFactory.Create(book, context.Categories));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int bookId)
    {
        Book? book = await context.Books.FindAsync(bookId);

        if (book != null)
        {
            BookEditorViewModel model = ViewModelFactory.Edit(book, context.Categories);
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
            context.Books.Update(book);
            await context.SaveChangesAsync();
            return RedirectWithReturnUrl("Goodjob!", "You updated a book!", "success");
        }
        return View("BookEditor", ViewModelFactory.Edit(book, context.Categories));
    }



    public async Task<IActionResult> Delete(int bookId)
    {
        Book? book = await context.Books.FindAsync(bookId);
        if (book != null)
        {
            TempData["ReturnUrl"] = HttpContext.Request.Headers["Referer"].ToString();
            context.Books.Remove(book);
            await context.SaveChangesAsync();
            return RedirectWithReturnUrl("Goodjob!", "You deleted a book!", "success");
        }
        return RedirectWithReturnUrl("Failed!", "Cannot delete a book because error!f", "error");
    }


    private IActionResult RedirectWithReturnUrl(string title, string description, string state)
    {
        string? returnUrl = TempData["ReturnUrl"] as string;

        TempData.Remove("ReturnUrl");
        SweetAlertSend(title, description, state);
        if (!string.IsNullOrEmpty(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction("Index");
    }

    private void SweetAlertSend(string title, string description, string state)
    {
        TempData["SweetAlert_Title"] = title;
        TempData["SweetAlert_Description"] = description;
        TempData["SweetAlert_State"] = state;
    }

}




