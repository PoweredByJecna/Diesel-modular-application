using Diesel_modular_application.Data;
using Diesel_modular_application.Models;
using DocumentFormat.OpenXml.Packaging;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileSystemGlobbing.Internal;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<DAdatabase>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();


builder.Services.ConfigureApplicationCookie(Options =>
{
    Options.ExpireTimeSpan = TimeSpan.FromMinutes(30);

});




builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;

})
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<DAdatabase>();
builder.Services.AddControllersWithViews();




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


app.UseMiddleware<RedirectToLoginMiddleware>();
app.UseAuthorization();

app.MapControllerRoute(


    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


using (var scope= app.Services.CreateAsyncScope())
{
    var roleManager=scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var roles = new [] {"Admin","User","Engineer"};

    foreach(var role in roles)
    {
        if(!await roleManager.RoleExistsAsync(role))
        await roleManager.CreateAsync(new IdentityRole(role));
    }
}

using (var scope=app.Services.CreateAsyncScope())
{
    var userManager=scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    string UserName="Administrator";
    string Email="dieselmodapp@gmail.com";
    string Password="Admin-123";

    if(await userManager.FindByNameAsync(UserName)==null)
    {
        var user=new IdentityUser();
        user.UserName=UserName;
        user.Email=Email;
        user.PasswordHash=Password;

        await userManager.CreateAsync(user,Password);

        await userManager.AddToRoleAsync(user,"Admin");
    }
}
using (var scope=app.Services.CreateAsyncScope())
{
    var userManager=scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    string UserName="jnovotny602";
    string Email="dieselmodapp@gmail.com";
    string Password="Admin-123";
   

    if(await userManager.FindByNameAsync(UserName)==null)
    {
        var user=new IdentityUser();
        user.UserName=UserName;
        user.Email=Email;
        user.PasswordHash=Password;

        await userManager.CreateAsync(user,Password);

        await userManager.AddToRoleAsync(user,"Engineer");
    }
}
using (var scope=app.Services.CreateAsyncScope())
{
    var userManager=scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    string UserName="psvoboda603";
    string Email="dieselmodapp@gmail.com";
    string Password="Admin-123";
    string PhoneNumber="";
   

    if(await userManager.FindByNameAsync(UserName)==null)
    {
        var user=new IdentityUser();
        user.UserName=UserName;
        user.Email=Email;
        user.PasswordHash=Password;

        await userManager.CreateAsync(user,Password);

        await userManager.AddToRoleAsync(user,"Engineer");
    }
}
await app.RunAsync();
