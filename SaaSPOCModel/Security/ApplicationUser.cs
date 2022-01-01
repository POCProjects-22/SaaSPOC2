using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SaaSPOCModel.Security
{
    public class ApplicationUser : IdentityUser
    {

        [Column(TypeName = "nvarchar(150)")]
        public string FullName { get; set; }
        /// <summary>
        /// PasswordHash will replace this
        /// </summary>
        public string Password { get; set; }

        public string DedicatedDatabaseName { get; set; }


       
        public bool IsPaidUser { get; set; }  

     

        public bool IsDeleted { get; set; } 

        [Required]
        public DateTime CreatedTime { get; set; } = DateTime.Now;

        public DateTime? UpdatedTime { get; set; }

        [Column(TypeName = "varchar(80)")]
        public string CreatedByIP { get; set; }

        [Column(TypeName = "varchar(80)")]
        public string UpdatedByIP { get; set; }
        //no need created by

       
    }
}