
using SaaSPOCModel.Security;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace SaaSPOCService.Contexts
{
   
    public class MasterDBContext : IdentityDbContext
    {
        public MasterDBContext()
        {

        }
        public MasterDBContext(DbContextOptions options): base(options)
        {

        }
        #region ctors and default methods
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            if (!optionsBuilder.IsConfigured)
            {
                IConfigurationRoot configuration = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json")
                   .Build();
                var POCMaster_db = "POCMaster_db";
                var connectionString = configuration.GetConnectionString(POCMaster_db);

                optionsBuilder.UseSqlServer(connectionString);

            }
        }
        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{

            
        //    modelBuilder.Entity<ApplicationUser>()
        //                .Property(t => t.IsDeleted)
        //                .IsRequired(true);
        //    modelBuilder.Entity<ApplicationUser>()
        //               .Property(t => t.IsPaidUser)
        //               .IsRequired(true);
        //    //OnModelCreating(modelBuilder);
        //}
       

        #endregion ctors and default methods  
        public DbSet<ApplicationUser> ApplicationUser { get; set; }
        

    }
}
