using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DAL.Models;
using Services;
using Services.Interface;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<BankAppDataContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>() // Implements Role-based authorization
    .AddEntityFrameworkStores<BankAppDataContext>();



//builder.Services.ConfigureApplicationCookie(options =>
//{
//    options.LoginPath = "/Identity/Account/Login";
//    options.Events.OnSignedIn = async context =>
//    {
//        // Tvinga omdirigering till Dashboard, ignorera ReturnUrl
//        context.Response.Redirect("/Dashboard");
//        await Task.CompletedTask;
//    };
//});

builder.Services.AddRazorPages(); // Add Razor Pages services
builder.Services.AddTransient<DataInitializer>();// Register the DataInitializer
builder.Services.AddScoped<IStatsService, StatsService>(); // Register the StatsService

//builder.Services.AddScoped<ICustomerService, CustomerService>();//Register the service CustomerService to get our customers data
builder.Services.AddScoped<ICustomerQueryService, CustomerQueryService>();
builder.Services.AddScoped<ICustomerCommandService, CustomerCommandService>();
builder.Services.AddScoped<ICustomerSorter, CustomerSorter>();
builder.Services.AddScoped<ITransactionService, TransactionService>();



// In Program.cs (or Startup.cs in older versions)
builder.Services.AddScoped<IPersonService, PersonService>(); // Register the StatsService
builder.Services.AddTransient<IAccountService, AccountService>();//Register the service AccountService to get our accounts data

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    scope.ServiceProvider.GetService<DataInitializer>().SeedData();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts(); // HTTP Strict Transport Security
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();


app.UseAuthentication(); 
app.UseAuthorization();

app.MapStaticAssets(); 
app.MapRazorPages()
   .WithStaticAssets();

app.Run();