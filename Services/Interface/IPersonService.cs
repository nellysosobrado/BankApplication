
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.Models;
using BankApplication.ViewModels;
using Microsoft.EntityFrameworkCore;


namespace Services.Interface;

public interface IPersonService
{
    public IEnumerable<Person> GetPersons();
    int SaveNew(Person person);

    void Update(Person person);
    Person GetPerson(int personId);
}