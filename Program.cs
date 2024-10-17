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
    string UserName="Admin";
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
    string PhoneNumber="602 123 456";

    if(await userManager.FindByNameAsync(UserName)==null)
    {
        var user=new IdentityUser();
        user.UserName=UserName;
        user.Email=Email;
        user.PasswordHash=Password;
        user.PhoneNumber=PhoneNumber;
        

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
    string PhoneNumber="603 234 567";

    if(await userManager.FindByNameAsync(UserName)==null)
    {
        var user=new IdentityUser();
        user.UserName=UserName;
        user.Email=Email;
        user.PasswordHash=Password;
        user.PhoneNumber=PhoneNumber;
        

        await userManager.CreateAsync(user,Password);

        await userManager.AddToRoleAsync(user,"Engineer");
    }
}
using (var scope=app.Services.CreateAsyncScope())
{
    var userManager=scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    string UserName="mdvorak604";
    string Email="dieselmodapp@gmail.com";
    string Password="Admin-123";
    string PhoneNumber="604 345 678";

    if(await userManager.FindByNameAsync(UserName)==null)
    {
        var user=new IdentityUser();
        user.UserName=UserName;
        user.Email=Email;
        user.PasswordHash=Password;
        user.PhoneNumber=PhoneNumber;
        

        await userManager.CreateAsync(user,Password);

        await userManager.AddToRoleAsync(user,"Engineer");
    }
}
using (var scope=app.Services.CreateAsyncScope())
{
    var userManager=scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    string UserName="tcerny605";
    string Email="dieselmodapp@gmail.com";
    string Password="Admin-123";
    string PhoneNumber="605 456 789";

    if(await userManager.FindByNameAsync(UserName)==null)
    {
        var user=new IdentityUser();
        user.UserName=UserName;
        user.Email=Email;
        user.PasswordHash=Password;
        user.PhoneNumber=PhoneNumber;
        

        await userManager.CreateAsync(user,Password);

        await userManager.AddToRoleAsync(user,"Engineer");
    }
}
using (var scope=app.Services.CreateAsyncScope())
{
    var userManager=scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    string UserName="lhorak606";
    string Email="dieselmodapp@gmail.com";
    string Password="Admin-123";
    string PhoneNumber="606 567 890";

    if(await userManager.FindByNameAsync(UserName)==null)
    {
        var user=new IdentityUser();
        user.UserName=UserName;
        user.Email=Email;
        user.PasswordHash=Password;
        user.PhoneNumber=PhoneNumber;
        

        await userManager.CreateAsync(user,Password);

        await userManager.AddToRoleAsync(user,"Engineer");
    }
}
using (var scope=app.Services.CreateAsyncScope())
{
    var userManager=scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    string UserName="pprochazka607";
    string Email="dieselmodapp@gmail.com";
    string Password="Admin-123";
    string PhoneNumber="607 678 901";

    if(await userManager.FindByNameAsync(UserName)==null)
    {
        var user=new IdentityUser();
        user.UserName=UserName;
        user.Email=Email;
        user.PasswordHash=Password;
        user.PhoneNumber=PhoneNumber;
        

        await userManager.CreateAsync(user,Password);

        await userManager.AddToRoleAsync(user,"Engineer");
    }
}
using (var scope=app.Services.CreateAsyncScope())
{
    var userManager=scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    string UserName="jmaly608";
    string Email="dieselmodapp@gmail.com";
    string Password="Admin-123";
    string PhoneNumber="608 789 012";

    if(await userManager.FindByNameAsync(UserName)==null)
    {
        var user=new IdentityUser();
        user.UserName=UserName;
        user.Email=Email;
        user.PasswordHash=Password;
        user.PhoneNumber=PhoneNumber;
        

        await userManager.CreateAsync(user,Password);

        await userManager.AddToRoleAsync(user,"Engineer");
    }
}
using (var scope=app.Services.CreateAsyncScope())
{
    var userManager=scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    string UserName="mkovar609";
    string Email="dieselmodapp@gmail.com";
    string Password="Admin-123";
    string PhoneNumber="609 890 123";

    if(await userManager.FindByNameAsync(UserName)==null)
    {
        var user=new IdentityUser();
        user.UserName=UserName;
        user.Email=Email;
        user.PasswordHash=Password;
        user.PhoneNumber=PhoneNumber;
        

        await userManager.CreateAsync(user,Password);

        await userManager.AddToRoleAsync(user,"Engineer");
    }
}
using (var scope=app.Services.CreateAsyncScope())
{
    var userManager=scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    string UserName="ovesely601";
    string Email="dieselmodapp@gmail.com";
    string Password="Admin-123";
    string PhoneNumber="601 901 234";

    if(await userManager.FindByNameAsync(UserName)==null)
    {
        var user=new IdentityUser();
        user.UserName=UserName;
        user.Email=Email;
        user.PasswordHash=Password;
        user.PhoneNumber=PhoneNumber;
        

        await userManager.CreateAsync(user,Password);

        await userManager.AddToRoleAsync(user,"Engineer");
    }
}
using (var scope=app.Services.CreateAsyncScope())
{
    var userManager=scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    string UserName="dkral602";
    string Email="dieselmodapp@gmail.com";
    string Password="Admin-123";
    string PhoneNumber="602 012 345";

    if(await userManager.FindByNameAsync(UserName)==null)
    {
        var user=new IdentityUser();
        user.UserName=UserName;
        user.Email=Email;
        user.PasswordHash=Password;
        user.PhoneNumber=PhoneNumber;
        

        await userManager.CreateAsync(user,Password);

        await userManager.AddToRoleAsync(user,"Engineer");
    }
}
using (var scope=app.Services.CreateAsyncScope())
{
    var userManager=scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    string UserName="rbenes603";
    string Email="dieselmodapp@gmail.com";
    string Password="Admin-123";
    string PhoneNumber="603 123 456";

    if(await userManager.FindByNameAsync(UserName)==null)
    {
        var user=new IdentityUser();
        user.UserName=UserName;
        user.Email=Email;
        user.PasswordHash=Password;
        user.PhoneNumber=PhoneNumber;
        

        await userManager.CreateAsync(user,Password);

        await userManager.AddToRoleAsync(user,"Engineer");
    }
}
using (var scope=app.Services.CreateAsyncScope())
{
    var userManager=scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    string UserName="vzelenka604";
    string Email="dieselmodapp@gmail.com";
    string Password="Admin-123";
    string PhoneNumber="604 234 567";

    if(await userManager.FindByNameAsync(UserName)==null)
    {
        var user=new IdentityUser();
        user.UserName=UserName;
        user.Email=Email;
        user.PasswordHash=Password;
        user.PhoneNumber=PhoneNumber;
        

        await userManager.CreateAsync(user,Password);

        await userManager.AddToRoleAsync(user,"Engineer");
    }
}
/*Engineer*/
await app.RunAsync();
