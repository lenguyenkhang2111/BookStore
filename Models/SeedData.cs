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

            if (!context.Books.Any())
            {
                context.Books.AddRange(
                new Book
                {
                    Title = "Book 1",
                    Description = "This is book 1 description",
                    Price = 19.99m,
                    DatePublished = "2023",
                    Author = "Recommend"

                },
                new Book
                {
                    Title = "Book 2",
                    Description = "This is book 2 description",
                    Price = 29.99m,
                    DatePublished = "2023",
                    Author = "Recommend"
                },
                new Book
                {
                    Title = "Book 3",
                    Description = "This is book 3 description",
                    Price = 39.99m,
                    DatePublished = "2023",
                    Author = "Recommend"
                },
                new Book
                {
                    Title = "Book 4",
                    Description = "This is book 4 description",
                    Price = 49.99m,
                    DatePublished = "2023",
                    Author = "Recommend"
                }
                );
                context.SaveChanges();
            }
        }
    }
}
