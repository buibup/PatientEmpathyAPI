using PatientEmpathy.Models;
using System.Collections.Generic;
using System.Net.Http;

namespace PatientEmpathy.Repository
{
    public interface IPatientRepository
    {
        bool IsPatientDischarge(string epiNo);
        EpisodeInquiry GetEpisodeInquiry(string hn);
        PatientInfo GetPatientInfo(string hn);
        PatientInfo GetPatientInfoPost(string hn);
        PatientInfo GetPatientInfoByEpiNo(string epiNo);
        PatientHN GetPatientHn(string epiNo);
        List<PatientLab> GetPatientLab(string epiNo);
        List<Patient> GetPatient(string date);
        List<Patient> GetPatient(string date, string locCode);
        List<Patient> GetPatientByHn(string hn);
        List<Location> GetLocation(string site);
        List<Room> GetRoom(string buId,string ward);
        List<Ward> GetWard(string buId);
        HttpResponseMessage GetPatientImage(string hn);
        HttpResponseMessage GetPatientImage(string hn, int width, int height);
        string GetLocationByLineBeacon(string beaconId);
        BeaconLocation GetBeaconLocation(string beaconId);
        List<MedDisch> GetMedDisch(string hn);
        List<FinDisch> GetFinDisc(string hn);
        List<Dischg> GetDischgs(string hn);
        Discharges GetDischarges(string hn);
        bool UpdateMedDisch();
        bool UpdateFinDisc();
        bool UpdateDischarge();
        bool UpdateAllDisch();
        bool UpdatePromptPay(string hn, string message);
        bool UpdateRegisLoc(string hn);
        List<Location> GetLocation(string site, string type);
        bool UpdateNewRegis(string hn, string loc);
        bool UpdateSeeToDoctor();
        bool UpdatePatientBilled();
        bool UpdatePharCollect();
    }
}