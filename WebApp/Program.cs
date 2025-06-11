using GamePlatformBL.AutoMappers;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WebApp.Services;

var builder = WebApplication.CreateBuilder(args);

// ----------------------------
// HTTP Client + Services
// ----------------------------
builder.Services.AddHttpClient("ApiClient", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"]);
});

builder.Services.AddScoped<ApiService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddAutoMapper(typeof(MappingProfile));

// ----------------------------
// Razor Pages & Controllers
// ----------------------------
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();

// ----------------------------
// Session & Cache
// ----------------------------
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// ----------------------------
// Authentication (Cookie-based)
// ----------------------------
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login";
        options.LogoutPath = "/Login/LoggedOut";
        options.AccessDeniedPath = "/Login/Forbidden";
        options.SlidingExpiration = true;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
    });

var app = builder.Build();

// ----------------------------
// Middleware Pipeline
// ----------------------------
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.MapRazorPages();

app.Run();
