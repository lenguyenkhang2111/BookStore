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
        .Where(b => categoryId == null || b.Category.Id == categoryId);
        booksQuery = sortby switch
        {
            "Name" => booksQuery.OrderBy(b => b.Title),
            "Year" => booksQuery.OrderBy(b => b.YearPublished),
            "Id" => booksQuery.OrderBy(b => b.Id),
            _ => booksQuery.OrderBy(b => b.Id),
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

    public async Task<IActionResult> Details(int? bookId)
    {
        if (bookId == null)
        {
            return RedirectToAction("Index");
        }

        var book = await context.Books
            .FirstOrDefaultAsync(b => b.Id == bookId);

        if (book == null)
        {
            return NotFound();
        }

        return View(book);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromForm] Book book)
    {

        if (ModelState.IsValid)
        {
            context.Books.Add(book);
            await context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
        return Error(book);
    }

    public async Task<IActionResult> Edit([FromForm] Book book)
    {
        if (ModelState.IsValid)
        {
            context.Books.Update(book);
            await context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        return Error();
    }



    public async Task<IActionResult> Delete(int id)
    {
        Book? book = await context.Books.FindAsync(id);
        if (book != null)
        {
            context.Books.Remove(book);
            await context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        return Error(book);
    }

    private IActionResult Error(Book? book = null)
    {
        foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
        {
            ViewBag.ErrorMessage = error.ErrorMessage;
        }
        return View("Index", book);
    }
}


