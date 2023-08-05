using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BookStore.Models;
using BookStore.Data;
using BookStore.Models.ViewModels;

namespace BookStore.Controllers;

public class BookController : Controller
{
    private StoreDbContext context;
    public int PageSize = 6;

    public BookController(StoreDbContext context)
    {
        this.context = context;
    }
    public ViewResult Index(Category? category, int bookPage = 1)
    {
        return View(new BookListViewModel
        {
            Books = context.Books
            .Where(b => category == null || b.Category == category)
            .OrderBy(b => b.Id)
            .Skip((bookPage - 1) * PageSize)
            .Take(PageSize),
            PagingInfo = new PagingInfo
            {
                CurrentPage = bookPage,
                ItemsPerPage = PageSize,
                TotalItems = 100
            },
            CurrentCategory = category,
            Categories = context.Categories.Select(c => new Category
            {
                Id = c.Id,
                CategoryName = c.CategoryName,
                BookCount = context.Books.Count(book => book.Category != null && book.Category == c)
            })
        });
    }
}
