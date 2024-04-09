using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using BrickHaven.Models;
using Azure.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// THIS IS FOR DOTNET IDENTITY
var connectionString = builder.Configuration.GetConnectionString("BrickHavenConnection")
    ?? throw new InvalidOperationException("Connection string not found");

// Add contexts to the system
//// THIS IS FOR SQLITE
//builder.Services.AddDbContext<LoginDbContext>(options => options.UseSqlite(connectionString));
//builder.Services.AddDbContext<LegoContext>(options => options.UseSqlite(connectionString));

// THIS IS FOR SQLSERVER
builder.Services.AddDbContext<LoginDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddDbContext<LegoContext>(options => options.UseSqlServer(connectionString));

// When we refer to IWaterRepository, we actually want to use the EFWaterRepository
builder.Services.AddScoped<ILegoRepository, EFLegoRepository>();

builder.Services.AddRazorPages(); // Allows us to use MVVM

builder.Services.AddDistributedMemoryCache(); // Allows us to use session state
builder.Services.AddSession(); // Allows us to use session state


builder.Services.AddIdentity<Customer, IdentityRole>(
    options =>
    {
        // Password settings
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 8;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;
        options.Password.RequiredUniqueChars = 4;
        // Other settings can be configured here

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

// Configure token lifespan
builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
{
    // Set token lifespan to 2 hours
    options.TokenLifespan = TimeSpan.FromHours(2);
});

builder.Services.AddAuthentication()
    .AddGoogle(options =>
    {
        options.ClientId = "756498149145-mrar76ae92g6tq44ga3k27q3tu59u244.apps.googleusercontent.com";
        options.ClientSecret = "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";
    })
    .AddMicrosoftAccount(microsoftOptions =>
    {
        microsoftOptions.ClientId = "94b6a9d3-ee11-42ca-ad9f-7338b0e19cc3";
        microsoftOptions.ClientSecret = "ZD~8Q~8XandPTRYXucsHFvCcPkNgtb_mGY_zCc.A";
    }
);

// END DOTNET IDENTITY STUFF

var app = builder.Build();

// Enable the Content-Security-Policy (CSP) HTTP header
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("Content-Security-Policy", "default-src 'self'; " +
        "style-src 'self' fonts.cdnfonts.com fonts.googleapis.com 'unsafe-inline';" +
        "font-src 'self' fonts.cdnfonts.com fonts.googleapis.com fonts.gstatic.com;"); 
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
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseSession();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

// Looks at the first route, and then the second, etc., but don't show the ones after a route is found
app.MapDefaultControllerRoute();
app.MapControllerRoute("pageenumandtype", "{legoType}/{pageNum}", new { Controller = "Home", action = "Shop" });
app.MapControllerRoute("pagination", "{pageNum}", new { Controller = "Home", action = "Shop", pageNum = 1 });
app.MapControllerRoute("legoType", "{legoType}", new { Controller = "Home", action = "Shop", pageNum = 1 });

app.MapRazorPages();

app.Run();
