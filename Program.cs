using BookStore.Data;
using BookStore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllersWithViews();
        builder.Services.AddDbContext<StoreDbContext>(opts =>
        {
            opts.UseSqlite(builder.Configuration.GetConnectionString("BookStoreSQLiteConnection"));
        });
        builder.Services.AddIdentity<User, IdentityRole>().AddEntityFrameworkStores<StoreDbContext>();
        builder.Services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/Account/Login";
            options.AccessDeniedPath = "/Account/AccessDenied";
        });
        //

        // builder.Services.AddAuthorization(options =>
        //     {
        //         options.AddPolicy("AdminOnly", policy =>
        //         {
        //             policy.RequireAuthenticatedUser();
        //             policy.RequireRole("Admin"); // Make sure this matches the role name if you're using roles
        //             // You can customize the policy further if needed
        //         });
        //     });
        // //


        var app = builder.Build();

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");
        SeedData.EnsurePopulatedAsync(app);
        app.Run();
    }
}