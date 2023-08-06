using BookStore.Models;

namespace BookStore.Models.ViewModels;

public class BookListViewModel
{
    public IEnumerable<Book> Books { get; set; } = Enumerable.Empty<Book>();
    public PagingInfo PagingInfo { get; set; } = new();
    public int? CurrentCategoryId { get; set; }

    public IEnumerable<Category> Categories { get; set; } = Enumerable.Empty<Category>();

    public string? CurrentSortby { get; set; }
}
