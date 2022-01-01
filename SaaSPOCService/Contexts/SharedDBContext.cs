
using SaaSPOCModel.Security;
using SaaSPOCModel.UserInfo;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;
using Microsoft.Data.SqlClient;

namespace SaaSPOCService.Contexts
{
    /// <summary>
    /// All free user's personal information
    /// </summary>
    public class SharedDBContext : DbContext
    {
        private string _SharedSchemaCreateDB;
        private string _SharedSchemaTable;
        public SharedDBContext()
        {
            _SharedSchemaCreateDB = @"CREATE DATABASE POCShared_db";
            _SharedSchemaTable = @"CREATE TABLE [dbo].[FavoritePL](
	        [Id] [uniqueidentifier] NOT NULL,
	        [FavoriteProgrammingLanguages] [nvarchar](max) NOT NULL,
	        [FavoriteIDEs] [nvarchar](max) NOT NULL,
	        [IsDeleted] [bit] NULL,
	        [CreatedById] [nvarchar](450) NULL,
	        [CreatedTime] [datetime2](7)  NULL,
	        [UpdatedById] [nvarchar](450)  NULL,
	        [UpdatedTime] [datetime2](7) NULL,
	        [CreatedByIP] [varchar](80)  NULL,
	        [UpdatedByIP] [varchar](80)  NULL,
         CONSTRAINT [PK_FavoritePL] PRIMARY KEY CLUSTERED 
        (
	        [Id] ASC
        )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
        ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
        ";

        }
        public SharedDBContext(DbContextOptions options) : base(options)
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

                //Optional: for auto create shared database and tables. Can be skkipped from settings or if the shared database is already exists.
                var POCShared_db = "POCShared_db";
               

                var connectionStringShared = configuration.GetConnectionString(POCShared_db);
                try
                {
                  

                    optionsBuilder.UseSqlServer(connectionStringShared);
                    SqlConnection sqlConnection = new SqlConnection(connectionStringShared);
                    //need other way to validate
                    sqlConnection.Open();

                    sqlConnection.Close();
                }
                catch (System.Exception)
                {

                    //Run script using master_db
                    try
                    {
                        var POCMaster_db = "POCMaster_db";
                        var connectionStringMaster = configuration.GetConnectionString(POCMaster_db);
                      

                        SqlConnection sqlConnection = new SqlConnection(connectionStringMaster);
                        sqlConnection.Open();
                        SqlCommand cmd = new SqlCommand(_SharedSchemaCreateDB, sqlConnection);
                        try
                        {
                            var res = cmd.ExecuteNonQuery();
                        }
                        catch (System.Exception ex)
                        {
                            if (ex.Message.Contains("already exists. Choose a different database name") == true)
                            {
                                //just ignore for now
                            }
                            else
                            {
                                throw;
                            }
                        }
                        sqlConnection.Close();
                        sqlConnection.ConnectionString = connectionStringShared;
                        sqlConnection.Open();
                        cmd = new SqlCommand(_SharedSchemaTable, sqlConnection);

                        var res2 = cmd.ExecuteNonQuery();
                    }
                    catch (System.Exception ex)
                    {
                        if (ex.Message.Contains("There is already an object named 'FavoritePL' in the database.") == true)
                        {
                            //just ignore for now
                        }
                        else
                        {
                            throw;
                        }
                    }

                    //end optional code


                    optionsBuilder.UseSqlServer(connectionStringShared);
                }


            }
        }


        #endregion ctors and default methods  
        public DbSet<FavoritePL> FavoritePL { get; set; }


    }
}
