using PatientEmpathy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace PatientEmpathy.Repository
{
    public interface IPatientRepository
    {
        bool IsPatientDischarge(string epiNo);
        EpisodeInquiry GetEpisodeInquiry(string hn);
        PatientInfo GetPatientInfo(string hn);
        PatientInfo GetPatientInfoPOST(string hn);
        PatientInfo GetPatientInfoByEpiNo(string epiNo);
        PatientHN GetPatientHN(string epiNo);
        List<PatientLab> GetPatientLab(string epiNo);
        List<Patient> GetPatient(string date);
        List<Patient> GetPatient(string date, string locCode);
        List<Patient> GetPatientByHN(string hn);
        List<Location> GetLocation(string site);
        List<Room> GetRoom(string buId,string Ward);
        List<Ward> GetWard(string buId);
        HttpResponseMessage GetPatientImage(string hn);
        HttpResponseMessage GetPatientImage(string hn, int width, int height);
    }
}