using GenericRepository;
using GenericRepository.Extensions;
using GenericRepository.IdentityRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SaaSPOCAPI.ViewModels;
using SaaSPOCModel.Security;
using SaaSPOCModel.UserInfo;
using SaaSPOCService.Contexts;
using SaaSPOCServices.Helper;
using SaaSPOCServices.SubscriptionService;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SaaSPOCAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationUserController : ControllerBase
    {
        private IdentityUnitOfWork<MasterDBContext> _masterUnitOfWork { get; set; }

        private IdentityRepository<ApplicationUser, MasterDBContext> _userRepository;
        
        private UserManager<ApplicationUser> _userManager;
        private SignInManager<ApplicationUser> _signInManager;



        private readonly ApplicationSettings _appSettings;
        public ApplicationUserController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
            IOptions<ApplicationSettings> appSettings, IHttpContextAccessor accessor)
        {
            //IPAddress remoteIpAddress = accessor.HttpContext.Connection.RemoteIpAddress;


            _masterUnitOfWork = new IdentityUnitOfWork<MasterDBContext>(accessor.HttpContext);



            _userRepository = new IdentityRepository<ApplicationUser, MasterDBContext>(_masterUnitOfWork);

            _userManager = userManager;
            _signInManager = signInManager;
            _appSettings = appSettings.Value;
        }

        [HttpPost]
        [Route("Register")]
        //POST : /api/ApplicationUser/Register
        public async Task<IActionResult> RegisterUserAsync(VMRegister model)
        {
            ApplicationUser applicationUserExists = null; // 
           
            try
            {
              

                applicationUserExists = await _userManager.FindByEmailAsync(model.Email);

                if (applicationUserExists != null)
                {

                    return Problem($"User {model.Email} already exists. Did you forget your password?" );

                }
                else
                {
                    var role = Role.User;
                    var applicationUserNew = new ApplicationUser();
                    CommonExtensions.CopyPropertiesTo(model, applicationUserNew);
                    applicationUserNew.UserName = model.Email;
                    var dedicatedDBName = Utilities.GetDBNameFromUserEmail(model.Email);
                    applicationUserNew.DedicatedDatabaseName = dedicatedDBName;
                    applicationUserNew.IsPaidUser = model.PaymentAmount > 0;
                    var result = await _userManager.CreateAsync(applicationUserNew, model.Password);
                    await _userManager.AddToRoleAsync(applicationUserNew, role);

                    if (result.Succeeded)
                    {
                       

                        if (applicationUserNew.IsPaidUser)//goes to dedicated DB
                        {
                            //dedicated database name. some format of email address is valid name for database name so need some validations.
                            
                            
                            //set on user session, not in any static variable.
                            HttpContext.Session.SetString("new_databasename", dedicatedDBName);

                            var fps = new FavoritePL();
                            fps.CreatedById = applicationUserNew.Id;
                            fps.FavoriteProgrammingLanguages = model.FavoriteProgrammingLanguages;
                            fps.FavoriteIDEs = model.FavoriteIDEs;
                           
                            ///create DB 1st 
                            var unitOfWork = new UnitOfWork<DedicatedDBContext>();
                            var favoritePLRepository = new GenericRepository<FavoritePL, DedicatedDBContext>(unitOfWork);
                            favoritePLRepository.Add(fps);
                            unitOfWork.Save();

                            
                        }
                        else  //goes to shared
                        {
                            var fps = new FavoritePL();
                            fps.CreatedById = applicationUserNew.Id;
                            fps.FavoriteProgrammingLanguages = model.FavoriteProgrammingLanguages;
                            fps.FavoriteIDEs = model.FavoriteIDEs;

                            var unitOfWork = new UnitOfWork<SharedDBContext>();
                            var favoritePLRepository = new GenericRepository<FavoritePL, SharedDBContext>(unitOfWork);
                            favoritePLRepository.Add(fps);
                            unitOfWork.Save();
                        }
                        return Ok(new { Msg = $"User was created successfully.", StatusCode = 0 });
                    }
                    else
                    {
                        return Problem($"Failed to create user {model.Email}. Error: {result}");
                    }


                }



            }
            catch (Exception ex)
            {
                applicationUserExists = await _userManager.FindByEmailAsync(model.Email);
                await _userManager.DeleteAsync(applicationUserExists);
                return Problem($"Server error occured.");
            }

        }

        
        [HttpPost]
        [Route("ChangeUserType")]
        //POST : /api/ApplicationUser/ChangeuserType
        public async Task<IActionResult> ChangeUserType(VMMoveUser model)
        {
             
            try
            {
                ApplicationUser applicationUser =  _userRepository.FirstOrDefault(u=>u.Id==model.UserId);

                ///The user was created as a free user but now change the user to dedicated user.
                

                HttpContext.Session.SetString("old_databasename", applicationUser.DedicatedDatabaseName);
                var dedicatedUnitOfWork = new UnitOfWork<DedicatedDBContext>();
                var sharedUnitOfWork = new UnitOfWork<SharedDBContext>();
                using (var dediFavsRepo = new GenericRepository<FavoritePL, DedicatedDBContext>(dedicatedUnitOfWork))
                using (var sharedFavsRepo = new GenericRepository<FavoritePL, SharedDBContext>(sharedUnitOfWork))
                {


                    //dedicated to shared
                    if (model.NewUserTypeId == 0)
                    {
                        applicationUser.IsPaidUser = false;
                        ///need some mechanism to copy large data. Also need to apply bulk copy when large data
                        var favs = dediFavsRepo.Where(f => f.CreatedById == model.UserId).ToList();
                        foreach (var fav in favs)
                        {
                            var newfavs = new FavoritePL();
                            CommonExtensions.CopyPropertiesTo(fav, newfavs);
                            newfavs.Id = Guid.NewGuid();
                            sharedFavsRepo.Add(newfavs);
                            fav.IsDeleted = true;
                            
                            dediFavsRepo.Update(fav);
                        }
                       
                    }

                    else if (model.NewUserTypeId == 1)//shared to dedicated
                    {
                        SubscriptionService subscriptionService = new SubscriptionService();
                        var dbexists = await subscriptionService.DedicatedDBExists(model.UserId);
                        if (!dbexists)
                        {
                            HttpContext.Session.SetString("new_databasename", applicationUser.DedicatedDatabaseName);
                        }
                        applicationUser.IsPaidUser = true;
                        ///need some mechanism to copy large data. Also need to apply bulk copy when large data
                        var favs = sharedFavsRepo.Where(f => f.CreatedById == model.UserId).ToList();
                        foreach (var fav in favs)
                        {
                            var newfavs = new FavoritePL();
                            CommonExtensions.CopyPropertiesTo(fav, newfavs);
                            fav.IsDeleted=true;
                            newfavs.Id = Guid.NewGuid();
                            dediFavsRepo.Add(newfavs);
                        }
                      
                    }
                    dedicatedUnitOfWork.Save();
                    sharedUnitOfWork.Save();
                    
                    _masterUnitOfWork.Save();
                    return Ok("User migration completed successfully.");
                }
            }
            catch (Exception ex)
            {
               
                return Problem($"Server error occured.");
            }

        }


        /// <summary>
        /// not tested
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(VMLogin model)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(model.UserName);

                if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
                {
                    var role = await _userManager.GetRolesAsync(user);

                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(new Claim[]
                        {
                        new Claim("UserName",user.Id.ToString()),
                        new Claim(ClaimTypes.Role,role.FirstOrDefault())
                        }),
                        Expires = DateTime.UtcNow.AddDays(7),
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.JwT_Secret)), SecurityAlgorithms.HmacSha256Signature)
                    };
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var securityToken = tokenHandler.CreateToken(tokenDescriptor);
                    var token = tokenHandler.WriteToken(securityToken);
                    var isAdmin = false;
                    isAdmin = role.FirstOrDefault(r => r.Contains("Admin")) != null;

                    return Ok(new CommonResponse { Msg = $"Login success.", StatusCode = CommonResponseEnum.Success, ResponseObject = token });

                }
                else
                    return BadRequest(new { message = "User Name or Password is incorrect." });
            }
            catch (Exception ex)
            {

                return BadRequest(new { message = "Unknown Server Error." });
            }

        }



    }
}
