using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TorneosBasketBall.Servicios;
using Microsoft.Extensions.DependencyInjection; // Ensure this namespace is included  

var builder = WebApplication.CreateBuilder(args);

/******************* Mis Servicios ****************************/

// 1) DbContext  
builder.Services.AddDbContext<ContextoBD>(options =>
   options.UseSqlServer(
     builder.Configuration.GetConnectionString("DefaultConnection")
   )
);
builder.Services.AddControllersWithViews();

// 2) Identity  
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
})
   .AddEntityFrameworkStores<ContextoBD>()
   .AddDefaultTokenProviders();

// 3) Cookie settings redirigir a /Account/Login  
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/Login";
});

// 4) MVC + Razor Pages (para las vistas de IdentityUI)  
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

/**************************************************************/

// Add services to the container.  
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// Middlewares de autenticación/autorization  
app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.  
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.  
    app.UseHsts();
}

// Default route apuntando al Login  
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}"
);

// Mapear Razor Pages (IdentityUI)  
app.MapRazorPages();

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
   name: "default",
   pattern: "{controller=Home}/{action=Index}/{id?}")
   .WithStaticAssets();

app.Run();
