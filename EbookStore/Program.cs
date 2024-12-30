using EbookStore.Models;
using EbookStore.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using EbookStore.Data;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Microsoft.AspNetCore.StaticFiles;

var builder = WebApplication.CreateBuilder(args);


// Add services to the container
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddTransient<EmailService>();

// Configure Identity with custom AppUser
builder.Services.AddDefaultIdentity<AppUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequiredUniqueChars = 0;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireLowercase = false;
})
.AddRoles<IdentityRole>() // Include roles
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<PayPalService>(provider =>
{
    var configuration = provider.GetRequiredService<IConfiguration>();
    return new PayPalService(
        configuration["PayPal:AV8bBCDpoCX38F6pEGoVIh5H8aFnjpg7wgqcr75-N3aFBZlW4WUPIdzDoH36kWxICqWa4nGsR2PgGNTV"],
        configuration["PayPal:EDtGhU509jfIRWjq4n2zqgbloZKUi0gm4yDYqRtfLNgqmrpprH_Gv_-_-SvU_5rixZbXNFx15bImtzep"]
    );
});
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.MinimumSameSitePolicy = SameSiteMode.Lax;
});

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();

// Add session services BEFORE `builder.Build()`
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout
    options.Cookie.HttpOnly = true; // Make the cookie HTTP-only
    options.Cookie.IsEssential = true; // Make the cookie essential
});
var configuration = builder.Configuration;

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")) // Use 'builder.Configuration'
           .EnableSensitiveDataLogging()
           .LogTo(Console.WriteLine)); // Log SQL queries to the console


var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseWebSockets();
app.UseStaticFiles(new StaticFileOptions
{
    ServeUnknownFileTypes = true, // Allows unknown file types
    DefaultContentType = "application/octet-stream", // Fallback MIME type
    ContentTypeProvider = new FileExtensionContentTypeProvider
    {
        Mappings =
        {
            [".mobi"] = "application/x-mobipocket-ebook",
            [".epub"] = "application/epub+zip",
            [".fb2"] = "application/xml"
        }
    }
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession(); // Use session middleware AFTER adding the session services

app.UseAuthentication();
app.UseAuthorization();
 

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Customer}/{action=Mainpage}/{id?}");
app.MapRazorPages();

app.MapControllerRoute(
    name: "store",
    pattern: "Store/{action=Gallery}/{id?}",
    defaults: new { controller = "Store" });

app.MapControllerRoute(
    name: "OrderConfirmation",
    pattern: "Cart/OrderConfirmation/{orderId}",
    defaults: new { controller = "Cart", action = "OrderConfirmation" });



app.MapControllerRoute(
    name: "book",
    pattern: "Book/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "book",
    pattern: "Book/{action=Index}/{id?}",
    defaults: new { controller = "Book" });

await SeedDataAsync(app); // Call the seed method

app.Run();

// Seed roles and admin user
async Task SeedDataAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    // Ensure that the database is created and migrations are applied
    await dbContext.Database.EnsureCreatedAsync();

    // Add roles if they don't exist
    var roles = new[] { "Admin", "User" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    // Add an admin user if it doesn't exist
    var adminEmail = "admin@example.com";
    var adminPassword = "Admin@123";
    var firstname = "Admin";
    var lastname = "web";
    if (await userManager.FindByEmailAsync(adminEmail) == null)
    {
        var adminUser = new AppUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true, FirstName = firstname, LastName=lastname};
        await userManager.CreateAsync(adminUser, adminPassword);
        await userManager.AddToRoleAsync(adminUser, "Admin");
    }
   
}
