using AspNetCoreIdentity.Data;
using AspNetCoreIdentity.Entities;
using AspNetCoreIdentity.Infrastructures;
using AspNetCoreIdentity.Infrastructures.EmailServices;
using AspNetCoreIdentity.Settings;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(configuration.GetConnectionString("Default"));
});

// Add to use AspNetCore Identity System here
/*
 * ASP.NET Core Identity is the membership system for building ASP.NET Core web applications, including membership, login,
 * and user data. ASP.NET Core Identity allows you to add login features to your application and makes it easy to customize data
 * about the logged in user.
 */
services.AddIdentity<User, IdentityRole>(options =>
    {
        options.Password.RequiredLength = 8;

        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);

        options.User.RequireUniqueEmail = true;
        options.SignIn.RequireConfirmedEmail = true;
    })
    // Tell the identity system to use the ApplicationDbContext to store data
    .AddEntityFrameworkStores<ApplicationDbContext>()
    // Add the email confirmation token provider to help GenerateEmailConfirmationTokenAsync or 2FA authentication token (userManager.GetAuthenticatorKeyAsync)
    .AddDefaultTokenProviders();

// Help config cookie authentication without adding Cookie authentication handler via .AddCookie
// Because AddAuthentication already add Authentication behind the scene
services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

services.AddAuthentication().AddFacebook(options =>
{
    options.AppId = configuration["FacebookAuth:AppId"];
    options.AppSecret = configuration["FacebookAuth:AppSecret"];
});

// Add services to the container.
services.AddRazorPages();

// AddControllers to use Controllers as well for redirect callback from external authentication
services.AddControllers();

// Register Infrastructures
// services.AddTransient<IEmailService, SystemSmtpEmailService>(); // Just for testing multiple implementation
services.AddTransient<IEmailService, MailKitEmailService>();

// Config Settings 
services.Configure<SmtpSetting>(configuration.GetSection("SMTP"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

// MapControllers to use Controllers as well for redirect callback from external authentication
app.MapControllers();

// Auto run Migrate db
using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetRequiredService<ApplicationDbContext>().Database.Migrate();
}

app.Run();
