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
               => View(new BookListViewModel
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
                       TotalItems = 15
                   },
                   CurrentCategory = category
               });
}
