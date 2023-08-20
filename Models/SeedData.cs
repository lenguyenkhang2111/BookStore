using BookStore.Data;
using BookStore.Models;
using BookStore.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
namespace BookStore.Models
{
    public class SeedData
    {
        private static object serviceProvider;

        public static async Task EnsurePopulatedAsync(IApplicationBuilder app)
        {
            StoreDbContext context = app.ApplicationServices.CreateScope().ServiceProvider.GetRequiredService<StoreDbContext>();
            var userManager = app.ApplicationServices.CreateScope().ServiceProvider.GetRequiredService<UserManager<User>>();
            var roleManager = app.ApplicationServices.CreateScope().ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
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
                    Title = "B Book",
                    ShortReview = "This is book 1 description",
                    Price = 19.99m,
                    YearPublished = "2023",
                    Author = "Loyadt Kasuchi",
                    Category = romance
                },
                new Book
                {
                    Title = "A Book",
                    ShortReview = "This is book 1 description",
                    Price = 19.99m,
                    YearPublished = "2023",
                    Author = "Loyadt Kasuchi",
                    Category = romance
                },
                new Book
                {
                    Title = "C Book",
                    ShortReview = "This is book 1 description",
                    Price = 19.99m,
                    YearPublished = "2023",
                    Author = "Loyadt Kasuchi",
                    Category = romance
                },
                new Book
                {
                    Title = "Book 1",
                    ShortReview = "This is book 1 description",
                    Price = 19.99m,
                    YearPublished = "2022",
                    Author = "Loyadt Kasuchi",
                    Category = romance
                },
                new Book
                {
                    Title = "Book 1",
                    ShortReview = "This is book 1 description",
                    Price = 19.99m,
                    YearPublished = "2021",
                    Author = "Loyadt Kasuchi",
                    Category = romance
                },
                new Book
                {
                    Title = "Book 1",
                    ShortReview = "This is book 1 description",
                    Price = 19.99m,
                    YearPublished = "1998",
                    Author = "Loyadt Kasuchi",
                    Category = romance
                },
                new Book
                {
                    Title = "Book 1",
                    ShortReview = "This is book 1 description",
                    Price = 19.99m,
                    YearPublished = "2023",
                    Author = "Loyadt Kasuchi",
                    Category = romance
                },
                new Book
                {
                    Title = "Book 1",
                    ShortReview = "This is book 1 description",
                    Price = 19.99m,
                    YearPublished = "2023",
                    Author = "Loyadt Kasuchi",
                    Category = romance
                },
                new Book
                {
                    Title = "Book 1",
                    ShortReview = "This is book 1 description",
                    Price = 19.99m,
                    YearPublished = "2023",
                    Author = "Loyadt Kasuchi",
                    Category = romance
                },
                new Book
                {
                    Title = "Book 1",
                    ShortReview = "This is book 1 description",
                    Price = 19.99m,
                    YearPublished = "2023",
                    Author = "Loyadt Kasuchi",
                    Category = romance
                },
                new Book
                {
                    Title = "Book 2",
                    ShortReview = "This is book 2 description",
                    Price = 29.99m,
                    YearPublished = "2022",
                    Author = "Rutawani Sibachao",
                    Category = education
                }
                );
                context.SaveChanges();
            }


            // Create Customer role if it doesn't exist

            await roleManager.CreateAsync(new IdentityRole
            {
                Name = "Customer"
            });


            // Create StoreOwner role if it doesn't exist

            await roleManager.CreateAsync(new IdentityRole
            {
                Name = "StoreOwner"
            });

            await roleManager.CreateAsync(new IdentityRole
            {
                Name = "Admin"
            });

            string adminEmail = "admin@gmail.com";
            string adminPassword = "Admin123!";
            var adminUser = new User
            {
                UserName = adminEmail,
                Email = adminEmail,
                FullName = "Admin Full Name",
                HomeAddress = "Admin Address Name"

            };
            var result2 = await userManager.CreateAsync(adminUser, adminPassword);
            var result1 = await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }
}