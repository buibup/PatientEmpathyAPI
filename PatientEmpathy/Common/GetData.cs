using InterSystems.Data.CacheClient;
using PatientEmpathy.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using PatientEmpathy.DA;
using static System.String;

namespace PatientEmpathy.Common
{
    public class GetData
    {
        public static Episode GetEpisode(string hn, CacheConnection con)
        {
            Episode epi = new Episode();

            var dtEpiIn = InterSystemsDa.DtBindDataCommand(QueryString.GetEpisodeInquiry(hn), con);
            if (dtEpiIn.Rows.Count > 0)
            {
                epi = Helper.DataTableToEpisode(dtEpiIn, con);
            }
            else
            {
                epi.EpisodeInquiry = new List<EpisodeInquiry>();
            }

            return epi;
        }

        public static IEnumerable<Appointment> GetAppointment(string hn, CacheConnection con)
        {
            List<Appointment> apps = new List<Appointment>();
            var dtApp = InterSystemsDa.DtBindDataCommandWithValuesMultiple(QueryString.GetAppointments(hn), con, hn);
            var dtAppCur = InterSystemsDa.DtBindDataCommandWithValues(QueryString.GetAppointmentsCurrent(hn), con, hn);
            var dtAppPast = InterSystemsDa.DtBindDataCommandWithValuesMultiple(QueryString.GetAppointmentsPast(hn), con, hn);

            List<Appointment> lstApp = dtApp.ToList<Appointment>();
            List<Appointment> lstAppCur = dtAppCur.ToList<Appointment>();
            List<Appointment> lstAppPast = dtAppPast.ToList<Appointment>();

            var results = lstApp.Concat(lstAppCur).Concat(lstAppPast);

            foreach (var item in results)
            {
                var tupleDatetime = string.IsNullOrEmpty(item.AS_Date)
                    ? new Tuple<string, string>("","")
                    : new Tuple<string, string>(item.AS_Date, item.AS_SessStartTime);
                var datetimeString = Helper.ConvertToISO_8601(tupleDatetime.Item1, tupleDatetime.Item2);
                var datetime = DateTime.Parse(datetimeString);
            
                Appointment app = new Appointment()
                {
                    APPT_Status = item.APPT_Status,
                    AS_Date = datetime.ToString("dd/MM/yyyy"),
                    AS_SessStartTime = datetime.ToString("HH:mm"),
                    CTLOC_Code = item.CTLOC_Code,
                    CTLOC_Desc = item.CTLOC_Desc,
                    CTPCP_Desc = item.CTPCP_Desc,
                    PAADM_VisitStatus = item.PAADM_VisitStatus,
                    SER_Desc = item.SER_Desc
                };
                apps.Add(app);
            }

            try
            {
                apps = apps.OrderByDescending(a => string.IsNullOrEmpty(a.AS_Date) ? (DateTime?)null : DateTime.ParseExact(a.AS_Date, "dd/MM/yyyy", null))
                    .ThenByDescending(a => string.IsNullOrEmpty(a.AS_SessStartTime) ? (DateTime?)null : DateTime.ParseExact(a.AS_SessStartTime, "HH:mm", null)).ToList();
            }
            catch (Exception)
            {
                return apps;
            }
            

            return apps;
        }

        public static IEnumerable<AlertMsg> GetAlertMsg(string hn, CacheConnection con)
        {
            List<AlertMsg> alertMsgs = new List<AlertMsg>();

            var query = QueryString.GetAlertMsg(hn);

            try
            {
                using (var cmd = new CacheCommand(query.Item1, con))
                {
                    foreach (KeyValuePair<string, string> pair in query.Item2)
                    {
                        cmd.AddInputParameters(new { key = pair.Value });
                    }

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
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return alertMsgs;
        }

        public static PatientCategory GetPatientCategory(string hn, CacheConnection con)
        {
            var result = new PatientCategory();

            var query = QueryString.GetPatientCategory(hn);

            try
            {
                using (var cmd = new CacheCommand(query.Item1, con))
                {
                    foreach (var pair in query.Item2)
                    {
                        cmd.AddInputParameters(new { key = pair.Value });
                    }

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
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return result;
        }

        public static List<Allergy> GetAllergys(string hn, CacheConnection con)
        {
            var cmdString = QueryString.GetAllergy(hn);
            var dtAlg = InterSystemsDa.DtBindDataCommand(cmdString, con);

            var results = Helper.DataTableToAllergy(dtAlg, con);

            return results;
        }

        public static string GetAllergyCategory(string algName)
        {
            var cmdString = QueryString.GetAllergyCategory(algName);
            var result = InterSystemsDa.BindDataCommand(cmdString, Constants.Cache89);

            return result;
        }

        public static string GetAllergyCategory(string algName, CacheConnection con)
        {
            var cmdString = QueryString.GetAllergyCategory(algName);
            var result = InterSystemsDa.BindDataCommand(cmdString, con);

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

        //public static 

        internal static bool IsImage(string hn, CacheConnection con)
        {
            var dt = InterSystemsDa.DtBindDataCommand(QueryString.GetPatientImage(hn), con);

            var flag = Helper.CheckPatientImage(dt);

            return flag;
        }

        public static Tuple<bool, PatientImage> GetPatientImage(string hn, CacheConnection con)
        {
            try
            {
                var ptImg = new PatientImage();

                var dt = InterSystemsDa.DtBindDataCommand(QueryString.GetPatientImage(hn), con);

                var isImage = Helper.CheckPatientImage(dt);

                //ptImg.Image = isImage ? Helper.DataTableToBase64(dt, 500, 500) : "";

                return new Tuple<bool, PatientImage>(isImage, ptImg);
            }
            catch (Exception)
            {
                return new Tuple<bool, PatientImage>(false, new PatientImage());
            }
        }

        public static Tuple<PatientRegisLoc, List<LocRegis>> GetPatientRegisLoc(string hn, string loc = "")
        {

            var lstLocRegis = new List<LocRegis>();
            var locs = Empty;

            if (!hn.Contains("-"))
            {
                hn = Regex.Replace(hn, @"^(.{2})(.{2})(.{6})$", "$1-$2-$3");
            }

            var query = QueryString.GetPatientRegisLoc(hn);

            var con = ConnectCache.DBUtils.GetDBConnection();

            try
            {
                con.Open();
                using (var cmd = new CacheCommand(query.Item1, con))
                {
                    foreach (var pair in query.Item2)
                    {
                        cmd.AddInputParameters(new { key = pair.Value });
                    }
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var newRegis = string.Equals(reader["CTLOC_Code"].ToString(), loc,
                                StringComparison.CurrentCultureIgnoreCase);
                            locs += IsNullOrEmpty(locs)
                                ? reader["CTLOC_Code"].ToString()
                                : $"|{reader["CTLOC_Code"].ToString()}";
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
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                con.Close();
                con.Dispose();
            }


            var patientRegisLoc = new PatientRegisLoc()
            {
                Papmi_No = hn,
                RegisLoc = locs
            };

            return new Tuple<PatientRegisLoc, List<LocRegis>>(patientRegisLoc, lstLocRegis);
        }

        public static PatientRegisLoc GetRegisConsultLocation(string hn)
        {
            var regisLocList = GetPatientRegisLoc(hn).Item2;

            var loclist = new List<string>();
            var locs = "";

            if (!hn.Contains("-"))
            {
                hn = Regex.Replace(hn, @"^(.{2})(.{2})(.{6})$", "$1-$2-$3");
            }

            var query = QueryString.GetRegisConsultLocation(hn);

            var con = ConnectCache.DBUtils.GetDBConnection();

            try
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
                            var loc = string.Equals(reader["LocRegis"].ToString(), reader["LocConsult"].ToString(), StringComparison.CurrentCultureIgnoreCase)
                                ? new Tuple<bool, string, string>(true, reader["LocRegis"].ToString(), "")
                                : new Tuple<bool, string, string>(false, reader["LocRegis"].ToString(), reader["LocConsult"].ToString());
                            if (loc.Item1)
                            {
                                loclist.Add(loc.Item2);
                            }
                            else
                            {
                                loclist.Add(loc.Item2);
                                loclist.Add(loc.Item3);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }

            foreach(var regisLoc in regisLocList)
            {
                loclist.Add(regisLoc.CTLOC_Code);
            }

            loclist = loclist.Distinct().ToList();

            locs = string.Join(",", loclist);

            var locRegisConsult = new PatientRegisLoc()
            {
                Papmi_No = hn,
                RegisLoc = locs
            };

            return locRegisConsult;
        }

        public static DataTable ListMedDish(string listHn)
        {
            var dt = new DataTable();

            var query = QueryString.HnMedDischList(listHn);

            var con = ConnectCache.DBUtils.GetDBConnection();
            try
            {
                con.Open();
                using (var cmd = new CacheCommand(query.Item1, con))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        dt.Load(reader);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                con.Close();
                con.Dispose();
            }

            return dt;
        }

        public static DataTable DataAllDischsList(string listHn)
        {
            var dt = new DataTable();

            var query = QueryString.DataAllDischsList(listHn);

            var con = ConnectCache.DBUtils.GetDBConnection();
            try
            {
                con.Open();
                using (var cmd = new CacheCommand(query.Item1, con))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        dt.Load(reader);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                con.Close();
                con.Dispose();
            }

            return dt;
        }

        public static DataTable GetAllPatientBilled(string listHn)
        {
            var dt = new DataTable();
            var query = QueryString.GetAllPatientBilled(listHn);

            var con = ConnectCache.DBUtils.GetDBConnection();
            try
            {
                con.Open();
                using (var cmd = new CacheCommand(query.Item1, con))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        dt.Load(reader);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                con.Close();
                con.Dispose();
            }

            return dt;
        }

        public static DataTable PatientSeeToDoctor(string listHn)
        {
            var dt = new DataTable();

            var query = QueryString.PatientSeeToDoctor(listHn);

            var con = ConnectCache.DBUtils.GetDBConnection();
            try
            {
                con.Open();
                using (var cmd = new CacheCommand(query.Item1, con))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        dt.Load(reader);
                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                con.Close();
                con.Dispose();
            }

            return dt;
        }

        public static DataTable GetAllPatientPharCollect(string listHn)
        {
            var dt = new DataTable();
            var query = QueryString.GetPharCollect(listHn);

            using (var con = ConnectCache.DBUtils.GetDBConnection())
            {
                try
                {
                    con.Open();
                    using (var cmd = new CacheCommand(query.Item1, con))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            dt.Load(reader);
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
                finally
                {
                    con.Close();
                    con.Dispose();
                }
            }

            return dt;
        }

        public static string GetNoImageBase64()
        {
            var result = @"

                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

            ";

            return result.Trim();
        }
    }
}