namespace PatientEmpathy.Models
{
    public class Location
    {

        public string CTLOC_Code { get; set; }
        public string CTLOC_Desc { get; set; }
    }
    public class LocRegis
    {
        public string CTLOC_Code { get; set; }
        public bool NewRegis { get; set; }
    }
}