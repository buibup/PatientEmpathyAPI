using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PatientEmpathy.Models
{
    public class AlertMsg
    {
        public string ALERTCAT_Code { get; set; }
        public string ALERTCAT_Desc { get; set; }
        public string ALM_Message { get; set; }
        public string ALM_Status { get; set; }
    }
}