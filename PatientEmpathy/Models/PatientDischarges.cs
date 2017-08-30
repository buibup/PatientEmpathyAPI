using System.Collections.Generic;

namespace PatientEmpathy.Models
{
    public class PatientDischarges
    {
        public string PAPMI_No { get; set; }
        public List<EPIAllDisch> EPIAllDisch { get; set; }
        public bool FlagMedDisch { get; set; }
        public bool FlagFinDisch { get; set; }
        public bool FlagDischg { get; set; }
        public bool FlagMedDischALL { get; set; }
    }

    public class EPIAllDisch
    {
        public string PAADM_ADMNo { get; set; }
        public bool IsMedDisch { get; set; }
        public bool IsFinDisch { get; set; }
        public bool IsDischg { get; set; }
    }
}