
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Models;
using BankApplication.ViewModels;
using Microsoft.EntityFrameworkCore;
using Services.Interface;


namespace Services
{
    public class PersonService : IPersonService
    {
        private readonly BankAppDataContext _context;

        public PersonService(BankAppDataContext context)
        {
            _context = context;
        }
        public IEnumerable<Person> GetPersons()
        {
            return _context.Person;
        }

        public int SaveNew(Person person)
        {
            person.Registered = DateTime.UtcNow;
            person.LastModified = DateTime.UtcNow;
            _context.Person.Add(person);
            _context.SaveChanges();
            return person.Id;
        }

        public void Update(Person person)
        {
            person.LastModified = DateTime.UtcNow;
            _context.SaveChanges();
        }

        public Person GetPerson(int personId)
        {
            return _context.Person.First(e => e.Id == personId);
        }
    }

}

