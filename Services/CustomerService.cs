using DAL.Models;
using Microsoft.EntityFrameworkCore;
using BankApplication.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Services.DTOs;

namespace Services
{
    //public interface ICustomerService
    //{
    //    Task<PaginatedList<CustomerViewModel>> SearchCustomersAsync(
    //        string searchTerm,
    //        string sortColumn,
    //        string sortOrder,
    //        int pageIndex = 1,
    //        int pageSize = 50);
    //    Task<CustomerDetailViewModel> GetCustomerByIdAsync(int id);
    //    Task<IEnumerable<CustomerViewModel>> GetRecentCustomersAsync(int count = 5);
    //}
    public class CustomerService : ICustomerService
    {
        private readonly BankAppDataContext _dbContext;
        private const int DefaultPageSize = 50;

        public CustomerService(BankAppDataContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PaginatedCustomerResult> SearchCustomersAsync(
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
                query = query.Where(c =>
                    EF.Functions.Like(c.Givenname + " " + c.Surname, $"%{searchTerm}%") ||
                    EF.Functions.Like(c.City, $"%{searchTerm}%") ||
                    EF.Functions.Like(c.Streetaddress, $"%{searchTerm}%") ||
                    EF.Functions.Like(c.NationalId, $"%{searchTerm}%"));
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
                .Select(c => new CustomerListDto
                {
                    CustomerId = c.CustomerId,
                    NationalId = c.NationalId ?? "N/A",
                    FullName = c.Givenname + " " + c.Surname,
                    Streetaddress = c.Streetaddress,
                    City = c.City,
                    Telephonenumber = c.Telephonenumber ?? "N/A"
                })
                .ToListAsync();

            return new PaginatedCustomerResult
            {
                Customers = items,
                TotalCount = totalCount,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }

        public async Task<CustomerDto> GetCustomerByIdAsync(int id)
        {
            return await _dbContext.Customers
                .AsNoTracking()
                .Where(c => c.CustomerId == id)
                .Select(c => new CustomerDto
                {
                    CustomerId = c.CustomerId,
                    NationalId = c.NationalId ?? "N/A",
                    Givenname = c.Givenname,
                    Surname = c.Surname,
                    Streetaddress = c.Streetaddress,
                    City = c.City,
                    Zipcode = c.Zipcode,
                    Country = c.Country,
                    Telephonenumber = c.Telephonenumber ?? "N/A",
                    Emailaddress = c.Emailaddress ?? "N/A"
                })
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<CustomerListDto>> GetRecentCustomersAsync(int count = 5)
        {
            return await _dbContext.Customers
                .AsNoTracking()
                .OrderByDescending(c => c.CustomerId)
                .Take(count)
                .Select(c => new CustomerListDto
                {
                    CustomerId = c.CustomerId,
                    FullName = c.Givenname + " " + c.Surname,
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