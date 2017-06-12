using PatientEmpathy.Models;
using PatientEmpathy.Repository;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;

namespace PatientEmpathy.Controllers
{
    public class PatientController : ApiController
    {
        private IPatientRepository _patientRepository;

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
            return _patientRepository.GetPatientByHN(hn);
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
        public List<Room> GetRoom(string buId,string Ward)
        {
            _patientRepository = new PatientRepository();
            return _patientRepository.GetRoom(buId,Ward);
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
    }
}
