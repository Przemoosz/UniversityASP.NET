using FirstProject.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using FirstProject.Models;
using FirstProject.Utils;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
// builder.Services.AddDbContext<ApplicationDbContext>(options =>
//     options.UseSqlServer(connectionString));
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false).AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();
// var app = builder.Build();

builder.Services.Configure<IdentityOptions>(option =>
{
    // Password
    option.Password.RequireDigit = false;
    option.Password.RequireNonAlphanumeric = false;
    
    // Sing In
    option.SignIn.RequireConfirmedEmail = false;
    option.SignIn.RequireConfirmedAccount = false;
});

AddAuthorizationPolicy(builder.Services);

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

// Set this value to true to initialize admin user and Admin/User Roles
if (false)
{
    
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        Thread.Sleep(3000);
        await AdminDefaultCreate.Initializer(services);
        await RolesDefaultCreate.Initializer(services);

    }
}
app.Run();


void AddAuthorizationPolicy(IServiceCollection service)
{
    // // Adding authorization
    // service.AddAuthorization((option) =>
    // {
    //     // Adding policy with required claim added in login.cshtml.cs
    //     option.AddPolicy("EmployeeOnly", policy => policy.RequireClaim("EmployeeNumber"));
    // });
    service.AddAuthorization((option) =>
    {
        // Adding policy with required claim added in login.cshtml.cs
        option.AddPolicy("RequireUser", policy => policy.RequireRole("User"));
        option.AddPolicy("RequireAdmin", policy => policy.RequireRole("Admin"));
    });

}
