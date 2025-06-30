using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;



namespace QMS.Models
{
    [Table("DOCREQ")]
    public class DOCREQ
    {
        [Key]
        [Display(Name = "Document Request ID")]
        public int Id { get; set; }
        [Display(Name = "Document Request Type")]
        //public string Docreason { get; set; }
        //[Display(Name = "Document Request Reason")]
        public string Reqtype { get; set; }
        [Display(Name = "Functional Group Owner")]
        public string Functgroup { get; set; }
        [Display(Name = "Others")]
        public string FunctgroupOthers { get; set; }
        [Display(Name = "Document Number")]
        [Required]
        public string Docnumber { get; set; }
        [Display(Name = "Link Document")]
        public string LinkDoc { get; set; }
        [Display(Name = "Associated Document Reference")]
        [Required]
        public string Associateddoc { get; set; }
        [Display(Name = "Document Type")]
        public string Doctype { get; set; }
        [Display(Name = "Superseeded Document if Any")]
        [Required]
        public string Superseededdoc { get; set; }
        [Display(Name = "Request Initiation Date")]
        public DateTime? Reqinitiatedate { get; set; }
        [Display(Name = "Priority Type Request")]
        public string Prioritydoc { get; set; }
        [Display(Name = "Audit or ETQ Number")]
        [Required]
        public string AuditNo { get; set; }
        [Display(Name = "Document Owner or Requestor")]
        public string Docowner { get; set; }
        [Display(Name = "Document Owner Approval Date")]
        public DateTime? Docownerdate { get; set; }
        [Display(Name = "Final Approval Name")]
        public string Finalapprovername { get; set; }
        [Display(Name = "Final Approver Date")]
        public DateTime? Finalapproverdate { get; set; }
        [Display(Name = "Final Approver Decision")]
        public string Finalapproverdecision { get; set; }
        [Display(Name = "Final Approver Note")]
        public string Finalapprovernote { get; set; }
        [Display(Name = "Subject Matter Expert Name")]
        public string SMEname { get; set; }
        [Display(Name = "Subject Matter Expert Date")]
        public DateTime? SMEdate { get; set; }
        [Display(Name = "Subject Matter Expert Approval Decision")]
        public string SMEapproverdecision { get; set; }
        [Display(Name = "Subject Matter Expert Approver Note ")]
        public string SMEapprovernote { get; set; }
        [Display(Name = "ITC Name")]
        public string ITCname { get; set; }
        [Display(Name = "ITC Approval Date")]
        public DateTime? ITCdate { get; set; }
        [Display(Name = "ITC Approval Decision")]
        public string ITCapproverdecision { get; set; }
        [Display(Name = "ITC Approval Note")]
        public string ITCapprovernote { get; set; }
        [Display(Name = "QMS AS Approver Note")]
        public string QMSASname { get; set; }
        [Display(Name = "QMS AS Approval Date")]
        public DateTime? QMSASdate { get; set; }
        [Display(Name = "QMS AS Approval Decision")]
        public string QMSASapproverdecision { get; set; }
        [Display(Name = "QMS AS Approver Note")]
        public string QMSASapprovernote { get; set; }
        [Display(Name = "QMS NADCAP Approver Name")]
        public string QMSNADCAPname { get; set; }
        [Display(Name = "QMS NADCAP Approval Date")]
        public DateTime? QMSNADCAPdate { get; set; }
        [Display(Name = "QMS NADCAP Approver Desicion")]
        public string QMSNADCAPapproverdecision { get; set; }
        [Display(Name = "QMS NADCAP Approver Note")]
        public string QMSNADCAPapprovernote { get; set; }
        [Display(Name = "Document Controller Approver Name")]
        public string Doccontrollername { get; set; }
        [Display(Name = "Document Controller Approval Date")]
        public DateTime? Doccontrollerdate { get; set; }
        [Display(Name = "Document Controller Approver Desicion")]
        public string Doccontrolleapproverdecision { get; set; }
        [Display(Name = "Document Controller Approver Note")]
        public string Doccontrolleapprovernote { get; set; }
        [Display(Name = "Technical Data")]
        public string TechnicalData { get; set; }
        [Display(Name = "Smartsolve Upload")]
        public string SmartsolveUpload { get; set; }
        [Display(Name = "List Document 1")]
        public string Listdoc1 { get; set; }
        [Display(Name = "List Document Affected 1")]
        public string Listdocaffected1 { get; set; }
        [Display(Name = "List Document 2")]
        public string Listdoc2 { get; set; }
        [Display(Name = "List Document Affected 2")]
        public string Listdocaffected2 { get; set; }
        [Display(Name = "List Document 3")]
        public string Listdoc3 { get; set; }
        [Display(Name = "List Document Affected 3")]
        public string Listdocaffected3 { get; set; }
        [Display(Name = "List Document 4")]
        public string Listdoc4 { get; set; }
        [Display(Name = "List Document Affected 4")]
        public string Listdocaffected4 { get; set; }
        [Display(Name = "List Document 5")]
        public string Listdoc5 { get; set; }
        [Display(Name = "List Document Affected 5")]
        public string Listdocaffected5 { get; set; }
        [Display(Name = "List Document 6")]
        public string Listdoc6 { get; set; }
        [Display(Name = "List Document Affected 6")]
        public string Listdocaffected6 { get; set; }
        [Display(Name = "List Document 7")]
        public string Listdoc7 { get; set; }
        [Display(Name = "List Document Affected 7")]
        public string Listdocaffected7 { get; set; }
        [Display(Name = "List Document 8")]
        public string Listdoc8 { get; set; }
        [Display(Name = "List Document Affected 8")]
        public string Listdocaffected8 { get; set; }
        [Display(Name = "List Document 9")]
        public string Listdoc9 { get; set; }
        [Display(Name = "List Document Affected 9")]
        public string Listdocaffected9 { get; set; }
        [Display(Name = "List Document 10")]
        public string Listdoc10 { get; set; }
        [Display(Name = "List Document Affected 10")]
        public string Listdocaffected10 { get; set; }
        [Display(Name = "Closing Date")]
        public DateTime? Closedate { get; set; }

        [Display(Name = "Closing Status")]
        public string Closestatus { get; set; }

        [Display(Name = "Document Request Reason")]
        [Required]
        public string documentReason { get; set; }

    }
}
