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
    public ViewResult Index(int? categoryId, int bookPage = 1)
    {
        return View(new BookListViewModel
        {
            Books = context.Books
            .Include(b => b.Category)
            .Where(b => categoryId == null || b.Category.Id == categoryId)
            .OrderBy(b => b.Id)
            .Skip((bookPage - 1) * PageSize)
            .Take(PageSize),
            PagingInfo = new PagingInfo
            {
                CurrentPage = bookPage,
                ItemsPerPage = PageSize,
                TotalItems = context.Books.Count()
            },
            CurrentCategoryId = categoryId,
            Categories = context.Categories.Include(c => c.Books)
        });
    }
}
