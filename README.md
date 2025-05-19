#  Bank Application


This is a banking system designed for **bank staff**, such as cashiers and admins, to manage customers, accounts, and system users.  
It is built with **ASP.NET Core (Razor Pages)** and **SQL Server**.

The project also includes a **FraudDetectionConsoleApp** – a separate console application that analyzes transactions to detect suspicious activity, such as users exceeding defined money transfer limits.


---

##  Live Demo (Hosted on Azure)

-  **Web App**: [https://bankapp20250511-hkcke7gsgzc8atbz.swedencentral-01.azurewebsites.net/](https://bankapp20250511-hkcke7gsgzc8atbz.swedencentral-01.azurewebsites.net/)
-  **Database**: Also hosted in **Azure SQL Database**
-  **Database is scaffolded** (generated from an existing SQL Server schema with existing users)

---

##  Roles & Login

| Role   | Description                        |
|--------|------------------------------------|
| Admin  | Manage system users (create, delete, update) |
| Cashier| Manage customers and accounts (CRUD)         |

###  Seeded test users

| Role     | Username                    | Password  |
|----------|-----------------------------|-----------|
| Admin    | `richard.chalk@admin.se`    | `Abc123#` |
| Cashier  | `richard.chalk@cashier.se`  | `Abc123#` |

---

##  Features

- View, search and manage customers
- View accounts & transactions with AJAX "load more"
- Deposit, withdraw and transfer money
- Search customers by name and city (with paging)
- Admin panel for user management (Identity roles)
- Dashboard with statistics

---

##  Principles Used

###  DRY – Don’t Repeat Yourself
- The system avoids duplication by reusing shared logic in the `DAL` and `Services` layers.
- The `FraudDetectionConsoleApp` uses the same `DbContext`, services and models as the web app.

###  Separation of Concerns
- Business logic is placed in `Services` and `Rules`.
- UI (Razor Pages) only handles presentation and routing.

---

##  Error Handling & Validation

- All user inputs are validated with **data annotations**:
  ```csharp
  [Required]
  [EmailAddress]
  [MaxLength(50)]


##  NuGet Packages Used

| Package                                                       | Purpose                                         |
|---------------------------------------------------------------|-------------------------------------------------|
| `Microsoft.AspNetCore.Identity.EntityFrameworkCore`           | ASP.NET Identity with EF Core                   |
| `Microsoft.EntityFrameworkCore.SqlServer`                     | SQL Server provider for EF Core                 |
| `Microsoft.EntityFrameworkCore.Tools`                         | EF Core CLI tools (e.g. migrations, scaffolding)|
| `Microsoft.EntityFrameworkCore.Design`                        | Design-time support for EF Core (e.g. scaffolding) |
| `AutoMapper`                                                  | Mapping between entities and viewmodels         |
| `AutoMapper.Extensions.Microsoft.DependencyInjection`         | Registers AutoMapper into ASP.NET Core DI       |
| `Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation`           | Enables Razor Page live reload without rebuild  |
| `Newtonsoft.Json` or `System.Text.Json`                       | JSON handling (used in fraud console reporting) |
