using DAL.Models;
using Microsoft.EntityFrameworkCore;
using BankApplication.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Services.Interface;

namespace Services
{

    public class CustomerService : ICustomerService
    {
        private readonly BankAppDataContext _dbContext;
        private const int DefaultPageSize = 50;

        public CustomerService(BankAppDataContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PaginatedList<CustomerViewModel>> SearchCustomersAsync(
    string searchTerm,
    string sortColumn,
    string sortOrder,
    int pageIndex = 1,
    int pageSize = DefaultPageSize)
        {
            // Validate parameters
            if (pageIndex < 1) pageIndex = 1;
            if (pageSize < 1) pageSize = DefaultPageSize;

            var query = _dbContext.Customers.AsNoTracking().AsQueryable();

            // Search functionality (case-insensitive)
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                // Try to parse searchTerm as CustomerId (int)
                if (int.TryParse(searchTerm, out int customerId))
                {
                    query = query.Where(c => c.CustomerId == customerId);
                }
                else
                {
                    query = query.Where(c =>
                        EF.Functions.Like(c.Givenname + " " + c.Surname, $"%{searchTerm}%") ||
                        EF.Functions.Like(c.City, $"%{searchTerm}%") ||
                        EF.Functions.Like(c.Streetaddress, $"%{searchTerm}%") ||
                        EF.Functions.Like(c.NationalId, $"%{searchTerm}%"));
                }
            }

            // Sorting implementation
            query = sortColumn switch
            {
                "CustomerId" when sortOrder == "asc" => query.OrderBy(c => c.CustomerId),
                "CustomerId" when sortOrder == "desc" => query.OrderByDescending(c => c.CustomerId),
                "Personnummer" when sortOrder == "asc" => query.OrderBy(c => c.NationalId),
                "Personnummer" when sortOrder == "desc" => query.OrderByDescending(c => c.NationalId),
                "Name" when sortOrder == "asc" => query.OrderBy(c => c.Surname),
                "Name" when sortOrder == "desc" => query.OrderByDescending(c => c.Surname),
                "Address" when sortOrder == "asc" => query.OrderBy(c => c.Streetaddress),
                "Address" when sortOrder == "desc" => query.OrderByDescending(c => c.Streetaddress),
                "City" when sortOrder == "asc" => query.OrderBy(c => c.City),
                "City" when sortOrder == "desc" => query.OrderByDescending(c => c.City),
                _ => query.OrderBy(c => c.Surname) // Default sorting by surname
            };

            // Calculate total count
            var totalCount = await query.CountAsync();

            // Get data for current page
            var items = await query
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new CustomerViewModel
                {
                    CustomerId = c.CustomerId.ToString(), // Convert to string
                    Personnummer = c.NationalId ?? "N/A",
                    Name = c.Givenname + " " + c.Surname,
                    Address = c.Streetaddress,
                    City = c.City,
                    Phone = c.Telephonenumber ?? "N/A"
                })
                .ToListAsync();

            return new PaginatedList<CustomerViewModel>(items, totalCount, pageIndex, pageSize);
        }

        public async Task<CustomerDetailViewModel> GetCustomerByIdAsync(int id)
        {
            return await _dbContext.Customers
                .AsNoTracking()
                .Where(c => c.CustomerId == id)
                .Select(c => new CustomerDetailViewModel
                {
                    CustomerId = c.CustomerId.ToString(), // Convert to string
                    Personnummer = c.NationalId ?? "N/A",
                    Name = c.Givenname + " " + c.Surname,
                    Address = c.Streetaddress,
                    City = c.City,
                    PostalCode = c.Zipcode,
                    Country = c.Country,
                    Phone = c.Telephonenumber ?? "N/A",
                    EmailAddress = c.Emailaddress ?? "N/A", // Changed to match view model
                    // Remove Gender and Birthday if not in view model
                })
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<CustomerViewModel>> GetRecentCustomersAsync(int count = 5)
        {
            return await _dbContext.Customers
                .AsNoTracking()
                .OrderByDescending(c => c.CustomerId)
                .Take(count)
                .Select(c => new CustomerViewModel
                {
                    CustomerId = c.CustomerId.ToString(), // Convert to string
                    Name = c.Givenname + " " + c.Surname,
                    City = c.City
                })
                .ToListAsync();
        }
    }

    public class PaginatedList<T>
    {
        public List<T> Items { get; }
        public int TotalCount { get; }
        public int PageIndex { get; }
        public int PageSize { get; }
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            Items = items ?? throw new ArgumentNullException(nameof(items));
            TotalCount = count;
            PageIndex = pageIndex;
            PageSize = pageSize;
        }

        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;
    }
}