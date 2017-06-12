using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PatientEmpathy.Models
{
    public class EpisodeInquiry
    {
        public string PAADM_ADMNo { get; set; }
        public string PAADM_AdmDate { get; set; }
        public string PAADM_AdmTime { get; set; }
        public string CTLOC_Code { get; set; }
        public string CTLOC_Desc { get; set; }
        public string CTPCP_Code { get; set; }
        public string CTPCP_Desc { get; set; }
        public string PAADM_Type { get; set; }
        public string PAADM_VisitStatus { get; set; }
        public string WARD_Code { get; set; }
        public string WARD_Desc { get; set; }
        public string ROOM_Code { get; set; }
        public List<ICD10> ICD10 { get; set; }
        public List<ICD9> ICD9 { get; set; }
    }
}