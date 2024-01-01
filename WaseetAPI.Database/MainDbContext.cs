using WaseetAPI.Domain.Models;
using WaseetAPI.Domain.Models.Salla;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaseetAPI.Database
{
    public class MainDbContext : DbContext
    {
        public MainDbContext(DbContextOptions<MainDbContext> options)
          : base(options) { }

        public DbSet<Users> users { get; set; }
     
      
        public DbSet<Admin> admin { get; set; }
        public DbSet<Companies> companies { get; set; }
        public DbSet<Logs> logs { get; set; }
        public DbSet<SallaUsers> sallausers { get; set; }
        public DbSet<SallaWebhookRequests> sallawebhookrequests { get; set; }
        public DbSet<SallaMerchantApp> sallamerchantapp { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<SallaMerchantApp>().HasKey(table => new
            {
                table.id,
                table.merchant_id,
                table.app_name
            });
        }
    }
}
