using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QMS.Models
{
    [Table("EngineeringPortal_CCRP1Drawing")]
    public class EngineeringPortal_CCRP1Drawing
    {
        [Key]
        [Display(Name = "ID")]
        public int id { get; set; }

        [Display(Name = "P1 D")]
        public string P1ID { get; set; } = "";

        [Display(Name = "Drawing Rev.")]
        public string Drawing { get; set; } = "";

        [Display(Name = "Part Number")]
        public string PartNumber { get; set; } = "";

        [Display(Name = "Part Name")]
        public string PartName { get; set; } = "";

        [Display(Name = "List Number")]
        public string listNo { get; set; } = "";

        [Display(Name = "Old Issue Number")]
        public string OldIssue { get; set; } = "";

        [Display(Name = "New Issue")]
        public string NewIssue { get; set; } = "";

        [Display(Name = "Customer Name")]
        public string CustomerName { get; set; } = "";


        [Display(Name = "Gap Analysis")]
        public string GapAnal { get; set; } = "";

        [Display(Name = "Review Date")]
        public Nullable<System.DateTime> DateReview { get; set; }

        [Display(Name = "Changes Category")]
        public string CategoryChange { get; set; } = "";

    }
}
