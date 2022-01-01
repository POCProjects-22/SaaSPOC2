using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SaaSPOCAPI.Extensions
{
    public static class Utilities
    {

        public static string GetNewDBName()
        {
            var dbName = Guid.NewGuid().ToString();
            return dbName;
        }
        public static string GetDBNameFromUserEmail(string email)
        {
            var dbName = email.Split('@')[0];
            return dbName;
        }
        public static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
        
        public static string BasePath { get;   set; }
    }

    
}
