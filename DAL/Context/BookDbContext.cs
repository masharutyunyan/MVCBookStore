﻿using DAL.Entities;
using DAL.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace DAL.Context
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }
        public DbSet<Book> Books { get; set; }
        public DbSet<Country> Countires { get; set; }
        public DbSet<Genre> Genres{ get; set; }
        public DbSet<GenreBook> GenreBooks { get; set; }
    }
}
