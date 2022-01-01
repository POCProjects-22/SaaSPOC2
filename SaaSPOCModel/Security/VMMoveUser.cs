using System;
using System.Collections.Generic;
using System.Text;

namespace SaaSPOCModel.Security
{
   public class VMMoveUser
    {
        public string UserId { get; set; }
        /// <summary>
        /// enum or table: 
        /// un paind = 0
        /// paid > 0
        /// </summary>
        public int NewUserTypeId { get; set; }
    }
}
