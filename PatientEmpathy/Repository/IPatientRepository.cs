using PatientEmpathy.Models;
using System;
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
        HttpResponseMessage GetImageLineByUserId(string userId, int width, int height);
        HttpResponseMessage GetPatientImagePostgres(string hn);
        HttpResponseMessage GetPatientImagePostgres(string hn, int width, int height);
        string GetLocationByLineBeacon(string beaconId);
        BeaconLocation GetBeaconLocation(string beaconId);
        List<MedDisch> GetMedDisch(string hn);
        List<FinDisch> GetFinDisc(string hn);
        List<Dischg> GetDischgs(string hn);
        Discharges GetDischarges(string hn);
        bool UpdateMedDisch();
        bool UpdateFinDisc();
        bool UpdateDischarge();
        Tuple<bool, List<PatientDischarges>> UpdateAllDisch();
        bool UpdatePromptPay(string hn, string message);
        Tuple<bool, string> UpdateRegisLoc(string hn);
        List<Location> GetLocation(string site, string type);
        bool UpdateNewRegis(string hn, string loc);
        bool UpdateSeeToDoctor();
        Tuple<bool, List<PatientBilled>> UpdatePatientBilled();
        Tuple<bool, List<PharCollect>> UpdatePharCollect();
        List<string> GetPatientAdmissionByTime(double minute);
        Tuple<int, List<PatientInfo>> GetPatientOPDCurrent(double minute);
        Tuple<int, List<PatientInfo>> GetPatientOPDCurrentRest(double minute);
        void RemoveAllIdleConnections();
    }
}