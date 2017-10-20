using System.Collections.Generic;

namespace PatientEmpathy.Models
{
    public class PharCollect
    {
        public string PAPMI_No { get; set; }
        public List<EpiPharCollect> EpiPharCollectList { get; set; }
        public bool HnCollect { get; set; }
        public string LastCollectDateTime { get; set; }
    }

    public class EpiPharCollect
    {
        public string PAADM_ADMNo { get; set; }
        public List<PharPrescNo> PharPrescNoList { get; set; }
        public bool EpiCollect { get; set; }
        public int TotalPharPrescNo { get; set; }
    }

    public class PharPrescNo
    {
        public string OEORI_PrescNo { get; set; }
        public bool PharPrescNoCollect { get; set; }
        public string OEORI_UpdateDateTime { get; set; }
    }
}