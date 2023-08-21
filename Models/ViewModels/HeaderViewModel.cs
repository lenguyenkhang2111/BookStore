namespace BookStore.Models.ViewModels;
public class HeaderViewModel
{
    public int CartCount { get; set; } = 0;

    public string SearchAspController { get; set; } = String.Empty;

    public IEnumerable<Category> Categories { get; set; } = Enumerable.Empty<Category>();


}