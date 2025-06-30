using System.ComponentModel.DataAnnotations;

namespace OJT_WebApp.Models
{
    public class EMPL
    {
        [Key] public string? NO_HRM { get; set; }
        public string? NM_KRY { get; set; }
        public string? NM_DEP { get; set; }
        public string? NM_JAB { get; set; }
        public string? NM_BAG { get; set; }
        public string? EE_Status { get; set; }
        public string? Cx_Leader { get; set; }
        public string? Cx_Supervisor { get; set; }
        public string? Cx_SC_Head { get; set; }
        public string? Cx_Manager { get; set; }
        public string? WinAccount { get; set; }
        public string? AL_EML { get; set; }
    }
}