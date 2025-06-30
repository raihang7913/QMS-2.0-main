using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QMS.Models
{
    public class EngineeringPortal_CCR
    {
        [Key]
        [Display(Name = "User ID")]
        public int id { get; set; }

        [Display(Name = "CCR ID")]
        public string CCRID { get; set; }

        [Display(Name = "CCR NO")]
        public string CCRNo { get; set; }

        [Display(Name = "Date Recived")]
        public Nullable<System.DateTime> DateReceived { get; set; }

        [Display(Name = "Referensi ECR")]
        public string RefECR { get; set; }

        [Display(Name = "P1DrawingID")]
        public string P1DrawingID { get; set; }

        [Display(Name = "P2GapID")]
        public string P2GapID { get; set; }

        [Display(Name = "P3ReviewID ")]
        public string P3ReviewID { get; set; }

        [Display(Name = "P4Verification ")]
        public string P4Verification { get; set; }
        //public EngineeringPortal_CCRP1Drawing P1Drawing{ get; set; }
    }
}


        
