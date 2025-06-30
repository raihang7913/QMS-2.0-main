using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QMS.Models
{
    public class EngineeringPortal_CCRP2GAP
    {
        public int id { get; set; }
        public string P2ID { get; set; }
        public string Document { get; set; }
        public string IdentifiedGap { get; set; }
        public string Closure { get; set; }
        public string PIC { get; set; }
        public Nullable<System.DateTime> DueDate { get; set; }
        public string Complated { get; set; }
    }
}
