﻿using System.Collections.Generic;

namespace PatientEmpathy.Models
{
    public class Appointment
    {
        
        public string AS_Date { get; set; }
        public string AS_SessStartTime { get; set; }
        public string APPT_Status { get; set; }
        public string PAADM_VisitStatus { get; set; }
        public string CTLOC_Code { get; set; }
        public string CTLOC_Desc { get; set; }
        public string CTPCP_Desc { get; set; }
        public string SER_Desc { get; set; }
    }

    public class PatientSeeDoctor
    {
        public string PAPMI_No { get; set; }
        public List<LocSeeDoctor> ListLocSeeDoctor { get; set; }
    }

    public class LocSeeDoctor
    {
        public string CTLOC_Code { get; set; }
        public bool IsSeeDoctor { get; set; }
    }
}