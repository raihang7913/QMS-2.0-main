using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QMS.Models
{
 
    public class CCRREQ
    {
        [Key]
        [Display(Name = "CCR Request ID")]
        public int Id { get; set; }

        [Display(Name = "Drawing No")]
        public string Drawing { get; set; }

        [Display(Name = "Part No")]
        public string PartNoDraw { get; set; }

        [Display(Name = "Part Name")]
        public string PartNameDraw { get; set; }

        [Display(Name = "List No")]
        public string ListNoDraw { get; set; }

        [Display(Name = "Old Issue")]
        public string OldIssueDraw { get; set; }

        [Display(Name = "New Issue")]
        public string NewIssueDraw { get; set; }

        [Display(Name = "Customer Name")]
        public string CustomerNameDraw { get; set; }

        [Display(Name = "Gap Analysis")]
        public string GapAnalysisDraw { get; set; }

        [Display(Name = "Date Review")]
        public DateTime DateReviewDraw { get; set; }

        [Display(Name = "CCR No")]
        public string CCRNo { get; set; }

        [Display(Name = "Date Recived")]
        public DateTime DateReceived { get; set; }

        [Display(Name = "Ref ECR/DR No")]
        public string RefECR { get; set; }

        [Display(Name = "Change Category Drawing")]
        public string ChangeCategoryDraw { get; set; }

        [Display(Name = "Standard or Specification")]
        public string StandardorSpec { get; set; }

        [Display(Name = "Specification Number")]
        public string NumberSpec { get; set; }

        [Display(Name = "Specification Old Issue")]
        public string OldIssueSpec { get; set; }

        [Display(Name = "Specification New Issue")]
        public string NewIssueSpec { get; set; }

        [Display(Name = "Specification Tittle")]
        public string TitleSpec { get; set; }

        [Display(Name = "Spesification Date Review")]
        public DateTime DateReviewSpec { get; set; }

        [Display(Name = "Spesification Gap Analysis")]
        public string GapAnalysisSpec { get; set; }

        [Display(Name = "Review Result")]
        public DateTime ReviewResult { get; set; }

        [Display(Name = "Document No / Tittle")]
        public string DocumentNoRR { get; set; }

        [Display(Name = "Revision Document")]
        public string NewRevOfDoc { get; set; }


        [Display(Name = "Reviewer Name")]
        public String ReviewName { get; set; }

        [Display(Name = "Employee ID")]
        public String ReviewID { get; set; }

        [Display(Name = "Department")]
        public String ReviewDepartment { get; set; }

        [Display(Name = "Position")]
        public String ReviewPosition { get; set; }

        [Display(Name = "Review Date")]
        public String ReviewDate { get; set; }

        [Display(Name = "Document No / Item Description")]
        public string VerificationDocNo { get; set; }

        [Display(Name = "New/Rev of Doc Or Change Of Item")]
        public string VerificationRevision { get; set; }

        [Display(Name = "PIC Verification")]
        public string VerificationPIC { get; set; }

        [Display(Name = "Verified By")]
        public string VerificationBy { get; set; }

        [Display(Name = "Verification Date")]
        public DateTime VerificationDate { get; set; }


        [Display(Name = "Configuration change Category")]
        public string ConfigurationChangeCategory { get; set; }

    }
}
