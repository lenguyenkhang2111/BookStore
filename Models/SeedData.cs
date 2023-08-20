using System.Drawing.Design;
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


            Category Romance = new Category { CategoryName = "Romance" };
            Category Travel = new Category { CategoryName = "Travel" };
            Category Economic = new Category { CategoryName = "Economic" };
            Category ForChildren = new Category { CategoryName = "For Children" };
            Category SelfHelp = new Category { CategoryName = "Self help" };
            Category Cuisine = new Category { CategoryName = "Cuisine" };


            if (!context.Categories.Any())
            {

                context.Categories.AddRange(Romance, Travel, Economic, ForChildren, SelfHelp, Cuisine);
                context.SaveChanges();
            }

            if (!context.Books.Any())
            {
                context.Books.AddRange(
            new Book
            {
                Title = "Eternal Love",
                ShortReview = "A heartwarming tale of love that transcends time and challenges.",
                Price = 14.99m,
                YearPublished = "2020",
                Author = "Emma Rose",
                Category = Romance
            },
            new Book
            {
                Title = "Passion's Embrace",
                ShortReview = "An intense story of forbidden love that ignites against all odds.",
                Price = 12.99m,
                YearPublished = "2018",
                Author = "Alex Turner",
                Category = Romance
            },
            new Book
            {
                Title = "Destined Hearts",
                ShortReview = "Two souls destined to be together, even when the world tries to keep them apart.",
                Price = 15.99m,
                YearPublished = "2021",
                Author = "Sophia Carter",
                Category = Romance
            },
            new Book
            {
                Title = "Love in Bloom",
                ShortReview = "A tale of blossoming romance set against a backdrop of scenic landscapes.",
                Price = 11.99m,
                YearPublished = "2017",
                Author = "Ryan Miller",
                Category = Romance
            },
            new Book
            {
                Title = "Heartstrings",
                ShortReview = "Pulls at your heartstrings with a story that explores the depths of emotions.",
                Price = 13.99m,
                YearPublished = "2019",
                Author = "Olivia Parker",
                Category = Romance
            }, new Book
            {
                Title = "Wealth Beyond Measure",
                ShortReview = "Unlock the secrets of building and managing wealth for a prosperous future.",
                Price = 24.99m,
                YearPublished = "2019",
                Author = "David Roberts",
                Category = Economic
            },
            new Book
            {
                Title = "The Economics of Tomorrow",
                ShortReview = "An insightful analysis of how economic trends today shape the world of tomorrow.",
                Price = 28.99m,
                YearPublished = "2021",
                Author = "Sophie Evans",
                Category = Economic
            },
            new Book
            {
                Title = "Money Matters Made Simple",
                ShortReview = "Demystify complex financial concepts and empower yourself with financial literacy.",
                Price = 22.99m,
                YearPublished = "2020",
                Author = "Michael Johnson",
                Category = Economic
            }, new Book
            {
                Title = "The Magical Adventures of Luna",
                ShortReview = "Join Luna on enchanting adventures in a world where imagination knows no bounds.",
                Price = 9.99m,
                YearPublished = "2021",
                Author = "Emily Davis",
                Category = ForChildren
            },
            new Book
            {
                Title = "Charlie and the Secret Treehouse",
                ShortReview = "Follow Charlie's journey as he uncovers the mysteries of a magical treehouse.",
                Price = 8.99m,
                YearPublished = "2019",
                Author = "Daniel White",
                Category = ForChildren
            },
            new Book
            {
                Title = "Mia's Wonderful Dreams",
                ShortReview = "Discover the power of dreams as Mia embarks on whimsical escapades each night.",
                Price = 10.99m,
                YearPublished = "2020",
                Author = "Sophia Lewis",
                Category = ForChildren
            }, new Book
            {
                Title = "Awaken Your Potential",
                ShortReview = "Empower yourself with actionable advice to unlock your hidden talents and potential.",
                Price = 14.99m,
                YearPublished = "2020",
                Author = "John Matthews",
                Category = SelfHelp
            },
            new Book
            {
                Title = "Finding Inner Harmony",
                ShortReview = "Navigate the path to inner peace and harmony through mindfulness and self-discovery.",
                Price = 12.99m,
                YearPublished = "2018",
                Author = "Sarah Foster",
                Category = SelfHelp
            },
            new Book
            {
                Title = "The Resilience Factor",
                ShortReview = "Build emotional resilience and overcome life's challenges with strength and grace.",
                Price = 15.99m,
                YearPublished = "2021",
                Author = "Michael Roberts",
                Category = SelfHelp
            }, new Book
            {
                Title = "From Farm to Table",
                ShortReview = "Explore the joys of farm-fresh cooking and savor the goodness of seasonal ingredients.",
                Price = 27.99m,
                YearPublished = "2019",
                Author = "James Wilson",
                Category = Cuisine
            },
            new Book
            {
                Title = "Spices & Secrets",
                ShortReview = "Uncover the secrets of spice blends and create dishes that ignite your taste buds.",
                Price = 31.99m,
                YearPublished = "2020",
                Author = "Samantha Lee",
                Category = Cuisine
            },
            new Book
            {
                Title = "Baking Delights",
                ShortReview = "Indulge in the world of baking with a collection of mouthwatering dessert recipes.",
                Price = 25.99m,
                YearPublished = "2018",
                Author = "Oliver Brown",
                Category = Cuisine
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