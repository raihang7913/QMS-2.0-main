using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QMS.Models
{
    [Table("DOCREQUSER")]
    public class DOCREQUSER
    {
        [Key]
        [Display(Name = "User ID")]
        public string Userid { get; set; }
        [Display(Name = "User Email")]
        public string Email { get; set; }
        [Display(Name = "User Name")]
        public string Name { get; set; }
        [Display(Name = "Request Access")]
        public string Requestaccess { get; set; }
        [Display(Name = "Final Approval Access")]
        public string Finalapprovalaccess { get; set; }
        [Display(Name = "ITC Approval Access")]
        public string ITCapprovalaccess { get; set; }
        [Display(Name = "AS9100 Approval Access")]
        public string AS9100approvalaccess { get; set; }
        [Display(Name = "NADCAP Approval Access")]
        public string NADCAPapprovalaccess { get; set; }
        [Display(Name = "Document Control Access")]
        public string Doccontrollaccess { get; set; }
        [Display(Name = "Admin Access")]
        public string Adminaccess { get; set; }
    }
}
