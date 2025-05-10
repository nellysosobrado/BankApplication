using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DAL.Models;
using Services;
using Services.Interface;
using Microsoft.AspNetCore.Authentication.Cookies;
using Services.Profiles;
using Services.Services;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<BankAppDataContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>() 
    .AddEntityFrameworkStores<BankAppDataContext>();


builder.Services.AddRazorPages(); 
builder.Services.AddTransient<DataInitializer>();
builder.Services.AddScoped<IStatsService, StatsService>(); 


builder.Services.AddScoped<ICustomerQueryService, CustomerQueryService>();
builder.Services.AddScoped<ICustomerCommandService, CustomerCommandService>();
builder.Services.AddScoped<ICustomerSorter, CustomerSorter>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddResponseCaching();



builder.Services.AddScoped<IPersonService, PersonService>(); 
builder.Services.AddTransient<IAccountService, AccountService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.
         GetRequiredService<BankAppDataContext>();
    if (dbContext.Database.IsRelational())
    {
        dbContext.Database.Migrate();
    }
    scope.ServiceProvider.GetService<DataInitializer>().SeedData();
}



//using (var scope = app.Services.CreateScope())
//{
//    scope.ServiceProvider.GetService<DataInitializer>().SeedData();
//}

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

app.UseResponseCaching();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();


app.UseAuthentication(); 
app.UseAuthorization();

app.MapStaticAssets(); 
app.MapRazorPages()
   .WithStaticAssets();

app.MapGet("/TopCustomers", async (HttpContext http, string country, BankAppDataContext db) =>
{
    http.Response.GetTypedHeaders().CacheControl = new Microsoft.Net.Http.Headers.CacheControlHeaderValue()
    {
        Public = true,
        MaxAge = TimeSpan.FromMinutes(1)
    };

    http.Response.Headers["Vary"] = "country";

    var topCustomers = await db.Customers
        .Where(c => c.Country == country)
        .Select(c => new
        {
            Name = c.Givenname + " " + c.Surname,
            TotalBalance = c.Dispositions
                .Select(d => d.Account)
                .Sum(a => (decimal?)a.Balance ?? 0)
        })
        .OrderByDescending(c => c.TotalBalance)
        .Take(10)
        .ToListAsync();

    return Results.Ok(topCustomers);
});



app.Run();