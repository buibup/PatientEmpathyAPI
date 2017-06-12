using System.Collections.Generic;

namespace PatientEmpathy.Models
{
    public class Patient
    {
        public PatientInfo PatientInfo { get; set; }
        public List<PatientLab> PatientLab { get; set; }
    }
}