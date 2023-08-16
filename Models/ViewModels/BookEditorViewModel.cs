namespace BookStore.Models.ViewModels;

public class BookEditorViewModel
{

    public string Action { get; set; } = "Create";

    public string FormName { get; set; } = "Add Book";
    public Book? Book { get; set; }

    public IEnumerable<Category> Categories { get; set; } = Enumerable.Empty<Category>();



}
