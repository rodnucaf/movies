using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using moviesMVC.Data;
using moviesMVC.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<MovieDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DbConnection")));

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddIdentityCore<Usuario>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 3;
    options.Password.RequireUppercase = false;
})
    .AddEntityFrameworkStores<MovieDbContext>()
    .AddRoles<IdentityRole>()
    .AddSignInManager();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = IdentityConstants.ApplicationScheme;
})
    .AddIdentityCookies();

var app = builder.Build();

//invocar la ejecucion del dbseeder con un using scope
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<MovieDbContext>();
    DbSeeder.Seed(context);
}

builder.Services.ConfigureApplicationCookie(options =>
{
    options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
    options.SlidingExpiration = true;
    options.LoginPath = "/Usuario/Login";
    options.AccessDeniedPath = "/Usuario/AccessDenied";
});


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
