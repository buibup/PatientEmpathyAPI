using System;
using InterSystems.Data.CacheClient;
using PatientEmpathy.Common;
using PatientEmpathy.Models;
using System.Collections.Generic;
using System.Web;
using System.Data;
using System.Text.RegularExpressions;
using Npgsql;
using System.Net.Http;
using System.Linq;
using PatientEmpathy.DA;
using Newtonsoft.Json;
using static System.String;

namespace PatientEmpathy.Repository
{
    public class PatientRepository : IPatientRepository
    {
        private readonly string _cache89ConString = Constants.Cache89;
        public EpisodeInquiry GetEpisodeInquiry(string hn)
        {
            EpisodeInquiry epiInq = null;

            using (var con = new CacheConnection(_cache89ConString))
            {
                con.Open();
                using (var cmd = new CacheCommand(QueryString.GetEpisodeInquiry(hn), con))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            epiInq = new EpisodeInquiry()
                            {
                                PAADM_ADMNo = reader["PAADM_ADMNo"].ToString(),
                                PAADM_AdmDate = Helper.ConvertDate(reader["PAADM_AdmDate"].ToString()),
                                PAADM_AdmTime = Convert.ToDateTime(reader["PAADM_AdmTime"].ToString()).ToString("HH:mm"),
                                CTLOC_Code = reader["CTLOC_Code"].ToString(),
                                CTLOC_Desc = reader["CTLOC_Desc"].ToString(),
                                CTPCP_Code = reader["CTPCP_Code"].ToString(),
                                CTPCP_Desc = reader["CTPCP_Desc"].ToString(),
                                PAADM_Type = reader["PAADM_Type"].ToString(),
                                PAADM_VisitStatus = reader["PAADM_VisitStatus"].ToString(),
                                WARD_Code = reader["WARD_Code"].ToString(),
                                WARD_Desc = reader["WARD_Desc"].ToString(),
                                ROOM_Code = reader["ROOM_Code"].ToString()
                            };
                        }
                    }
                }
            }

            return epiInq;
        }

        public List<Location> GetLocation(string site)
        {
            var locations = new List<Location>();
            var query = QueryString.GetLocation(site);

            using (var con = new CacheConnection(_cache89ConString))
            {
                con.Open();
                using (var cmd = new CacheCommand(query.Item1, con))
                {
                    foreach (var pair in query.Item2)
                    {
                        var key = pair.Key;
                        cmd.AddInputParameters(new { key = pair.Value });
                    }
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var loc = new Location()
                            {
                                CTLOC_Code = reader["CTLOC_Code"].ToString(),
                                CTLOC_Desc = reader["CTLOC_Desc"].ToString()
                            };
                            locations.Add(loc);
                        }
                    }
                }
            }

            return locations;
        }

        public List<Location> GetLocation(string site, string type)
        {
            var locations = new List<Location>();
            var query = QueryString.GetLocation(site, type);

            using (var con = new CacheConnection(_cache89ConString))
            {
                con.Open();
                using (var cmd = new CacheCommand(query.Item1, con))
                {
                    foreach (var pair in query.Item2)
                    {
                        var key = pair.Key;
                        cmd.AddInputParameters(new { key = pair.Value });
                    }
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var loc = new Location()
                            {
                                CTLOC_Code = reader["CTLOC_Code"].ToString(),
                                CTLOC_Desc = reader["CTLOC_Desc"].ToString()
                            };
                            locations.Add(loc);
                        }
                    }
                }
            }

            return locations;
        }

        public List<Room> GetRoom(string buId, string ward)
        {
            var rooms = new List<Room>();

            using (var con = new CacheConnection(_cache89ConString))
            {
                con.Open();
                using (var cmd = new CacheCommand(QueryString.GetRoom(buId, ward), con))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var room = new Room()
                            {
                                WARD_Code = reader["WARD_Code"].ToString(),
                                ROOM_Code = reader["ROOM_Code"].ToString(),
                                ROOM_Desc = reader["ROOM_Desc"].ToString()
                            };
                            rooms.Add(room);
                        }
                    }
                }
            }

            return rooms;
        }
        public List<Ward> GetWard(string buId)
        {
            var wards = new List<Ward>();

            using (var con = new CacheConnection(_cache89ConString))
            {
                con.Open();
                using (var cmd = new CacheCommand(QueryString.GetWard(buId), con))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var ward = new Ward()
                            {
                                WARD_Code = reader["WARD_Code"].ToString(),
                                WARD_Desc = reader["WARD_Desc"].ToString()
                            };
                            wards.Add(ward);
                        }
                    }
                }
            }

            return wards;
        }

        public List<Patient> GetPatient(string date)
        {
            date = Regex.Replace(date, @"^(.{4})(.{2})(.{2})$", "$1-$2-$3");
            //date = Convert.ToDateTime(date).ToString("yyyy-MM-dd").ToString();
            var patients = new List<Patient>();

            using (var con = new NpgsqlConnection(Constants.PostgreSQL))
            {
                con.Open();
                using (var cmd = new NpgsqlCommand(QueryString.GetPatient(date), con))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var pt = new Patient()
                            {
                                PatientInfo = Helper.JsonStringToPatientInfo(reader["JsonData"].ToString()),
                                PatientLab = Helper.JsonStringToPatientLab(reader["Labjson"].ToString())
                            };
                            patients.Add(pt);
                        }
                    }
                }
            }

            return patients;
        }

        public List<Patient> GetPatient(string date, string locCode)
        {
            date = Regex.Replace(date, @"(\d{4})(\d{2})(\d{2})", "$1-$2-$3");
            //date = Convert.ToDateTime(date).ToString("yyyy-MM-dd").ToString();
            var patients = new List<Patient>();

            using (var con = new NpgsqlConnection(Constants.PostgreSQL))
            {
                con.Open();
                using (var cmd = new NpgsqlCommand(QueryString.GetPatient(date, locCode), con))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var pt = new Patient()
                            {
                                PatientInfo = Helper.JsonStringToPatientInfo(reader["JsonData"].ToString()),
                                PatientLab = Helper.JsonStringToPatientLab(reader["Labjson"].ToString())
                            };
                            patients.Add(pt);
                        }

                    }
                }
            }

            return patients;
        }

        public List<Patient> GetPatientByHn(string hn)
        {
            var patients = new List<Patient>();

            if (IsNullOrEmpty(hn)) return patients;
            if (!hn.Contains("-"))
            {
                hn = Regex.Replace(hn, @"^(.{2})(.{2})(.{6})$", "$1-$2-$3");
            }

            using (var con = new NpgsqlConnection(Constants.PostgreSQL))
            {
                con.Open();
                using (var cmd = new NpgsqlCommand(QueryString.GetPatientByHn(hn), con))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var pt = new Patient()
                            {
                                PatientInfo = Helper.JsonStringToPatientInfo(reader["JsonData"].ToString()),
                                PatientLab = Helper.JsonStringToPatientLab(reader["Labjson"].ToString())
                            };
                            patients.Add(pt);
                        }
                    }
                }
            }

            return patients;
        }

        public PatientHN GetPatientHn(string epiNo)
        {
            var ptHn = new PatientHN();

            using (var con = new CacheConnection(Constants.Cache89))
            {
                con.Open();
                using (var cmd = new CacheCommand(QueryString.GetPatientHn(epiNo)))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            ptHn.HN = reader["Papmi_No"].ToString();
                        }
                    }
                }
            }

            return ptHn;
        }

        public PatientInfo GetPatientInfo(string hn)
        {
            var epiNo = GetData.GetLastEpisode(hn);

            var ptInfo = new PatientInfo();

            if (IsNullOrEmpty(hn)) return ptInfo;

            if (!hn.Contains("-"))
            {
                hn = Regex.Replace(hn, @"^(.{2})(.{2})(.{6})$", "$1-$2-$3");
            }

            using (var con = new CacheConnection(Constants.Cache89))
            {
                con.Open();
                using (var cmd = new CacheCommand(QueryString.GetPatientInfo(hn), con))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read()) return ptInfo;
                        var url = "http://10.104.10.45/CRM_API/api/crm/GetListCRM?hn=" + HttpUtility.UrlEncode(hn);
                        ptInfo.PAPMI_No = reader["PAPMI_No"].ToString();
                        ptInfo.Name = IsNullOrEmpty(reader["TTL_Desc"].ToString()) ? reader["PAPMI_Name"] + " " + reader["PAPMI_Name2"] : reader["TTL_Desc"] + " " + reader["PAPMI_Name"] + " " + reader["PAPMI_Name2"];
                        ptInfo.PAPMI_DOB = Helper.ConvertDate(reader["PAPMI_DOB"].ToString());
                        ptInfo.PAPER_AgeYr = IsNullOrEmpty(reader["PAPER_AgeYr"].ToString()) ? 0 : Convert.ToInt16(reader["PAPER_AgeYr"].ToString());
                        ptInfo.PAPER_AgeMth = IsNullOrEmpty(reader["PAPER_AgeMth"].ToString()) ? 0 : Convert.ToInt16(reader["PAPER_AgeMth"].ToString());
                        ptInfo.PAPER_AgeDay = IsNullOrEmpty(reader["PAPER_AgeDay"].ToString()) ? 0 : Convert.ToInt16(reader["PAPER_AgeDay"].ToString());
                        ptInfo.CTSEX_Desc = reader["CTSEX_Desc"].ToString();
                        ptInfo.PAPER_StName = reader["PAPER_StName"].ToString();
                        ptInfo.CTCIT_Desc = reader["CTCIT_Desc"].ToString();
                        ptInfo.Address = Helper.GetAddress(reader["PAPER_StName"].ToString(), reader["CITAREA_Desc"].ToString(), reader["CTCIT_Desc"].ToString(), reader["PROV_Desc"].ToString(), reader["CTZIP_Code"].ToString());
                        ptInfo.PAPER_TelH = reader["PAPER_TelH"].ToString();
                        ptInfo.IsImage = GetData.IsImage(hn);
                        ptInfo.AlertMsgs = GetData.GetAlertMsg(hn);
                        ptInfo.PatientCategory = GetData.GetPatientCategory(hn);
                        ptInfo.Episode = GetData.GetEpisode(hn);
                        ptInfo.Appointments = GetData.GetAppointment(hn);
                        ptInfo.Allergys = GetData.GetAllergys(hn);

                        try
                        {
                            ptInfo.CRMs = Helper.SerializedJsonData<List<CRM>>(url);
                        }
                        catch (Exception)
                        {
                            var crms = new List<CRM>
                            {
                                new CRM { CRM_Type = "Error" , CRM_DES = "Check CRMAPI"}
                            };
                            ptInfo.CRMs = crms;
                        }
                    }
                }
            }

            return ptInfo;
        }

        public PatientInfo GetPatientInfoPost(string hn)
        {
            var ptInfo = GetPatientInfo(hn);

            return ptInfo;
        }

        public PatientInfo GetPatientInfoByEpiNo(string epiNo)
        {
            var hn = GetData.GetHn(epiNo);

            var ptInfo = new PatientInfo();

            if (IsNullOrEmpty(hn)) return ptInfo;
            if (!hn.Contains("-"))
            {
                hn = Regex.Replace(hn, @"^(.{2})(.{2})(.{6})$", "$1-$2-$3");
            }

            using (var con = new CacheConnection(Constants.Cache89))
            {
                con.Open();
                using (var cmd = new CacheCommand(QueryString.GetPatientInfo(hn), con))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader == null || !reader.Read()) return ptInfo;
                        ptInfo = new PatientInfo();
                        var url = "http://10.104.10.45/CRM_API/api/crm/GetListCRM?hn=" + HttpUtility.UrlEncode(hn);
                        ptInfo.PAPMI_No = reader["PAPMI_No"].ToString();
                        ptInfo.Name = IsNullOrEmpty(reader["TTL_Desc"].ToString()) ? reader["PAPMI_Name"].ToString() + " " + reader["PAPMI_Name2"].ToString() : reader["TTL_Desc"].ToString() + " " + reader["PAPMI_Name"].ToString() + " " + reader["PAPMI_Name2"].ToString();
                        ptInfo.PAPMI_DOB = Helper.ConvertDate(reader["PAPMI_DOB"].ToString());
                        ptInfo.PAPER_AgeYr = IsNullOrEmpty(reader["PAPER_AgeYr"].ToString()) ? 0 : Convert.ToInt16(reader["PAPER_AgeYr"].ToString());
                        ptInfo.CTSEX_Desc = reader["CTSEX_Desc"].ToString();
                        ptInfo.PAPER_StName = reader["PAPER_StName"].ToString();
                        ptInfo.CTCIT_Desc = reader["CTCIT_Desc"].ToString();
                        ptInfo.Address = Helper.GetAddress(reader["PAPER_StName"].ToString(), reader["CITAREA_Desc"].ToString(), reader["CTCIT_Desc"].ToString(), reader["PROV_Desc"].ToString(), reader["CTZIP_Code"].ToString());
                        ptInfo.PAPER_TelH = reader["PAPER_TelH"].ToString();
                        ptInfo.Episode = GetData.GetEpisode(epiNo);
                        ptInfo.Appointments = GetData.GetAppointment(hn);
                        ptInfo.Allergys = GetData.GetAllergys(hn);
                        try
                        {
                            ptInfo.CRMs = Helper.SerializedJsonData<List<CRM>>(url);
                        }
                        catch (Exception)
                        {
                            var crms = new List<CRM>
                            {
                                new CRM { CRM_Type = "Error" , CRM_DES = "Check CRMAPI"}
                            };
                            ptInfo.CRMs = crms;
                        }
                    }
                }
            }


            return ptInfo;
        }

        public List<PatientLab> GetPatientLab(string epiNo)
        {
            var hn = GetData.GetHn(epiNo);
            var ptLabList = new List<PatientLab>();

            var paras = new Dictionary<string, object>
            {
                { "iHN", hn },
                { "iEpiNo", epiNo }
            };

            //string procedureString = "\"Custom_THSV_Report_ZEN_StoredProc\".\"SVNHRptEprLabResult_GetData\"";
            const string procedureString = @"Custom_THSV_Report_ZEN_StoredProc.SVNHRptEprLabResult_GetData";

            using (var results = InterSystemsDa.DataTableExecuteProcedure(procedureString, paras, Constants.Cache89))
            {
                for (var i = 0; i < results.Rows.Count; i++)
                {
                    var row = results.Rows[i];
                    var ptLab = new PatientLab()
                    {
                        DateOfAuth = row["DateOfAuth"].ToString(),
                        DateOfRec = row["DateOfRec"].ToString(),
                        LabNo = row["LabNo"].ToString(),
                        Department = row["Department"].ToString(),
                        HosCode = row["HosCode"].ToString(),
                        HosDesc = row["HosDesc"].ToString(),
                        tsCode = row["tsCode"].ToString(),
                        tsName = row["tsName"].ToString(),
                        tcCode = row["tcCode"].ToString(),
                        tcname = row["tcname"].ToString(),
                        unit = row["unit"].ToString(),
                        data = row["data"].ToString(),
                        flag = row["flag"].ToString(),
                        low = row["low"].ToString(),
                        high = row["high"].ToString(),
                        Reference = row["Reference"].ToString()
                    };
                    ptLabList.Add(ptLab);
                }
            }

            return ptLabList;
        }

        public HttpResponseMessage GetPatientImage(string hn)
        {
            var dt = InterSystemsDa.DtBindDataCommand(QueryString.GetPatientImage(hn), Constants.Cache89);

            var response = Helper.DataTableToImage(dt, 0, 0);

            return response;
        }

        public bool IsPatientDischarge(string epiNo)
        {
            var strDischarge = InterSystemsDa.BindDataCommand(QueryString.IsPatientDischarge(epiNo), Constants.Cache89);

            var isDischarge = !IsNullOrEmpty(strDischarge);

            return isDischarge;
        }

        public HttpResponseMessage GetPatientImage(string hn, int width, int height)
        {
            var dt = InterSystemsDa.DtBindDataCommand(QueryString.GetPatientImage(hn), Constants.Cache89);

            var response = Helper.DataTableToImage(dt, width, height);

            return response;
        }

        public string GetLocationByLineBeacon(string beaconId)
        {
            var result = Empty;

            var query = QueryString.GetLocationByLineBeacon(beaconId);

            using (var con = new NpgsqlConnection(Constants.PostgreSQL))
            {
                con.Open();
                using (var cmd = new NpgsqlCommand(query.Item1, con))
                {
                    foreach (var pair in query.Item2)
                    {
                        cmd.Parameters.AddWithValue(pair.Key, pair.Value);
                    }
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            result = reader["CTLOC_CODE"].ToString();
                        }
                    }
                }
            }

            return result;
        }

        public BeaconLocation GetBeaconLocation(string beaconId)
        {
            var result = new BeaconLocation();

            var query = QueryString.GetLocationByLineBeacon(beaconId);

            using (var conn = new NpgsqlConnection(Constants.PostgreSQL))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = query.Item1;
                    foreach (var pair in query.Item2)
                    {
                        cmd.Parameters.AddWithValue(pair.Key, pair.Value);
                    }

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read()) return result;
                        result.LineBeacon_CODE = reader["LineBeacon_CODE"].ToString();
                        result.CTLOC_CODE = reader["CTLOC_CODE"].ToString();
                        result.CTLOC_DESC = reader["CTLOC_DESC"].ToString();
                        result.CTLOC_FLOOR = reader["CTLOC_FLOOR"].ToString();
                    }
                }
            }

            return result;
        }

        public List<MedDisch> GetMedDisch(string hn)
        {
            var medDischs = new List<MedDisch>();

            var query = QueryString.GetMedDisch(hn);

            using (var con = new CacheConnection(Constants.Cache89))
            {
                con.Open();
                using (var cmd = new CacheCommand(query.Item1, con))
                {
                    foreach (var pair in query.Item2)
                    {
                        cmd.AddInputParameters(new { key = pair.Value });
                    }
                    try
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var isMedDisch = !IsNullOrEmpty(reader["PAADM_MedDischDate"].ToString());
                                var medDisch = new MedDisch()
                                {
                                    PAADM_ADMNO = reader["PAADM_ADMNO"].ToString(),
                                    IsMedDisch = isMedDisch
                                };
                                medDischs.Add(medDisch);
                            }
                        }
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                }
            }

            return medDischs;
        }
        public List<FinDisch> GetFinDisc(string hn)
        {
            var finDischs = new List<FinDisch>();

            var query = QueryString.GetFinDisc(hn);

            using (var con = new CacheConnection(Constants.Cache89))
            {
                con.Open();
                using (var cmd = new CacheCommand(query.Item1, con))
                {
                    foreach (var pair in query.Item2)
                    {
                        var key = pair.Key;
                        cmd.AddInputParameters(new { key = pair.Value });
                    }
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var isFinDisch = !IsNullOrEmpty(reader["PAADM_FinDischDate"].ToString());
                            var finDisch = new FinDisch()
                            {
                                PAADM_ADMNO = reader["PAADM_ADMNO"].ToString(),
                                IsFinDisch = isFinDisch
                            };
                            finDischs.Add(finDisch);
                        }
                    }
                }
            }

            return finDischs;
        }

        public List<Dischg> GetDischgs(string hn)
        {
            var dischgs = new List<Dischg>();

            var query = QueryString.GetDischg(hn);

            using (var con = new CacheConnection(Constants.Cache89))
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
                            var isDischg = !IsNullOrEmpty(reader["PAADM_DischgDate"].ToString());
                            var dischg = new Dischg()
                            {
                                PAADM_ADMNO = reader["PAADM_ADMNO"].ToString(),
                                IsDischg = isDischg
                            };
                            dischgs.Add(dischg);
                        }
                    }
                }
            }

            return dischgs;
        }

        public Discharges GetDischarges(string hn)
        {
            var discharges = new Discharges();

            var medDischs = GetMedDisch(hn);
            var finDischs = GetFinDisc(hn);
            var dischgs = GetDischgs(hn);

            discharges.MedDischs = medDischs;
            discharges.IsMedDischs = Helper.IsMedDischs(medDischs);
            discharges.FinDischs = finDischs;
            discharges.IsFinDischs = Helper.IsFinDischs(finDischs);
            discharges.Dischgs = dischgs;
            discharges.IsDischgs = Helper.IsDischgs(dischgs);

            return discharges;
        }

        public bool UpdateAllDisch()
        {
            // get hn คนไข้ที่ยังไม่ทำ FinDisch
            var dtAllPatients = PostgreSqlda.DtByProcedure(Constants.GetPatientAll, Constants.PostgreSQL);
            var hns = Empty;

            if (!(dtAllPatients.Rows.Count > 0))
            {
                return false;
            }

            for (var index = 0; index < dtAllPatients.Rows.Count; index++)
            {
                var row = dtAllPatients.Rows[index];
                var hn = row[Constants.Hn].ToString().Trim();

                hns += IsNullOrEmpty(hns) ? $"'{hn}'" : $",'{hn}'";
            }

            // get สถานะ Discharge ทั้งหมด of patients
            var dtAllDischs = GetData.DataAllDischsList(hns);

            // clear ข้อมูล PAADM_ADMNo ที่เป็น null
            var dtAllDischsClearNull = Helper.DataTableClearNull(dtAllDischs, "PAADM_ADMNo");

            // convert datatable to list model
            var data = Helper.DtToPatientDischargesList(dtAllDischsClearNull);

            // convert model to json string
            var jsonData = JsonConvert.SerializeObject(data);

            //update Json To Postgres
            var upJson = PostgreSqlda.UpJsonStringDischs(jsonData);

            return upJson;
        }

        #region now is not used
        public bool UpdateMedDisch()
        {
            var patientMedDischLocs = new List<PatientRegisLoc>();
            var dtAllPatients = PostgreSqlda.DtByProcedure(Constants.GetPatientAll, Constants.PostgreSQL);
            var hns = Empty;

            if (!(dtAllPatients.Rows.Count > 0))
            {
                return false;
            }

            for (var index = 0; index < dtAllPatients.Rows.Count; index++)
            {
                var row = dtAllPatients.Rows[index];
                var hn = row[Constants.Hn].ToString().Trim();

                hns += IsNullOrEmpty(hns) ? $"'{hn}'" : $",'{hn}'";
            }

            // get สถานะ Discharge ทั้งหมด of patients
            var dtAllDischs = GetData.DataAllDischsList(hns);

            // convert datatable to list model
            var data = Helper.DtToPatientDischargesList(dtAllDischs);

            // convert model to json string
            var jsonData = JsonConvert.SerializeObject(data);

            //update Json To Postgres
            var upJson = PostgreSqlda.UpJsonStringDischs(jsonData);

            return upJson;
        }

        public bool UpdateFinDisc()
        {
            var dtMedDischs = PostgreSqlda.DtByProcedure(Constants.GetMedDisch, Constants.PostgreSQL);
            var hns = Empty;

            if (!(dtMedDischs.Rows.Count > 0))
            {
                return false;
            }

            for (var index = 0; index < dtMedDischs.Rows.Count; index++)
            {
                var row = dtMedDischs.Rows[index];
                var hn = row[Constants.Hn].ToString().Trim();

                var finDiscs = GetFinDisc(hn);

                // update JsonDisc To Postgres
                var upJson = PostgreSqlda.UpJsonDischs(GetDischarges(hn), hn);

                if (Helper.IsFinDischs(finDiscs))
                {
                    hns += IsNullOrEmpty(hns) ? $"{hn}" : $",{hn}";
                }
            }

            if (IsNullOrEmpty(hns))
            {
                return false;
            }

            var tup = Tuple.Create(Constants.Hnlist, hns);
            // update findisc true, false
            var isSuccess = PostgreSqlda.ExcecuteProcedure(Constants.UpFinDisc, Constants.PostgreSQL, tup);

            return isSuccess;
        }

        public bool UpdateDischarge()
        {
            var dtFinDischs = PostgreSqlda.DtByProcedure(Constants.GetFinDisc, Constants.PostgreSQL);
            var hns = Empty;

            if (!(dtFinDischs.Rows.Count > 0))
            {
                return false;
            }

            for (var index = 0; index < dtFinDischs.Rows.Count; index++)
            {
                var row = dtFinDischs.Rows[index];
                var hn = row[Constants.Hn].ToString().Trim();

                var dischgs = GetDischgs(hn);

                // // update JsonDisc To Postgres
                var upJson = PostgreSqlda.UpJsonDischs(GetDischarges(hn), hn);

                if (Helper.IsDischgs(dischgs))
                {
                    hns += IsNullOrEmpty(hns) ? $"{hn}" : $",{hn}";
                }
            }

            if (IsNullOrEmpty(hns))
            {
                return false;
            }

            var tup = Tuple.Create(Constants.Hnlist, hns);
            // update discharge true, false
            var isSuccess = PostgreSqlda.ExcecuteProcedure(Constants.UpDischarge, Constants.PostgreSQL, tup);

            return isSuccess;
        }
        #endregion

        public bool UpdatePromptPay(string hn, string message)
        {
            message = IsNullOrEmpty(message) ? "" : message;
            var lstTup = new List<Tuple<string, string>>();
            var tupHn = Tuple.Create(Constants.Hn, hn);
            var tupMessage = Tuple.Create("message", message);
            lstTup.Add(tupHn);
            lstTup.Add(tupMessage);
            var isSuccess = PostgreSqlda.ExcecuteProcedures(Constants.UpPromptPay, Constants.PostgreSQL, lstTup);

            return isSuccess;
        }
        public bool UpdateRegisLoc(string hn)
        {

            hn = Helper.HnFormat(hn);

            // get PatientRegisLoc, LocRegis
            var regisLoc = GetData.GetPatientRegisLoc(hn);

            // insert string locreig to posgress => loc1|loc2 UpListRegisLoc
            var upRegis = PostgreSqlda.UpRegisLoc(regisLoc.Item1.Papmi_No, regisLoc.Item1.RegisLoc);

            // insert list model locRegis to posgress convert to string json format เปลี่ยนไป up จาก store procedure
            //var upLstRegis = PostgreSQLDA.UpListRegisLoc(hn, regisLoc.Item2);

            return upRegis;
        }
        public bool UpdateNewRegis(string hn, string loc)
        {

            //var regisLoc = GetData.GetPatientRegisLoc(hn, loc);

            // update list by store procedure ที่ทำการกดให้เป็น true
            var upLstRegis = PostgreSqlda.UpListRegisLoc(hn, loc);

            return upLstRegis;
        }
        // run every 10 sec on node-red พบแพทย์
        public bool UpdateSeeToDoctor()
        {

            // get hn คนไข้ที่ยังไม่ปิด Med
            var dtAllPatients = PostgreSqlda.DtByProcedure(Constants.GetPatientSeeDoctor, Constants.PostgreSQL);

            var hns = Empty;

            if (!(dtAllPatients.Rows.Count > 0))
            {
                return false;
            }

            for (var index = 0; index < dtAllPatients.Rows.Count; index++)
            {
                var row = dtAllPatients.Rows[index];
                var hn = row[Constants.Hn].ToString().Trim();

                hns += IsNullOrEmpty(hns) ? $"'{hn}'" : $",'{hn}'";
            }

            // get สถานะ PatientSeeToDoctor ทั้งหมด of patients
            var dtPatientSeeToDoctors = GetData.PatientSeeToDoctor(hns);

            // convert datatable to list model
            var data = Helper.DtToPatientSeeDoctorList(dtPatientSeeToDoctors);

            // convert model to json string
            var jsonData = JsonConvert.SerializeObject(data);

            //update Json To Postgres
            //var upJson = PostgreSQLDA.UpJsonStringDischs(jsonData);

            return false;
        }
        public bool UpdatePatientBilled()
        {
            var listPatientBilled = new List<PatientBilled>();

            // get all hn ที่ทำการปิด finDisc
            var dtPtNoBilled = PostgreSqlda.DtByProcedure(Constants.GetPatientNoBilled, Constants.PostgreSQL);
            var hns = Empty;
            var lstHn = new List<string>();

            if (!(dtPtNoBilled.Rows.Count > 0))
            {
                return false;
            }
            
            // get all from trakcare
            for (var index = 0; index < dtPtNoBilled.Rows.Count; index++)
            {
                var row = dtPtNoBilled.Rows[index];
                var hn = row[Constants.Hn].ToString().Trim();
                hns += IsNullOrEmpty(hns) ? $"'{hn}'" : $",'{hn}'";
            }

            //test
            //hns = "'11-12-044184'";

            lstHn.Add(hns);

            #region split get from trackcare
            //int i = 1;
            //foreach (DataRow row in dtPTNoBilled.Rows)
            //{

            //    string hn = row[Constants.hn].ToString().Trim();
            //    if (i < dtPTNoBilled.Rows.Count)
            //    {
            //        if (i % 200 == 0)
            //        {
            //            lstHn.Add(hns);
            //            hns = string.Empty;
            //        }
            //        hns += string.IsNullOrEmpty(hns) ? $"'{hn}'" : $",'{hn}'";
            //    }

            //    if (i == dtPTNoBilled.Rows.Count)
            //    {
            //        lstHn.Add(hns);
            //    }

            //    i++;
            //}

            //if (!(lstHn.Count > 0))
            //{
            //    return false;
            //}

            #endregion

            for (var index = lstHn.Count - 1; index >= 0; index--)
            {
                var item = lstHn[index];

                // get สถานะ FinDisch ทั้งหมด of patients
                var dtAllPatientBilled = GetData.GetAllPatientBilled(item);

                // clear ข้อมูล PAADM_ADMNo ที่เป็น null
                var dtAllPatientBilledClearNull = Helper.DataTableClearNull(dtAllPatientBilled, "PAADM_ADMNo");

                // convert datatable to list model
                var data = Helper.DtToPatientBilledList(dtAllPatientBilledClearNull);

                // concat lisit data
                listPatientBilled = listPatientBilled.Concat(data).ToList();
            }

            // convert model to json string
            var jsonData = JsonConvert.SerializeObject(listPatientBilled);

            //update Json To Postgres
            var upJson = PostgreSqlda.UpPatientBilledJson(jsonData);

            return upJson;
        }
        public bool UpdatePharCollect()
        {
            // get hn patient MedDisch but not Financial
            var dt = PostgreSqlda.DtByProcedure(Constants.GetMedDischForPharCollect, Constants.PostgreSQL);
            var dtRowCount = dt.Rows.Count;
            var hns = Empty;

            if (!(dtRowCount > 0))
            {
                return false;
            }

            for (var i = 0; i < dtRowCount; i++)
            {
                var row = dt.Rows[i];
                var hn = row[Constants.Hn].ToString().Trim();
                hns += IsNullOrEmpty(hns) ? $"'{hn}'" : $",'{hn}'";
            }

            //hns = $"'11-17-044088','12-04-022135',''";

            // get data from trakcare
            var dtAllPatientPharCollect = GetData.GetAllPatientPharCollect(hns);

            // clear ข้อมูล OEORI_PrescNo ที่เป็น null
            var dtAllPatientPharCollectClearNull = 
                Helper.DataTableClearNull(dtAllPatientPharCollect, "OEORI_PrescNo");

            // convert datatable to list model
            var data = Helper.DtToPatientPharCollectList(dtAllPatientPharCollectClearNull);

            // convert model to json string
            var jsonData = JsonConvert.SerializeObject(data);

            //update Json To Postgres
            var upJson = PostgreSqlda.UpPharCollect(jsonData);

           return upJson;
        }
    }
}