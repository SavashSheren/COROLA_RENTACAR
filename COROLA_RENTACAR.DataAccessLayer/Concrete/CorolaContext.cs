using COROLA_RENTACAR.EntityLayer.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COROLA_RENTACAR.DataAccessLayer.Concrete
{
    public class CorolaContext :DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=savasseren\\MSSQLSERVER02;Database=CarolaRentDb;Trusted_Connection=True;TrustServerCertificate=True;");
        }
        public DbSet<Brand>  Brands { get; set; }
        public DbSet<Car>  Cars { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Reservation> Reservations { get; set; }

    }
}
