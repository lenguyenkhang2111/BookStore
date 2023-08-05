using BookStore.Data;
using BookStore.Models;
using Microsoft.EntityFrameworkCore;
namespace BookStoreStore.Models
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
                }
                );
                context.SaveChanges();
            }
        }
    }
}
