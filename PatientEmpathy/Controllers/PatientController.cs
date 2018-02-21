using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PatientEmpathy.Models;
using PatientEmpathy.Repository;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;

namespace PatientEmpathy.Controllers
{
    public class PatientController : ApiController
    {
        private IPatientRepository _patientRepository;

        public PatientController()
        {
            _patientRepository = new PatientRepository();
        }

        [HttpGet]
        public bool IsPatientDischarge(string epiNo)
        {
            _patientRepository = new PatientRepository();
            return _patientRepository.IsPatientDischarge(epiNo);
        }

        [HttpGet]
        public EpisodeInquiry GetEpisodeInquiry(string hn)
        {
            _patientRepository = new PatientRepository();
            return _patientRepository.GetEpisodeInquiry(hn);
        }

        [HttpGet]
        public PatientInfo GetPatientInfo(string hn)
        {
            _patientRepository = new PatientRepository();
            return _patientRepository.GetPatientInfo(hn);
        }

        [HttpPost]
        public PatientInfo GetPatientInfoPOST(string hn)
        {
            _patientRepository = new PatientRepository();
            return _patientRepository.GetPatientInfo(hn);
        }

        [HttpGet]
        public List<PatientLab> GetPatientLab(string epiNo)
        {
            _patientRepository = new PatientRepository();
            return _patientRepository.GetPatientLab(epiNo);
        }

        [HttpGet]
        public List<Patient> GetPatient(string date)
        {
            _patientRepository = new PatientRepository();
            return _patientRepository.GetPatient(date);
        }

        public List<Patient> GetPatient(string date, string locCode)
        {
            _patientRepository = new PatientRepository();
            return _patientRepository.GetPatient(date, locCode);
        }

        public List<Patient> GetPatientByHN(string hn)
        {
            _patientRepository = new PatientRepository();
            return _patientRepository.GetPatientByHn(hn);
        }

        [HttpGet]
        public PatientInfo GetPatientInfoByEpiNo(string epiNo)
        {
            _patientRepository = new PatientRepository();
            return _patientRepository.GetPatientInfoByEpiNo(epiNo);
        }

        [HttpGet]
        public List<Location> GetLocation(string site)
        {
            _patientRepository = new PatientRepository();
            return _patientRepository.GetLocation(site);
        }

        [HttpGet]
        public List<Room> GetRoom(string buId, string Ward)
        {
            _patientRepository = new PatientRepository();
            return _patientRepository.GetRoom(buId, Ward);
        }

        public List<Ward> GetWard(string buId)
        {
            _patientRepository = new PatientRepository();
            return _patientRepository.GetWard(buId);
        }

        [HttpGet]
        public HttpResponseMessage GetPatientImage(string hn)
        {
            _patientRepository = new PatientRepository();
            return _patientRepository.GetPatientImage(hn);
        }

        [HttpGet]
        public HttpResponseMessage GetPatientImage(string hn, int width, int height)
        {
            _patientRepository = new PatientRepository();
            return _patientRepository.GetPatientImage(hn, width, height);
        }

        [HttpGet]
        public HttpResponseMessage GetImageLineByUserId(string userId, int width, int height)
        {
            _patientRepository = new PatientRepository();
            return _patientRepository.GetImageLineByUserId(userId, width, height);
        }

        [HttpGet]
        public HttpResponseMessage GetPatientImagePostgres(string hn)
        {
            return _patientRepository.GetPatientImagePostgres(hn);
        }

        [HttpGet]
        public HttpResponseMessage GetPatientImagePostgres(string hn, int width, int height)
        {
            return _patientRepository.GetPatientImagePostgres(hn, width, height);
        }

        [HttpPost]
        public string GetLocationByLineBeacon(string beaconId)
        {
            return _patientRepository.GetLocationByLineBeacon(beaconId);
        }

        [HttpPost]
        public BeaconLocation GetBeaconLocation(string beaconId)
        {
            return _patientRepository.GetBeaconLocation(beaconId);
        }

        [HttpPost]
        public Discharges GetDischarges(string hn)
        {
            return _patientRepository.GetDischarges(hn);
        }

        [HttpPost]
        public Tuple<bool, List<PatientDischarges>> UpdateAllDisch()
        {
            return _patientRepository.UpdateAllDisch();
        }

        public bool UpdatePromptPay(string hn, string message)
        {
            return _patientRepository.UpdatePromptPay(hn, message);
        }

        public Tuple<bool, string> UpdateRegisLoc(string hn)
        {
            return _patientRepository.UpdateRegisLoc(hn);
        }

        [HttpGet]
        public List<Location> GetLocation(string site, string type)
        {
            return _patientRepository.GetLocation(site, type);
        }

        public bool UpdateNewRegis(string hn, string loc)
        {
            return _patientRepository.UpdateNewRegis(hn, loc);
        }

        [HttpPost]
        public Tuple<bool, List<PatientBilled>> UpdatePatientBilled()
        {
            return _patientRepository.UpdatePatientBilled();
        }

        [HttpPost]
        public bool UpdateSeeToDoctor()
        {
            return _patientRepository.UpdateSeeToDoctor();
        }

        [HttpPost]
        public Tuple<bool, List<PharCollect>> UpdatePharCollect()
        {
            return _patientRepository.UpdatePharCollect();
        }

        [HttpPost]
        public List<string> GetPatientAdmissionByTime(double minute)
        {
            return _patientRepository.GetPatientAdmissionByTime(minute);
        }

        [HttpPost]
        public Tuple<int, List<PatientInfo>> GetPatientOPDCurrent(double minute)
        {
            return _patientRepository.GetPatientOPDCurrent(minute);
        }

        [HttpPost]
        public Tuple<int, List<PatientInfo>> GetPatientOPDCurrentRest(double minute)
        {
            return _patientRepository.GetPatientOPDCurrentRest(minute);
        }

        [HttpPost]
        public void RemoveAllIdleConnections()
        {
            _patientRepository.RemoveAllIdleConnections();
        }

        [HttpGet]
        public void SetLogAccess(string dept, string type, string value)
        {
            _patientRepository.SetLogAccess(dept, type, value);
        }

        [HttpPost]
        public void UploadImageHN(HttpRequestMessage req)
        {
            var data = req.Content.ReadAsStringAsync().Result;
            ImageHN img = JsonConvert.DeserializeObject<ImageHN>(data);
            _patientRepository.UploadImageHN(img.hn, img.extension, img.imageBase64);
        }

        [HttpPost]
        public void UploadImageMID(HttpRequestMessage req)
        {
            var data = req.Content.ReadAsStringAsync().Result;
            ImageMID img = JsonConvert.DeserializeObject<ImageMID>(data);
            _patientRepository.UploadImageMID(img.mid, img.extension, img.imageBase64);
        }
        
    }


    public class ImageHN
    {
        public string hn { get; set; }
        public string extension { get; set; }
        public string imageBase64 { get; set; }
    }

    public class ImageMID
    {
        public string mid { get; set; }
        public string extension { get; set; }
        public string imageBase64 { get; set; }
    }
}