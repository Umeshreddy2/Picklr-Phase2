using Microsoft.EntityFrameworkCore;
using Picklr.Models;

var builder = WebApplication.CreateBuilder(args);

// Add MVC services
builder.Services.AddControllersWithViews();

// Add EF Core DI
builder.Services.AddDbContext<PicklrContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("PicklrContext")));

// Phase 2: Session-backed shopping cart. AddDistributedMemoryCache() gives
// Session an in-memory store to keep its data in (fine for one server / a
// class project); AddSession() registers ISession itself.
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Apply any pending EF Core migrations at startup. Locally this is a no-op
// because the database is already up to date. On a fresh deployment (Azure)
// there is no picklr.db file at all, so this creates it and seeds it --
// otherwise every page would fail with "no such table: Clubs".
using (var scope = app.Services.CreateScope())
{
    var ctx = scope.ServiceProvider.GetRequiredService<PicklrContext>();
    ctx.Database.Migrate();
}

// Configure HTTP pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// UseSession() must come after UseRouting() and before the routes are
// mapped / MVC runs, so that HttpContext.Session is available inside
// every controller action.
app.UseSession();

app.UseAuthorization();

// Admin area route — must come BEFORE the default route
app.MapAreaControllerRoute(
    name: "admin",
    areaName: "Admin",
    pattern: "Admin/{controller=Home}/{action=Index}/{id?}");

// Default (client) route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
