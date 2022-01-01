using GenericRepository.IdentityRepository;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using SaaSPOCModel.Security;
using SaaSPOCService.Contexts;
using SaaSPOCServices.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SaaSPOCServices.SubscriptionService
{
  public  class SubscriptionService
    {
        public async Task<bool> DedicatedDBExists(string userId)
        {
             
            IdentityUnitOfWork<MasterDBContext> _masterUnitOfWork=new IdentityUnitOfWork<MasterDBContext>();
            try
            {

                using (var userRepository = new IdentityRepository<ApplicationUser, MasterDBContext>(_masterUnitOfWork))
                {
                    IConfigurationRoot configuration = new ConfigurationBuilder()
                     .SetBasePath(Directory.GetCurrentDirectory())
                     .AddJsonFile("appsettings.json")
                     .Build();
                    var POCMaster_db = "POCMaster_db";
                    var connectionStringMaster = configuration.GetConnectionString(POCMaster_db);

                    var appUser = userRepository.FirstOrDefault(u => u.Id == userId);

                  

                    
                    var sql = $"SELECT name FROM sys.databases where name='{appUser.DedicatedDatabaseName}'"; 

                    var dt = Utilities.ExecuteDataSet(connectionStringMaster, sql).Tables[0];
                    //var dbCount = dt.Rows.Count>0;
                    return dt.Rows.Count > 0;// (bool)(userRepository.FirstOrDefault(u => u.Id == userId)?.IsPaidUser);
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        //public async Task<object> PostApplicationUserAsync(VMRegister model)
        //{
        //    ApplicationUser applicationUserExists = null; // 

        //    try
        //    {


        //        applicationUserExists = await _userManager.FindByEmailAsync(model.Email);

        //        if (applicationUserExists != null)
        //        {

        //            return Problem($"User {model.Email} already exists. Did you forget your password?");

        //        }
        //        else
        //        {
        //            var role = Role.User;
        //            var applicationUserNew = new ApplicationUser();
        //            CommonExtensions.CopyPropertiesTo(model, applicationUserNew);
        //            applicationUserNew.UserName = model.Email;
        //            var dedicatedDBName = Utilities.GetDBNameFromUserEmail(model.Email);
        //            applicationUserNew.DedicatedDatabaseName = dedicatedDBName;
        //            applicationUserNew.IsPaidUser = model.PaymentAmount > 0;
        //            var result = await _userManager.CreateAsync(applicationUserNew, model.Password);
        //            await _userManager.AddToRoleAsync(applicationUserNew, role);

        //            if (result.Succeeded)
        //            {


        //                if (applicationUserNew.IsPaidUser)//goes to dedicated DB
        //                {
        //                    //dedicated database name. some format of email address is valid name for database name so need some validations.


        //                    //set on user session, not in any static variable.
        //                    HttpContext.Session.SetString("new_databasename", dedicatedDBName);

        //                    var fps = new FavoritePL();
        //                    fps.CreatedById = applicationUserNew.Id;
        //                    fps.FavoriteProgrammingLanguages = model.FavoriteProgrammingLanguages;
        //                    fps.FavoriteIDEs = model.FavoriteIDEs;

        //                    ///create DB 1st 
        //                    var unitOfWork = new UnitOfWork<DedicatedDBContext>();
        //                    var favoritePLRepository = new GenericRepository<FavoritePL, DedicatedDBContext>(unitOfWork);
        //                    favoritePLRepository.Add(fps);
        //                    unitOfWork.Save();


        //                }
        //                else  //goes to shared
        //                {
        //                    var fps = new FavoritePL();
        //                    fps.CreatedById = applicationUserNew.Id;
        //                    fps.FavoriteProgrammingLanguages = model.FavoriteProgrammingLanguages;
        //                    fps.FavoriteIDEs = model.FavoriteIDEs;

        //                    var unitOfWork = new UnitOfWork<SharedDBContext>();
        //                    var favoritePLRepository = new GenericRepository<FavoritePL, SharedDBContext>(unitOfWork);
        //                    favoritePLRepository.Add(fps);
        //                    unitOfWork.Save();
        //                }
        //                return Ok(new { Msg = $"User was created successfully.", StatusCode = 0 });
        //            }
        //            else
        //            {
        //                return Problem($"Failed to create user {model.Email}. Error: {result}");
        //            }


        //        }



        //    }
        //    catch (Exception ex)
        //    {
        //        applicationUserExists = await _userManager.FindByEmailAsync(model.Email);
        //        await _userManager.DeleteAsync(applicationUserExists);
        //        return Problem($"Server error occured.");
        //    }


        //    return "";
        //}
    }
}
