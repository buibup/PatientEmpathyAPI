using CRMWebApi.DA;
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

        public static List<Appointment> GetAppointment(string hn)
        {
            List<Appointment> apps = new List<Appointment>();
            var dtApp = InterSystemsDA.DTBindDataCommand(QueryString.GetAppointments(hn), conStr);
            foreach (DataRow row in dtApp.Rows)
            {
                Appointment app = new Appointment();
                app.AS_Date = Helper.ConvertDate(row["AS_Date"].ToString());
                app.AS_SessStartTime = Convert.ToDateTime(row["AS_SessStartTime"].ToString()).ToString("HH:mm");
                app.APPT_Status = row["APPT_Status"].ToString();
                app.CTLOC_Code = row["CTLOC_Code"].ToString();
                app.CTLOC_Desc = row["CTLOC_Desc"].ToString();
                app.CTPCP_Desc = row["CTPCP_Desc"].ToString();
                app.SER_Desc = row["SER_Desc"].ToString();
                apps.Add(app);
            }

            return apps;
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