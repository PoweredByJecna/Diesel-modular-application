using Diesel_modular_application.Data;
using Diesel_modular_application.Models;
using DocumentFormat.OpenXml.Packaging;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileSystemGlobbing.Internal;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Hosting;
using Diesel_modular_application.Services;
using Diesel_modular_application.Controllers;

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

builder.Services.AddHostedService<CleaningDatabase>();
builder.Services.AddScoped<OdstavkyService>();
builder.Services.AddScoped<DieslovaniController>();
builder.Services.AddScoped<TableOdstavky>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.UseMiddleware<RedirectToLoginMiddleware>();


app.MapControllerRoute(


    name: "default",
    pattern: "{controller=Dieslovani}/{action=Index}/{id?}");


/*Engineer*/
await app.RunAsync();
