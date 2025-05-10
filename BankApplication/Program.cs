using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DAL.Models;
using Services;
using Services.Interface;
using Microsoft.AspNetCore.Authentication.Cookies;
using Services.Profiles;

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

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();


app.UseAuthentication(); 
app.UseAuthorization();

app.MapStaticAssets(); 
app.MapRazorPages()
   .WithStaticAssets();

app.Run();