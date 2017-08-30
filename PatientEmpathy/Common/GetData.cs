using InterSystems.Data.CacheClient;
using PatientEmpathy.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using PatientEmpathy.DA;
using static System.String;

namespace PatientEmpathy.Common
{
    public class GetData
    {
        private static readonly string ConStr = Constants.Cache89;
        public static Episode GetEpisode(string hn)
        {
            Episode epi = new Episode();

            var dtEpiIn = InterSystemsDa.DtBindDataCommand(QueryString.GetEpisodeInquiry(hn), ConStr);
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
            var dtApp = InterSystemsDa.DtBindDataCommandWithValuesMultiple(QueryString.GetAppointments(hn), ConStr, hn);
            var dtAppCur = InterSystemsDa.DtBindDataCommandWithValues(QueryString.GetAppointmentsCurrent(hn), ConStr, hn);
            var dtAppPast = InterSystemsDa.DtBindDataCommandWithValuesMultiple(QueryString.GetAppointmentsPast(hn), ConStr, hn);

            List<Appointment> lstApp = dtApp.ToList<Appointment>();
            List<Appointment> lstAppCur = dtAppCur.ToList<Appointment>();
            List<Appointment> lstAppPast = dtAppPast.ToList<Appointment>();

            var results = lstApp.Concat(lstAppCur).Concat(lstAppPast);

            foreach (var item in results)
            {
                Appointment app = new Appointment()
                {
                    APPT_Status = item.APPT_Status,
                    AS_Date = item.AS_Date,//Helper.ConvertDate(item.AS_Date),
                    AS_SessStartTime = Convert.ToDateTime(item.AS_SessStartTime).ToString("HH:mm"),
                    CTLOC_Code = item.CTLOC_Code,
                    CTLOC_Desc = item.CTLOC_Desc,
                    CTPCP_Desc = item.CTPCP_Desc,
                    PAADM_VisitStatus = item.PAADM_VisitStatus,
                    SER_Desc = item.SER_Desc
                };
                apps.Add(app);
            }

            apps = apps.OrderByDescending(a => string.IsNullOrEmpty(a.AS_Date) ? (DateTime?)null : DateTime.ParseExact(a.AS_Date, "dd/MM/yyyy", null))
                        .ThenByDescending(a => string.IsNullOrEmpty(a.AS_SessStartTime) ? (DateTime?)null : DateTime.ParseExact(a.AS_SessStartTime, "HH:mm", null)).ToList();

            return apps;
        }

        public static IEnumerable<AlertMsg> GetAlertMsg(string hn)
        {
            List<AlertMsg> alertMsgs = new List<AlertMsg>();

            var query = QueryString.GetAlertMsg(hn);

            using (var con = new CacheConnection(ConStr))
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
            var result = new PatientCategory();

            var query = QueryString.GetPatientCategory(hn);

            using(var con = new CacheConnection(ConStr))
            {
                con.Open();
                using(var cmd = new CacheCommand(query.Item1, con))
                {
                    foreach(var pair in query.Item2)
                    {
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
            var cmdString = QueryString.GetAllergy(hn);
            var dtAlg = InterSystemsDa.DtBindDataCommand(cmdString, Constants.Cache89);

            var results = Helper.DataTableToAllergy(dtAlg);

            return results;
        }

        public static string GetAllergyCategory(string algName)
        {
            var cmdString = QueryString.GetAllergyCategory(algName);
            var result = InterSystemsDa.BindDataCommand(cmdString, Constants.Cache89);

            return result;
        }

        public static string GetHn(string epiNo)
        {
            var hn = InterSystemsDa.BindDataCommand(QueryString.GetPatientHn(epiNo), Constants.Cache89);

            return hn;
        }

        public static string GetLastEpisode(string hn)
        {
            var epiNo = InterSystemsDa.BindDataCommand(QueryString.GetLastEpisode(hn), Constants.Cache89);

            return epiNo;
        }

        internal static bool IsImage(string hn)
        {
            var dt = InterSystemsDa.DtBindDataCommand(QueryString.GetPatientImage(hn), Constants.Cache89);

            var flag = Helper.CheckPatientImage(dt);

            return flag;
        }

        public static Tuple<PatientRegisLoc, List<LocRegis>> GetPatientRegisLoc(string hn, string loc="")
        {

            var lstLocRegis = new List<LocRegis>();
            var locs = Empty;

            if (!hn.Contains("-"))
            {
                hn = Regex.Replace(hn, @"^(.{2})(.{2})(.{6})$", "$1-$2-$3");
            }

            var query = QueryString.GetPatientRegisLoc(hn);

            using(var con = new CacheConnection(Constants.Cache89))
            {
                con.Open();
                using(var cmd = new CacheCommand(query.Item1, con))
                {
                    foreach(var pair in query.Item2)
                    {
                        cmd.AddInputParameters(new { key = pair.Value });
                    }
                    using(var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var newRegis = string.Equals(reader["CTLOC_Code"].ToString(), loc, StringComparison.CurrentCultureIgnoreCase);
                            locs += IsNullOrEmpty(locs) ? reader["CTLOC_Code"].ToString() : $"|{reader["CTLOC_Code"].ToString()}";
                            var locRegis = new LocRegis()
                            {
                                CTLOC_Code = reader["CTLOC_Code"].ToString(),
                                NewRegis = newRegis
                            };
                            lstLocRegis.Add(locRegis);
                        }
                    }
                }
            }

            var patientRegisLoc = new PatientRegisLoc()
            {
                Papmi_No = hn,
                RegisLoc = locs
            };

            return new Tuple<PatientRegisLoc, List<LocRegis>>(patientRegisLoc, lstLocRegis);
        }

        public static DataTable ListMedDish(string listHn)
        {
            var dt = new DataTable();

            var query = QueryString.HnMedDischList(listHn);

            using (var con = new CacheConnection(Constants.Cache89))
            {
                con.Open();
                using(var cmd = new CacheCommand(query.Item1, con))
                {
                    try
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            dt.Load(reader);
                        }
                    }
                    catch (Exception)
                    {

                        throw;
                    }

                }
            }

            return dt;
        }

        public static DataTable DataAllDischsList(string listHn)
        {
            var dt = new DataTable();

            var query = QueryString.DataAllDischsList(listHn);

            using (var con = new CacheConnection(Constants.Cache89))
            {
                con.Open();
                using (var cmd = new CacheCommand(query.Item1, con))
                {
                    try
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            dt.Load(reader);
                        }
                    }
                    catch (Exception)
                    {

                        throw;
                    }

                }
            }

            return dt;
        }

        public static DataTable GetAllPatientBilled(string listHn)
        {
            var dt = new DataTable();
            var query = QueryString.GetAllPatientBilled(listHn);

            using (var con = new CacheConnection(Constants.Cache89))
            {
                con.Open();
                using (var cmd = new CacheCommand(query.Item1, con))
                {
                    try
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            dt.Load(reader);
                        }
                    }
                    catch (Exception)
                    {

                        throw;
                    }

                }
            }

            return dt;
        }

        public static DataTable PatientSeeToDoctor(string listHn)
        {
            var dt = new DataTable();

            var query = QueryString.PatientSeeToDoctor(listHn);

            using (var con = new CacheConnection(Constants.Cache89))
            {
                con.Open();
                using (var cmd = new CacheCommand(query.Item1, con))
                {
                    try
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            dt.Load(reader);
                        }
                    }
                    catch (Exception)
                    {

                        throw;
                    }

                }
            }

            return dt;
        }

        public static DataTable GetAllPatientPharCollect(string listHn)
        {
            var dt = new DataTable();
            var query = QueryString.GetPharCollect(listHn);

            using (var con = new CacheConnection(Constants.Cache89))
            {
                con.Open();
                using (var cmd = new CacheCommand(query.Item1, con))
                {
                    try
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            dt.Load(reader);
                        }
                    }
                    catch (Exception)
                    {

                        throw;
                    }

                }
            }

            return dt;
        }
    }
}