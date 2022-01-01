using GenericRepository.IdentityRepository;
using SaaSPOCModel;
using SaaSPOCModel.Security;
using SaaSPOCService.Contexts;
using System;
using System.Linq;
using System.Net;
using System.Security.Claims;

namespace Microsoft.AspNetCore.Mvc
{
    public static class ControllerBaseExtensions
    {
        public static string GetLoggedinUserId(this ControllerBase controller)
        {
            try
            {
                var claimsIdentity = controller.User.Identity as ClaimsIdentity;
                var userid = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "UserName").Value;
                return userid;
            }
            catch (Exception ex)
            {
                // throw new Exception(ex.Message);
                return "";
            }
        }
        public static ApplicationUser GetLoggedinUser(this ControllerBase controller)
        {
            try
            {
                IdentityUnitOfWork<MasterDBContext> _unitOfWork = new IdentityUnitOfWork<MasterDBContext>();
                IdentityRepository<ApplicationUser, MasterDBContext> _userRepository;
                _userRepository = new IdentityRepository<ApplicationUser, MasterDBContext>(_unitOfWork);


                var claimsIdentity = controller.User.Identity as ClaimsIdentity;
                var userid = claimsIdentity.Claims.FirstOrDefault(c => c.Type == "UserName").Value;
                return _userRepository.FirstOrDefault(u => u.Id == userid);
            }
            catch (Exception ex)
            {
                //throw new Exception(ex.Message);
                throw;
            }
        }

        public static void AddCreateInfo(this ControllerBase controller, BaseEntity baseEntity, bool defaultParent=false)
        {
            if (!defaultParent)
            {
                var loggedinuserId = GetLoggedinUserId(controller);
                baseEntity.CreatedById = string.IsNullOrEmpty(loggedinuserId) ? null : loggedinuserId;
            }
            else
            {

                using (MasterDBContext ctx = new MasterDBContext())
                {
                    var admin1 = ctx.ApplicationUser.OrderBy(u => u.CreatedTime).FirstOrDefault().Id;
                    baseEntity.CreatedById = admin1;
                }

               
            }
            baseEntity.CreatedByIP = GetIP(controller);
            baseEntity.CreatedTime = DateTime.Now;
            baseEntity.IsDeleted = false;            
        }
        public static string GetIP(this ControllerBase controller)
        {
            try
            {
                IPAddress remoteIpAddress = controller.Request.HttpContext.Connection.RemoteIpAddress;
              
                return remoteIpAddress.ToString();
                //string result = "";
                //if (remoteIpAddress != null)
                //{
                //    // If we got an IPV6 address, then we need to ask the network for the IPV4 address 
                //    // This usually only happens when the browser is on the same machine as the server.
                //    // if (remoteIpAddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                //    {
                //        remoteIpAddress = System.Net.Dns.GetHostEntry(remoteIpAddress).AddressList.First(x => x.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
                //    }
                //    result = remoteIpAddress.ToString();
                //}
                //return result;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }

        }


    }
}
