using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using BrickHaven.Models;
using Azure.Identity;
using BrickHaven.Models.ViewModels;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// THIS IS FOR DOTNET IDENTITY
string connectionString = Environment.GetEnvironmentVariable("BrickHavenConnection")
    ?? throw new InvalidOperationException("Connection string not found");

// Add contexts to the system
// THIS IS FOR SQLSERVER
builder.Services.AddDbContext<LoginDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddDbContext<LegoContext>(options => options.UseSqlServer(connectionString));

// When we refer to IWaterRepository, we actually want to use the EFWaterRepository
builder.Services.AddScoped<ILegoRepository, EFLegoRepository>();
builder.Services.AddScoped<ProductRecommendationsViewModel>();

builder.Services.AddRazorPages(); // Allows us to use MVVM

builder.Services.AddDistributedMemoryCache(); // Allows us to use session state
builder.Services.AddSession(); // Allows us to use session state

builder.Services.AddIdentity<Customer, IdentityRole>(
    options =>
    {
        // Password settings
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 9;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;
        options.Password.RequiredUniqueChars = 4;

    })
    .AddEntityFrameworkStores<LoginDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddTransient<ISenderEmail, EmailSender>();

// Configure the Application Cookie settings
builder.Services.ConfigureApplicationCookie(options =>
{
    // If the LoginPath isn't set, ASP.NET Core defaults the path to /Account/Login.
    options.LoginPath = "/Account/Login"; // Set your login path here
    options.AccessDeniedPath = "/Account/InsufficientPrivileges"; // Set the path to the page for insufficient privileges
});

//// Register the UserImporter service
//builder.Services.AddScoped<UserImporter>(); // AddScoped is used here assuming it's appropriate for your scenario

// Configure token lifespan
builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
{
    // Set token lifespan to 2 hours
    options.TokenLifespan = TimeSpan.FromHours(2);
});

string googleClientId = Environment.GetEnvironmentVariable("GoogleClientId");
string googleClientSecret = Environment.GetEnvironmentVariable("GoogleClientSecret");
string microsoftClientId = Environment.GetEnvironmentVariable("MicrosoftClientId");
string microsoftClientSecret = Environment.GetEnvironmentVariable("MicrosoftClientSecret");

builder.Services.AddAuthentication()
    .AddGoogle(options =>
    {
        options.ClientId = googleClientId;
        options.ClientSecret = googleClientSecret;
    })
    .AddMicrosoftAccount(microsoftOptions =>
    {
        microsoftOptions.ClientId = microsoftClientId;
        microsoftOptions.ClientSecret = microsoftClientSecret;
    });

// END DOTNET IDENTITY STUFF

var app = builder.Build();

// Enable the Content-Security-Policy (CSP) HTTP header
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("Content-Security-Policy", "default-src 'self'; " +
        "style-src 'self' fonts.cdnfonts.com fonts.googleapis.com 'unsafe-inline';" +
        "font-src 'self' fonts.cdnfonts.com fonts.googleapis.com fonts.gstatic.com; " +
        "script-src 'self' ajax.googleapis.com 'unsafe-inline';" +
        "img-src 'self' https://bwbricks.azurewebsites.net https://www.youtube.com https://www.lego.com https://m.media-amazon.com https://images.brickset.com https://www.brickeconomy.com;");
    await next.Invoke();
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseStatusCodePagesWithReExecute("/Error/{0}");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseSession();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Looks at the first route, and then the second, etc., but don't show the ones after a route is found
app.MapDefaultControllerRoute();
app.MapControllerRoute("typeColorPageNum", "{legoType}/{legoColor}/{pageNum}", new { Controller = "Home", action = "Shop" });
app.MapControllerRoute("typeColor", "{legoType}/{legoColor}", new { Controller = "Home", action = "Shop" });
app.MapControllerRoute("ColorPageNum", "{legoColor}/{pageNum}", new { Controller = "Home", action = "Shop" });
app.MapControllerRoute("pageenumandtype", "{legoType}/{pageNum}", new { Controller = "Home", action = "Shop" });
app.MapControllerRoute("legoType", "{legoType}", new { Controller = "Home", action = "Shop", pageNum = 1});
app.MapControllerRoute("legoColor", "{legoColor}", new { Controller = "Home", action = "Shop", pageNum = 1});
app.MapControllerRoute("pagination", "{pageNum}", new { Controller = "Home", action = "Shop" });

app.MapRazorPages();

app.Run();
