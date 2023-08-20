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
        builder.Services.AddIdentity<User, IdentityRole>()
        .AddEntityFrameworkStores<StoreDbContext>()
        .AddTokenProvider<DataProtectorTokenProvider<User>>(TokenOptions.DefaultProvider);
        builder.Services.ConfigureApplicationCookie(options =>
        {
            options.LoginPath = "/Account/Login";
            options.AccessDeniedPath = "/Account/AccessDenied";
        });



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