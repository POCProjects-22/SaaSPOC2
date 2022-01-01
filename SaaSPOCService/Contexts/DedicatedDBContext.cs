using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SaaSPOCModel.UserInfo;
using System.IO;


namespace SaaSPOCService.Contexts
{

    /// <summary>
    /// only the paid user's personal information
    /// </summary>
    public class DedicatedDBContext : DbContext
    {
        // private string _pocDedicatedDBName;
        private string _dedicatedSchemaCreateDB;
        private string _dedicatedSchemaTable;
        private bool _isNewUser { get; set; }
        #region ctors and default methods
        public DedicatedDBContext()
        {
            _dedicatedSchemaCreateDB = @"CREATE DATABASE {dedicateddbname}";
            _dedicatedSchemaTable = @"CREATE TABLE [dbo].[FavoritePL](
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

     
        public DedicatedDBContext(DbContextOptions options) : base(options)
        {

        }
       
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            if (!optionsBuilder.IsConfigured)
            {
                IConfigurationRoot configuration = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json")
                   .Build();

                var POCMaster_db = "POCMaster_db";
                var connectionStringMaster = configuration.GetConnectionString(POCMaster_db);
                var POCDedicated_db = "POCDedicated_db";
                IHttpContextAccessor _httpContextAccessor = new HttpContextAccessor();
                string dedicatedDBName = _httpContextAccessor.HttpContext.Session.GetString("new_databasename");
                if (string.IsNullOrEmpty(dedicatedDBName))
                {
                    _isNewUser = false;
                    dedicatedDBName = _httpContextAccessor.HttpContext.Session.GetString("old_databasename");
                }
                else
                {
                    _isNewUser = true;
                }
                var connectionStringDedicated = configuration.GetConnectionString(POCDedicated_db).Replace("{dedicateddbname}", dedicatedDBName);
                //optionsBuilder.UseSqlServer(connectionStringMaster);

                if (_isNewUser)
                {
                    //Run script using master_db
                    try
                    {
                        _dedicatedSchemaCreateDB = _dedicatedSchemaCreateDB.Replace("{dedicateddbname}", dedicatedDBName);
                        SqlConnection sqlConnection = new SqlConnection(connectionStringMaster);
                        sqlConnection.Open();
                        SqlCommand cmd = new SqlCommand(_dedicatedSchemaCreateDB, sqlConnection);
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
                        sqlConnection.ConnectionString = connectionStringDedicated;
                        sqlConnection.Open();
                        cmd = new SqlCommand(_dedicatedSchemaTable, sqlConnection);

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
                }
                ///////////////////////////////////////////////////



                optionsBuilder.UseSqlServer(connectionStringDedicated);

            }
        }


        #endregion ctors and default methods  
        public DbSet<FavoritePL> FavoritePL { get; set; }
       
    }
}
