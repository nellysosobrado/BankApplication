using BankApplication.ViewModels;
using DAL.Models;
using Microsoft.EntityFrameworkCore;
using Services.ViewModels;
using Services.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class CustomerQueryService : ICustomerQueryService
    {
        private readonly BankAppDataContext _context;
        private readonly ICustomerSorter _sorter;

        public CustomerQueryService(BankAppDataContext context, ICustomerSorter sorter)
        {
            _context = context;
            _sorter = sorter;
        }

        public IEnumerable<Customer> GetAllCustomers() => _context.Customers;

        public async Task<CustomerDetailViewModel> GetCustomerByIdAsync(int customerId)
        {
            return await _context.Customers
                .AsNoTracking()
                .Include(c => c.Dispositions)
                    .ThenInclude(d => d.Account)
                .Where(c => c.CustomerId == customerId)
                .Select(c => new CustomerDetailViewModel
                {
                    CustomerId = c.CustomerId.ToString(),
                    Personnummer = c.NationalId ?? "N/A",
                    Name = $"{c.Givenname} {c.Surname}",
                    Address = c.Streetaddress,
                    City = c.City,
                    PostalCode = c.Zipcode,
                    Country = c.Country,
                    Phone = c.Telephonenumber ?? "N/A",
                    EmailAddress = c.Emailaddress ?? "N/A",
                    Dispositions = c.Dispositions
                        .Where(d => d.Type == "OWNER")
                        .Select(d => new DispositionViewModel
                        {
                            DispositionId = d.DispositionId,
                            Type = d.Type,
                            Account = new AccountViewModel
                            {
                                AccountId = d.Account.AccountId,
                                Balance = d.Account.Balance,
                                Frequency = d.Account.Frequency,
                                CreatedDate = d.Account.Created.ToDateTime(TimeOnly.MinValue)
                            }
                        }).ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<CustomerDetailViewModel> GetCustomerDetailAsync(int id)
        {
            return await _context.Customers
                .AsNoTracking()
                .Include(c => c.Dispositions)
                    .ThenInclude(d => d.Account)
                .Where(c => c.CustomerId == id)
                .Select(c => new CustomerDetailViewModel
                {
                    CustomerId = c.CustomerId.ToString(),
                    Personnummer = c.NationalId ?? "N/A",
                    Name = $"{c.Givenname} {c.Surname}",
                    Address = c.Streetaddress,
                    City = c.City,
                    PostalCode = c.Zipcode,
                    Country = c.Country,
                    Phone = c.Telephonenumber ?? "N/A",
                    EmailAddress = c.Emailaddress ?? "N/A",
                    Dispositions = c.Dispositions
                        .Where(d => d.Type == "OWNER")
                        .Select(d => new DispositionViewModel
                        {
                            DispositionId = d.DispositionId,
                            Type = d.Type,
                            Account = new AccountViewModel
                            {
                                AccountId = d.Account.AccountId,
                                Balance = d.Account.Balance,
                                Frequency = d.Account.Frequency,
                                CreatedDate = d.Account.Created.ToDateTime(TimeOnly.MinValue)
                            }
                        }).ToList()
                })
                .FirstOrDefaultAsync();
        }

        public async Task<PaginatedList<CustomerViewModel>> SearchAsync(
            string searchTerm, string sortColumn, string sortOrder, int pageIndex, int pageSize)
        {
            var query = _context.Customers.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = int.TryParse(searchTerm, out int id)
                    ? query.Where(c => c.CustomerId == id)
                    : query.Where(c =>
                        EF.Functions.Like(c.Givenname + " " + c.Surname, $"%{searchTerm}%") ||
                        EF.Functions.Like(c.City, $"%{searchTerm}%") ||
                        EF.Functions.Like(c.Streetaddress, $"%{searchTerm}%") ||
                        EF.Functions.Like(c.NationalId, $"%{searchTerm}%"));
            }

            query = _sorter.ApplySorting(query, sortColumn, sortOrder);

            var totalCount = await query.CountAsync();
            var items = await query.Skip((pageIndex - 1) * pageSize).Take(pageSize)
                .Select(c => new CustomerViewModel
                {
                    CustomerId = c.CustomerId.ToString(),
                    Personnummer = c.NationalId ?? "N/A",
                    Name = c.Givenname + " " + c.Surname,
                    Address = c.Streetaddress,
                    City = c.City,
                    Phone = c.Telephonenumber ?? "N/A"
                }).ToListAsync();

            return new PaginatedList<CustomerViewModel>(items, totalCount, pageIndex, pageSize);
        }

        public async Task<IEnumerable<CustomerViewModel>> GetRecentCustomersAsync(int count)
        {
            return await _context.Customers
                .AsNoTracking()
                .OrderByDescending(c => c.CustomerId)
                .Take(count)
                .Select(c => new CustomerViewModel
                {
                    CustomerId = c.CustomerId.ToString(),
                    Name = c.Givenname + " " + c.Surname,
                    City = c.City
                })
                .ToListAsync();
        }
        public async Task<PaginatedList<CustomerViewModel>> SearchCustomersAsync(
       string searchTerm, string sortColumn, string sortOrder, int pageIndex, int pageSize)
        {
            var query = _context.Customers.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = int.TryParse(searchTerm, out int id)
                    ? query.Where(c => c.CustomerId == id)
                    : query.Where(c =>
                        EF.Functions.Like(c.Givenname + " " + c.Surname, $"%{searchTerm}%") ||
                        EF.Functions.Like(c.City, $"%{searchTerm}%") ||
                        EF.Functions.Like(c.Streetaddress, $"%{searchTerm}%") ||
                        EF.Functions.Like(c.NationalId, $"%{searchTerm}%"));
            }

            query = _sorter.ApplySorting(query, sortColumn, sortOrder);

            var totalCount = await query.CountAsync();
            var items = await query.Skip((pageIndex - 1) * pageSize).Take(pageSize)
                .Select(c => new CustomerViewModel
                {
                    CustomerId = c.CustomerId.ToString(),
                    Personnummer = c.NationalId ?? "N/A",
                    Name = c.Givenname + " " + c.Surname,
                    Address = c.Streetaddress,
                    City = c.City,
                    Phone = c.Telephonenumber ?? "N/A"
                }).ToListAsync();

            return new PaginatedList<CustomerViewModel>(items, totalCount, pageIndex, pageSize);
        }

    }
}

