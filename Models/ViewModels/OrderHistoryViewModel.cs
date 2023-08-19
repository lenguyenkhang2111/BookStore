using BookStore.Models;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Models.ViewModels;

public class OrderHistoryViewModel
{
    public IEnumerable<Order> Orders { get; set; } = Enumerable.Empty<Order>();

    public decimal Total { get; set; }



}
