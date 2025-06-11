using Microsoft.EntityFrameworkCore;
using TorneosBasketBall.Servicios;

var builder = WebApplication.CreateBuilder(args);

/******************* Mis Servicios ****************************/

// 1) DbContext  
builder.Services.AddDbContext<ContextoBD>(options =>
  options.UseSqlServer(
    builder.Configuration.GetConnectionString("DefaultConnection")
  )
);
builder.Services.AddControllersWithViews();

// 2) Autenticación y Autorización  
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add services to the container.  
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

app.UseSession(); // Moved after 'app' is declared  

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

app.UseStaticFiles();

app.Run();
