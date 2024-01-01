using WaseetAPI.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Reflection;

namespace WaseetAPI.Database
{ 
    public class ApplicationDbContext : DbContext
    {
        private readonly string _connectionString;
        public ApplicationDbContext(string connectionString)
        {
            _connectionString = connectionString;
        }
        public DbSet<LocalUser> localUser { get; set; }

        public DbSet<Customers> customers { get; set; }
        public DbSet<Invoices> invoices { get; set; }
        public DbSet<InvoicesProducts> invoicesProducts { get; set; }
        public DbSet<Receipts> receipts { get; set; }
        public DbSet<ReceiptsInvoices> receiptsInvoices { get; set; }
   
        public DbSet<SERIALS> serials { get; set; }
        public DbSet<VOU_SETTING> vou_setting { get; set; }
        public DbSet<WSHOW> wshow { get; set; }        
        public DbSet<UsersWarehouse> usersWarehouse { get; set; }
        public DbSet<UserStore> userStore { get; set; }
        public DbSet<Products> products { get; set; }
        public DbSet<BATCH> batch { get; set; }
        public DbSet<ANET> anet { get; set; }
        public DbSet<MobilePermissions> mobilepermissions { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<VOU_SETTING>().HasKey(table => new
            {
                table.FTYPE,
                table.FTYPE2
            });

            modelBuilder.Entity<WSHOW>().HasKey(table => new
            {
                table.ftype,
                table.bran,
                table.code
            });

            modelBuilder.Entity<SERIALS>().HasKey(table => new
            {
                table.BTYPE,
                table.BRAN,
                table.CODE,
                table.FTYPE,
                table.FTYPE2
            });
            modelBuilder.Entity<BATCH>().HasNoKey();
            modelBuilder.Entity<ANET>().HasNoKey();
            //modelBuilder.Entity<Invoices>().HasKey(table => new {
            //    table.invoice_no,
            //    table.invoice_type
            //});

            //modelBuilder.Entity<Product>()
            //    .HasKey(x => new { x.Id });

            //base.OnModelCreating(modelBuilder);
            //modelBuilder.Entity<SocialConfig>()
            //    .HasKey(x => new { x.Id });

            //base.OnModelCreating(modelBuilder);
            //modelBuilder.Entity<OrgInfo>()
            //    .HasKey(x => new { x.Id });
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_connectionString);
        }
        public List<T> ExecSQL<T>(string query, List<object> query_values)
        {
            List<T> list = new List<T>();
            try
            {
                using (var command = Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = query;
                    command.CommandType = CommandType.Text;
                    Database.OpenConnection();

                    using (var result = command.ExecuteReader())
                    {
                        T obj = default(T);
                        while (result.Read())
                        {
                            obj = Activator.CreateInstance<T>();
                            foreach (PropertyInfo prop in obj.GetType().GetProperties())
                            {
                                if (!object.Equals(result[prop.Name], DBNull.Value))
                                {
                                    prop.SetValue(obj, result[prop.Name], null);
                                }
                            }
                            list.Add(obj);
                        }
                    }
                    Database.CloseConnection();
                }
            }
            catch
            {
            }
            return list;
        }
    }
}
