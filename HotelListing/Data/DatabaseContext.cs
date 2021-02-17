using HotelListing.Configurations.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HotelListing.Data
{
    public class DatabaseContext : IdentityDbContext<ApiUser>
    {
        public DatabaseContext(DbContextOptions options):base(options)
        {}


        public DbSet<Country> Countries { get; set; } //Define a table named Countries in the database that store data of type Country
        public DbSet<Hotel> Hotels { get; set; }  //Define a table named Hotels in the database that store data of type Hotel 

        protected override void OnModelCreating(ModelBuilder builder) 
        {

            base.OnModelCreating(builder);

            builder.ApplyConfiguration(new RoleConfiguration()); 

            builder.Entity<Country>().HasData(
                new Country
                {
                    Id = 1,
                    Name ="South AFrica",
                    ShortName="SA"
                },
                 new Country
                 {
                     Id = 2,
                     Name = "Botswana",
                     ShortName = "BNA"
                 },
                  new Country
                  {
                      Id = 3,
                      Name = "Zimbabwe",
                      ShortName = "ZMB"
                  }
                );
            builder.Entity<Hotel>().HasData(
                new Hotel
                {
                    Id = 1,
                    Name = "Protea Hotel",
                    Address = "23 Nelson Mandel strt",
                    CountryId = 1,
                    Rating = 5

                },
                 new Hotel
                 {
                     Id = 2,
                     Name = "Sunny 1",
                     Address = "12 Samora Machel strt",
                     CountryId = 3,
                     Rating = 4.5
                 },
                  new Hotel
                  {
                      Id = 3,
                      Name = "Premier Hotel",
                      Address = "5 Oliver Tambo strt",
                      CountryId = 2,
                      Rating = 3.5
                  }
                ) ;

        }

        
    }
}
