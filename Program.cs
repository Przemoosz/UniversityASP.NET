using FirstProject.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using FirstProject.Models;
using FirstProject.PolicyBasedAuthorization;
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

// Chose between: DefaultPolicy or PolicyLoaded from Json Config File
// Note: Initialize admin and roles before loading any authorization
// Comment/Uncomment line below:

AddAuthorizationPolicyFromJson(builder.Services);
// AddAuthorizationPolicy(builder.Services);

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
// Dictionary<string, List<string>> a = new Dictionary<string, List<string>>(2);
// a.Add("RequireUser", new List<string>(1){"User"});
// a.Add("RequireAdmin", new List<string>(1){"Admin"});
// await ConfigurationFile.SaveAsync(a);

void AddAuthorizationPolicy(IServiceCollection service)
{
    service.AddAuthorization((option) =>
    {
        // Adding policy with required claim added in login.cshtml.cs
        option.AddPolicy("RequireUser", policy => policy.RequireRole("User"));
        option.AddPolicy("RequireAdmin", policy => policy.RequireRole("Admin"));
    });

}
void AddAuthorizationPolicyFromJson(IServiceCollection service)
{
    var policy = ConfigurationFile.Load();
    service.AddAuthorization((option) =>
    {
        if (policy is not null)
        {
            foreach (KeyValuePair<string, List<string>> kvp in policy)
            {
                foreach (string role in kvp.Value)
                {
                    option.AddPolicy(kvp.Key, policy => policy.RequireRole(role));
                }
            }

        }
        // Adding policy with required claim added in login.cshtml.cs
        // option.AddPolicy("RequireUser", policy => policy.RequireRole("User"));
        // option.AddPolicy("RequireAdmin", policy => policy.RequireRole("Admin"));
    });

}
