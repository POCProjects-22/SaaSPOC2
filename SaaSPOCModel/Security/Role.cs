using System;
using System.Collections.Generic;
using System.Text;

namespace SaaSPOCModel.Security
{
    public class Role
    {
        public const string Admin = "Admin";
        public const string User = "User";
        public const string AdminUser = Admin + "," + User;
    }

    public class Policy
    {
        public const string All = "All";
        //public const string User = "User";
        //public const string AdminUser = Admin + "," + User;
    }
}
