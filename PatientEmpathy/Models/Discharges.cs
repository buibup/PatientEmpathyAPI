using System.Collections.Generic;

namespace PatientEmpathy.Models
{
    public class Discharges
    {
        public List<MedDisch> MedDischs { get; set; }
        public bool IsMedDischs { get; set; }
        public List<FinDisch> FinDischs { get; set; }
        public bool IsFinDischs { get; set; }
        public List<Dischg> Dischgs { get; set; }
        public bool IsDischgs { get; set; }
    }
}