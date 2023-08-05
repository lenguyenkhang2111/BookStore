﻿using System.ComponentModel.DataAnnotations;
using BookStore.Data;
namespace BookStore.Models;

public class Category
{
    public int Id { get; set; }

    public required string CategoryName { get; set; }

    public int BookCount { get; set; } = 0;

}
