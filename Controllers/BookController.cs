using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BookStore.Models;
using BookStore.Data;
using BookStore.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Controllers;

public class BookController : Controller
{
    private StoreDbContext _context;
    public int PageSize = 6;

    public BookController(StoreDbContext context)
    {
        this._context = context;
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

    public async Task<IActionResult> Detail(int? bookId)
    {
        if (bookId == null)
        {
            return RedirectToAction("Index");
        }

        Book? book = await _context.Books
            .Include(b => b.Category)
            .FirstOrDefaultAsync(b => b.Id == bookId);

        if (book == null)
        {
            return NotFound();
        }

        return View(book);
    }
}


