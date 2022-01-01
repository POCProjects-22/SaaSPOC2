using SaaSPOCModel.Security;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SaaSPOCModel
{
    public class BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
        public bool IsDeleted { get; set; }

        /// Not possible to set ForeignKey because user table is in other database
        ////ref to ApplicationUser
        //[ForeignKey("ApplicationUser")]

        [Required()]
        public string CreatedById { get; set; }
        public virtual ApplicationUser? CreatedBy { get; set; }
        //ref to ApplicationUser

     
        public DateTime CreatedTime { get; set; }
        /// <summary>
        /// Not possible to set ForeignKey because user table is in other database
        /// </summary>
        //[ForeignKey("ApplicationUser")]
        public string UpdatedById { get; set; }
        public virtual ApplicationUser UpdatedBy { get; set; }
        public DateTime? UpdatedTime { get; set; }

        [Column(TypeName = "varchar(80)")]
        public string CreatedByIP { get; set; }

        [Column(TypeName = "varchar(80)")]
        public string UpdatedByIP { get; set; }
    }
}
