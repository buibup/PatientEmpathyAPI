using System.Collections.Generic;

namespace PatientEmpathy.Models
{
    public class PatientBilled
    {
        public string PAPMI_No { get; set; }
        public List<EpiBilled> ListEpiBilled { get; set; }
        public bool FlagBilled { get; set; }
    }
    public class EpiBilled
    {
        public string PAADM_ADMNO { get; set; }
        public List<Billed> ListBilled { get; set; }
        public bool IsBilled { get; set; }
    }
    public class Billed
    {
        public string ARPBL_BillNo { get; set; }
        public string ARPBL_DatePrinted { get; set; }
        public string ARPBL_TimePrinted { get; set; }
    }
}