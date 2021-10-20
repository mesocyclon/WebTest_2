using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace WebTest_2.Models
{
    public class ErrorViewModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }

    public class User
    {
        public Guid Id { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public DateTime Date1 { get; set; }

       // public List<User> Users { get; set; }
    }

    public class Phone
    {
        public Guid PhoneId { get; set; }
        public string PhoneNumber { get; set; }
    }

    public class SqlDBContext : DbContext
    {
        public DbSet<User> Users/*Имя таблицы*/{ get; set; }
        public DbSet<Phone> Phones/*имя таблицы*/{ get; set; }
  

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=(local); Database=Site_2; Persist Security Info=false; MultipleActiveResultSets=True; Trusted_Connection=True;");
        }
    }

}
