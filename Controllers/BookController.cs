using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BookStore.Models;
using BookStore.Data;
using BookStore.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Controllers;

public class BookController : Controller
{
    private StoreDbContext context;
    public int PageSize = 6;

    public BookController(StoreDbContext context)
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
                TotalItems = categoryId == null ? context.Books.Count() : context.Books.Where(b => b.Category.Id == categoryId).Count(),
            },
            CurrentCategoryId = categoryId,
            Categories = context.Categories.Include(c => c.Books),
            CurrentSortby = sortby
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

}


//  public ViewResult Index(int? categoryId, string? sortby, int bookPage = 1)
//     {
//         IQueryable<Book> booksQuery = context.Books
//         .Include(b => b.Category)
//         .Where(b => categoryId == null || b.Category.Id == categoryId);

//         booksQuery = sortby switch
//         {
//             "Name" => booksQuery.OrderBy(b => b.Title),
//             "Year" => booksQuery.OrderBy(b => b.YearPublished),
//             "Id" => booksQuery.OrderBy(b => b.Id),
//             _ => booksQuery.OrderBy(b => b.Id),
//         };

//         return View(new BookListViewModel
//         {
//             Books = context.Books
//             .Include(b => b.Category)
//             .Where(b => categoryId == null || b.Category.Id == categoryId)
//             .OrderBy(b => b.Id)
//             .Skip((bookPage - 1) * PageSize)
//             .Take(PageSize),
//             PagingInfo = new PagingInfo
//             {
//                 CurrentPage = bookPage,
//                 ItemsPerPage = PageSize,
//                 TotalItems = context.Books.Count()
//             },
//             CurrentCategoryId = categoryId,
//             Categories = context.Categories.Include(c => c.Books)
//         });
//     }