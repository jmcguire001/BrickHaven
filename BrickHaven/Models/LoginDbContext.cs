﻿using BrickHaven.Models;
using BrickHaven.Models.ViewModels;
using Microsoft.AspNetCore.Hosting.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace BrickHaven.Models
{
    public class LoginDbContext : IdentityDbContext<Customer, IdentityRole, string>
    {
        public LoginDbContext(DbContextOptions<LoginDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Customizing the ASP.NET Identity model and overriding the defaults if needed

            builder.Entity<IdentityUserRole<string>>()
                   .HasOne<IdentityRole>()
                   .WithMany()
                   .HasForeignKey(ur => ur.RoleId)
                   .OnDelete(DeleteBehavior.NoAction);

            //The following code will set ON DELETE NO ACTION to all Foreign Keys
            //foreach (var foreignKey in builder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            //{
            //    foreignKey.DeleteBehavior = DeleteBehavior.NoAction;
            //}
        }
    }
}