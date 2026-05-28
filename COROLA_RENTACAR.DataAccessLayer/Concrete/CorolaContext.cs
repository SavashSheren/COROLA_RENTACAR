using COROLA_RENTACAR.EntityLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace COROLA_RENTACAR.DataAccessLayer.Concrete
{
    public class CorolaContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=savasseren\\MSSQLSERVER02;Database=CarolaRentDb;Trusted_Connection=True;TrustServerCertificate=True;");
        }

        public DbSet<Brand> Brands { get; set; }
        public DbSet<Car> Cars { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<CarImage> CarImages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Car>()
      .HasOne(x => x.Brand)
      .WithMany(x => x.Cars)
      .HasForeignKey(x => x.BrandId)
      .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Car>()
    .HasOne(x => x.Category)
    .WithMany(x => x.Cars)
    .HasForeignKey(x => x.CategoryId)
    .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<CarImage>()
                .HasOne(x => x.Car)
                .WithMany(x => x.CarImages)
                .HasForeignKey(x => x.CarId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Reservation>()
       .HasOne(x => x.Car)
       .WithMany(x => x.Reservations)
       .HasForeignKey(x => x.CarId)
       .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Reservation>()
                .HasOne(x => x.Customer)
                .WithMany(x => x.Reservations)
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Reservation>()
                .HasOne(x => x.PickupLocation)
                .WithMany()
                .HasForeignKey(x => x.PickupLocationId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Reservation>()
                .HasOne(x => x.ReturnLocation)
                .WithMany()
                .HasForeignKey(x => x.ReturnLocationId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}