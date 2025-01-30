using FridayFilmClub.Data;
using FridayFilmClub.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Register ApplicationDbContext and Identity
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure Identity services
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Register custom services
builder.Services.AddScoped<AccountService>(); // Use AddScoped instead of AddSingleton for services managing per-request data

// Register Razor Pages
builder.Services.AddRazorPages();

// Register HttpClient for external API calls
builder.Services.AddHttpClient();

// Configure and register Session
builder.Services.AddDistributedMemoryCache(); // For storing session data in memory
builder.Services.AddSession(options =>
{
    options.Cookie.Name = ".FridayFilmClub.Session";
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Session timeout
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Ensure that TempData works (if used in Razor Pages for flash messages, etc.)
builder.Services.AddControllersWithViews().AddSessionStateTempDataProvider();
builder.Services.AddScoped<MovieService>();

var app = builder.Build();

// Configure middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();



// Enable Session Middleware
app.UseSession();
app.UseRouting();
// Authentication and Authorization Middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.Run();
