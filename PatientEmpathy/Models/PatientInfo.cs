﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PatientEmpathy.Models
{
    public class PatientInfo
    {
        public string PAPMI_No { get; set; }
        public string Name { get; set; }
        public string PAPMI_DOB { get; set; }
        public int PAPER_AgeYr { get; set; }
        public int PAPER_AgeMth { get; set; }
        public int PAPER_AgeDay { get; set; }
        public string CTSEX_Desc { get; set; }
        public string PAPER_StName { get; set; }
        public string CTCIT_Desc { get; set; }
        public string Address { get; set; }
        public string PAPER_TelH { get; set; }
        public bool IsImage { get; set; }
        public Episode Episode { get; set; }
        public List<Appointment> Appointments { get; set; }
        public List<Allergy> Allergys { get; set; }
        public List<CRM> CRMs { get; set; }

    }
}