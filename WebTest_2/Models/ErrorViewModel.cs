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
        public DateTime DateBirth { get; set; }
        public List<Phone> Phones { get; set; } = new List<Phone>();
        public List<PhoneBook> PhoneBooks { get; set; } = new List<PhoneBook>();
    }

    public class Phone
    {
        public Guid Id { get; set; }
        public string PhoneNumber { get; set; }
        public List<User> Users { get; set; } = new List<User>();
        public List<PhoneBook> PhoneBooks { get; set; } = new List<PhoneBook>();
    }

    public class PhoneBook
    {
        public Guid PhoneId { get; set; }
        public User User { get; set; }

        public Guid UserId { get; set; }
        public Phone Phone { get; set; }

        public DateTime InsertDate { get; set; } // дата
    }

    // public class PhoneUsers
    // {
    //     public Guid Id { get; set; }
    //     public DateTime DateCr { get; set; }
    // }




    public class SqlDBContext : DbContext
    {
        public DbSet<User> Users/*»м€ таблицы*/{ get; set; }
        public DbSet<Phone> Phones/*им€ таблицы*/{ get; set; }

        public DbSet<PhoneBook> PhoneBooks/*им€ таблицы*/{ get; set; }
        // public DbSet<PhoneUsers> PhoneUsers/*им€ таблицы*/{ get; set; }
         
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Phone>()
                .HasMany(c => c.Users)
                .WithMany(s => s.Phones)
                .UsingEntity<PhoneBook>(
                   j => j
                    .HasOne(pt => pt.User)
                    .WithMany(t => t.PhoneBooks)
                    .HasForeignKey(pt => pt.UserId),
                j => j
                    .HasOne(pt => pt.Phone)
                    .WithMany(p => p.PhoneBooks)
                    .HasForeignKey(pt => pt.PhoneId),
                j =>
                {
                    j.Property(pt => pt.InsertDate).HasDefaultValueSql("CURRENT_TIMESTAMP");
                    j.HasKey(t => new { t.UserId, t.PhoneId });
                    j.ToTable("PhoneBooks");
                });
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=(local); Database=Site_2; Persist Security Info=false; MultipleActiveResultSets=True; Trusted_Connection=True;");
        }
    }

}
