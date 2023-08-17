using BookStore.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Models.ViewModels;

public static class ViewModelFactory
{
    public static BookEditorViewModel Create(Book book, IEnumerable<Category> categories)
    {
        return new BookEditorViewModel
        {
            Book = book,
            Categories = categories
        };
    }

    public static BookEditorViewModel Edit(Book book, IEnumerable<Category> categories)
    {
        return new BookEditorViewModel
        {
            Book = book,
            Categories = categories,
            Action = "Edit",
            FormName = "Edit Book"
        };
    }



}
