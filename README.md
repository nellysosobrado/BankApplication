Bank application

Project for managing a bank.
It is built with ASP.NET Core using Razor Pages, which helps create web pages with C# and HTML together. 
The project also connects to a real SQL Server database.

This system is for **bank employees only** (not customers).  
There are two roles:

- **Admin**: Can manage users, customers, and accounts
- **Cashier**: Can manage customers and accounts

Two users are automatically added when the app starts:
- Username:`richard.chalk@admin.se` / Password: `Abc123#`
- Username:`richard.chalk@cashier.se` /Password: `Abc123#`

##  Features

- View customers and their accounts
- View account transactions
- Deposit, withdraw, and transfer money
- Search customers by name and city (with paging)
- Admin can manage system users
- Start page shows statistics (open to everyone)
- AJAX "load more" for transactions
- Console app checks for suspicious transactions

Link to live website: razorcrudazur250509.azurewebsites.net
