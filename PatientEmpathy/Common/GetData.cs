using CRMWebApi.DA;
using InterSystems.Data.CacheClient;
using Newtonsoft.Json;
using PatientEmpathy.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web;

namespace PatientEmpathy.Common
{
    public class GetData
    {
        private static string conStr = Constants.Cache89;
        public static Episode GetEpisode(string hn)
        {
            Episode epi = new Episode();

            var dtEpiIn = InterSystemsDA.DTBindDataCommand(QueryString.GetEpisodeInquiry(hn), conStr);
            if(dtEpiIn.Rows.Count > 0)
            {
                epi = Helper.DataTableToEpisode(dtEpiIn);
            }else
            {
                epi.EpisodeInquiry = new List<EpisodeInquiry>();
            }

            return epi;
        }

        public static IEnumerable<Appointment> GetAppointment(string hn)
        {
            List<Appointment> apps = new List<Appointment>();
            var dtApp = InterSystemsDA.DTBindDataCommandWithValuesMultiple(QueryString.GetAppointments(hn), conStr, hn);
            var dtAppCur = InterSystemsDA.DTBindDataCommandWithValues(QueryString.GetAppointmentsCurrent(hn), conStr, hn);
            var dtAppPast = InterSystemsDA.DTBindDataCommandWithValuesMultiple(QueryString.GetAppointmentsPast(hn), conStr, hn);

            List<Appointment> lstApp = dtApp.ToList<Appointment>();
            List<Appointment> lstAppCur = dtAppCur.ToList<Appointment>();
            List<Appointment> lstAppPast = dtAppPast.ToList<Appointment>();

            var results = lstApp.Concat(lstAppCur).Concat(lstAppPast);

            foreach(var item in results)
            {
                Appointment app = new Appointment()
                {
                    APPT_Status = item.APPT_Status,
                    AS_Date = Helper.ConvertDate(item.AS_Date),
                    AS_SessStartTime = Convert.ToDateTime(item.AS_SessStartTime).ToString("HH:mm"),
                    CTLOC_Code = item.CTLOC_Code,
                    CTLOC_Desc = item.CTLOC_Desc,
                    CTPCP_Desc = item.CTPCP_Desc,
                    PAADM_VisitStatus = item.PAADM_VisitStatus,
                    SER_Desc = item.SER_Desc
                };
                apps.Add(app);
            }

            apps = apps.OrderByDescending(a => DateTime.ParseExact(a.AS_Date, "dd/MM/yyyy", null)).ThenByDescending(a => DateTime.ParseExact(a.AS_SessStartTime, "HH:mm", null)).ToList();

            return apps;
        }

        public static IEnumerable<AlertMsg> GetAlertMsg(string hn)
        {
            List<AlertMsg> alertMsgs = new List<AlertMsg>();

            var query = QueryString.GetAlertMsg(hn);

            using (var con = new CacheConnection(conStr))
            {
                con.Open();
                using (var cmd = new CacheCommand(query.Item1, con))
                {
                    foreach (KeyValuePair<string, string> pair in query.Item2)
                    {
                        var key = pair.Key;
                        cmd.AddInputParameters(new { key = pair.Value });
                    }
                    try
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                AlertMsg result = new AlertMsg()
                                {
                                    ALERTCAT_Code = reader["ALERTCAT_Code"].ToString(),
                                    ALERTCAT_Desc = reader["ALERTCAT_Desc"].ToString(),
                                    ALM_Message = reader["ALM_Message"].ToString(),
                                    ALM_Status = reader["ALM_Status"].ToString()
                                };
                                alertMsgs.Add(result);
                            }
                        }
                    }
                    catch (Exception)
                    {

                        throw;
                    }

                }
            }

            return alertMsgs;
        }

        public static PatientCategory GetPatientCategory(string hn)
        {
            PatientCategory result = new PatientCategory();

            var query = QueryString.GetPatientCategory(hn);

            using(var con = new CacheConnection(conStr))
            {
                con.Open();
                using(var cmd = new CacheCommand(query.Item1, con))
                {
                    foreach(KeyValuePair<string,string> pair in query.Item2)
                    {
                        var key = pair.Key;
                        cmd.AddInputParameters(new { key = pair.Value });
                    }
                    try
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                result = new PatientCategory()
                                {
                                    PCAT_CODE = reader["PCAT_CODE"].ToString(),
                                    PCAT_Desc = reader["PCAT_Desc"].ToString()
                                };

                                return result;
                            }
                        }
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                    
                }
            }

            return result;
        }

        public static List<Allergy> GetAllergys(string hn)
        {
            List<Allergy> results = new List<Allergy>();

            var cmdString = QueryString.GetAllergy(hn);
            var dtAlg = InterSystemsDA.DTBindDataCommand(cmdString, Constants.Cache89);

            results = Helper.DataTableToAllergy(dtAlg);

            return results;
        }

        public static string GetAllergyCategory(string algName)
        {
            string result = string.Empty;

            var cmdString = QueryString.GetAllergyCategory(algName);
            result = InterSystemsDA.BindDataCommand(cmdString, Constants.Cache89);

            return result;
        }

        public static string GetHN(string epiNo)
        {
            string hn = InterSystemsDA.BindDataCommand(QueryString.GetPatientHN(epiNo), Constants.Cache89);

            return hn;
        }

        public static string GetLastEpisode(string hn)
        {
            string epiNo = InterSystemsDA.BindDataCommand(QueryString.GetLastEpisode(hn), Constants.Cache89);

            return epiNo;
        }

        internal static bool IsImage(string hn)
        {
            bool flag = false;
            var dt = InterSystemsDA.DTBindDataCommand(QueryString.GetPatientImage(hn), Constants.Cache89);

            flag = Helper.CheckPatientImage(dt);

            return flag;
        }
    }
}