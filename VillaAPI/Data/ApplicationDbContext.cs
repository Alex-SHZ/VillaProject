using System;
using Microsoft.EntityFrameworkCore;
using VillaAPI.Models;

namespace VillaAPI.Data;

public class ApplicationDbContext : DbContext
{
	public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
		: base(options)
	{
    }

	public DbSet<Villa> Villas { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Villa>().HasData(
            new Villa
            {
                Id = 1,
                Name = "Royal Villa",
                Details = "Big villa bulding in 1920",
                ImageUrl = "",
                Occupancy = 4,
                Rate = 200,
                Sqft = 550,
                Amenity = "",
                CreatedDate = DateTime.Now
            },
            new Villa
            {
                Id = 2,
                Name = "Premium Pool Villa",
                Details = "Big villa bulding in 1915",
                ImageUrl = "",
                Occupancy = 4,
                Rate = 300,
                Sqft = 550,
                Amenity = "",
                CreatedDate = DateTime.Now
            },
            new Villa
            {
                Id = 3,
                Name = "Luxury Pool Villa",
                Details = "Big villa bulding in 1960",
                ImageUrl = "",
                Occupancy = 4,
                Rate = 400,
                Sqft = 750,
                Amenity = "",
                CreatedDate = DateTime.Now
            },
            new Villa
            {
                Id = 4,
                Name = "Diamond Villa",
                Details = "Big villa bulding in 2011",
                ImageUrl = "",
                Occupancy = 4,
                Rate = 550,
                Sqft = 900,
                Amenity = "",
                CreatedDate = DateTime.Now
            },
            new Villa
            {
                Id = 5,
                Name = "Diamond Pool Villa",
                Details = "Big villa bulding in 1980",
                ImageUrl = "",
                Occupancy = 4,
                Rate = 600,
                Sqft = 1100,
                Amenity = "",
                CreatedDate = DateTime.Now
            }) ;
    }
}

