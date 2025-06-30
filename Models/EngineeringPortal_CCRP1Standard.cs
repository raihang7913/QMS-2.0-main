using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QMS.Models
{
    public class EngineeringPortal_CCRP1Standard
    {
        [Key]
        [Display(Name = "User ID")]
        public int id { get; set; }

        [Display(Name = "P1ID")]
        public string P1ID { get; set; }

        [Display(Name = "Standard or Specification")]
        public string StandardType { get; set; }

        [Display(Name = "Customer Name")]
        public string CustomerID { get; set; }

        [Display(Name = "Spec Number")]
        public string Number { get; set; }

        [Display(Name = "Old Issue Spec")]
        public string OldIssue { get; set; }

        [Display(Name = "New Issue Spec")]
        public string NewIssue { get; set; }

        [Display(Name = "Spec Title")]
        public string Title { get; set; }

        [Display(Name = "Date Review Spec")]
        public Nullable<System.DateTime> DateReview { get; set; }

        [Display(Name = "Gap Analisys Spec")]
        public string GapAnaly { get; set; }


    }
}
