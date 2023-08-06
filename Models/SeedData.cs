using BookStore.Data;
using BookStore.Models;
using Microsoft.EntityFrameworkCore;
namespace BookStore.Models
{
    public class SeedData
    {
        public static void EnsurePopulated(IApplicationBuilder app)
        {
            StoreDbContext context = app.ApplicationServices.CreateScope().ServiceProvider.GetRequiredService<StoreDbContext>();
            if (context.Database.GetPendingMigrations().Any())
            {
                context.Database.Migrate();
            }

            Category romance = new() { CategoryName = "Romance" };
            Category education = new() { CategoryName = "Education" };
            if (!context.Categories.Any())
            {
                context.Categories.AddRange(romance, education);
                context.SaveChanges();
            }

            if (!context.Books.Any())
            {
                context.Books.AddRange(
                new Book
                {
                    Title = "Book 1",
                    Description = "This is book 1 description",
                    Price = 19.99m,
                    YearPublished = "2023",
                    Author = "Loyadt Kasuchi",
                    Category = romance
                },
                new Book
                {
                    Title = "Book 2",
                    Description = "This is book 2 description",
                    Price = 29.99m,
                    YearPublished = "2022",
                    Author = "Rutawani Sibachao",
                    Category = education
                }
                );
                context.SaveChanges();
            }
        }
    }
}
