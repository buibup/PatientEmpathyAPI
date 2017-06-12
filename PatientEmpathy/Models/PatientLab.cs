using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PatientEmpathy.Models
{
    public class PatientLab
    {
        public string DateOfAuth { get; set; }
        public string DateOfRec { get; set; }
        public string LabNo { get; set; }
        public string Department { get; set; }
        public string HosCode { get; set; }
        public string HosDesc { get; set; }
        public string tsCode { get; set; }
        public string tsName { get; set; }
        public string tcCode { get; set; }
        public string tcname { get; set; }
        public string unit { get; set; }
        public string data { get; set; }
        public string flag { get; set; }
        public string low { get; set; }
        public string high { get; set; }
        public string Reference { get; set; }
    }
}