using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QMS.Models
{
    public class EngineeringPortal_CCRP3Review
    {
        public int id { get; set; }
        public string P3ID { get; set; }
        public Nullable<System.DateTime> ReviewDate { get; set; }
        public string Esign { get; set; }
        public string EmployeeID { get; set; }


    }
}
