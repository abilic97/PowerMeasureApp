using Microsoft.EntityFrameworkCore;
using PowerMeasure.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PowerMeasure.Data
{
    public class PowerMeasureDbContext : DbContext
    {
        public PowerMeasureDbContext(DbContextOptions<PowerMeasureDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User_Role>()
                .HasOne(x => x.Role)
                .WithMany(y => y.Roles)
                .HasForeignKey(x => x.RoleId);

            modelBuilder.Entity<User_Role>()
                .HasOne(x => x.User)
                .WithMany(y => y.Roles)
                .HasForeignKey(x => x.UserId);

            modelBuilder.Entity<UserContract>()
                .HasOne(a => a.EnergyMeter)
                .WithOne(b => b.Contract).HasForeignKey<EnergyMeter>(e => e.UserContractRef);
        }

        public DbSet<Users> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<User_Role> User_Roles { get; set; }
        public DbSet<UserContract> Contracts { get; set; }
        public DbSet<EnergyMeter> Meter { get; set; }
        public DbSet<EnergyConsumed> Consumption { get; set; }
        public DbSet<Bill> Bills { get; set; }

    }
}
