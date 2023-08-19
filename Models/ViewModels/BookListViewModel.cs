using BookStore.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Models.ViewModels;

public class BookListViewModel
{
    public IEnumerable<Book> Books { get; set; } = Enumerable.Empty<Book>();
    public PagingInfo PagingInfo { get; set; } = new();

    public IEnumerable<Category> Categories { get; set; } = Enumerable.Empty<Category>();

    public string? CurrentCategoryName { get; set; }

    public int? CurrentCategoryId { get; set; }

    public string? CurrentSortby { get; set; }

    public string? CurrentSearchQuery { get; set; }



}
