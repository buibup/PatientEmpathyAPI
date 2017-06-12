using System;
using CRMWebApi.DA;
using InterSystems.Data.CacheClient;
using PatientEmpathy.Common;
using PatientEmpathy.Models;
using System.Collections.Generic;
using System.Web;
using System.Data;
using System.Text.RegularExpressions;
using Npgsql;
using System.Text;
using System.IO;
using System.Net.Http;
using System.Net;
using System.Drawing;

namespace PatientEmpathy.Repository
{
    public class PatientRepository : IPatientRepository
    {
        private string cache89ConString = Constants.Cache89;
        public EpisodeInquiry GetEpisodeInquiry(string hn)
        {
            EpisodeInquiry epiInq = null;

            using (var con = new CacheConnection(cache89ConString))
            {
                con.Open();
                using (var cmd = new CacheCommand(QueryString.GetEpisodeInquiry(hn), con))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            epiInq = new EpisodeInquiry();
                            epiInq.PAADM_ADMNo = reader["PAADM_ADMNo"].ToString();
                            epiInq.PAADM_AdmDate = Helper.ConvertDate(reader["PAADM_AdmDate"].ToString());
                            epiInq.PAADM_AdmTime = Convert.ToDateTime(reader["PAADM_AdmTime"].ToString()).ToString("HH:mm");
                            epiInq.CTLOC_Code = reader["CTLOC_Code"].ToString();
                            epiInq.CTLOC_Desc = reader["CTLOC_Desc"].ToString();
                            epiInq.CTPCP_Code = reader["CTPCP_Code"].ToString();
                            epiInq.CTPCP_Desc = reader["CTPCP_Desc"].ToString();
                            epiInq.PAADM_Type = reader["PAADM_Type"].ToString();
                            epiInq.PAADM_VisitStatus = reader["PAADM_VisitStatus"].ToString();
                            epiInq.WARD_Code = reader["WARD_Code"].ToString();
                            epiInq.WARD_Desc = reader["WARD_Desc"].ToString();
                            epiInq.ROOM_Code = reader["ROOM_Code"].ToString();
                        }
                    }
                }
            }

            return epiInq;
        }

        public List<Location> GetLocation(string site)
        {
            List<Location> locations = new List<Location>();
            Location loc = null;

            using (var con = new CacheConnection(cache89ConString))
            {
                con.Open();
                using (var cmd = new CacheCommand(QueryString.GetLocation(site), con))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            loc = new Location();
                            loc.CTLOC_Code = reader["CTLOC_Code"].ToString();
                            loc.CTLOC_Desc = reader["CTLOC_Desc"].ToString();

                            locations.Add(loc);
                        }
                    }
                }
            }

            return locations;
        }

        public List<Room> GetRoom(string buId,string ward)
        {
            List<Room> Rooms = new List<Room>();
            Room room = null;

            using (var con = new CacheConnection(cache89ConString))
            {
                con.Open();
                using (var cmd = new CacheCommand(QueryString.GetRoom(buId,ward), con))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            room = new Room();
                            room.WARD_Code = reader["WARD_Code"].ToString();
                            room.ROOM_Code = reader["ROOM_Code"].ToString();
                            room.ROOM_Desc = reader["ROOM_Desc"].ToString();

                            Rooms.Add(room);
                        }
                    }
                }
            }

            return Rooms;
        }
        public List<Ward> GetWard(string buId)
        {
            List<Ward> Wards = new List<Ward>();
            Ward ward = null;

            using (var con = new CacheConnection(cache89ConString))
            {
                con.Open();
                using (var cmd = new CacheCommand(QueryString.GetWard(buId), con))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ward = new Ward();
                            ward.WARD_Code = reader["WARD_Code"].ToString();
                            ward.WARD_Desc = reader["WARD_Desc"].ToString();

                            Wards.Add(ward);
                        }
                    }
                }
            }

            return Wards;
        }

        public List<Patient> GetPatient(string date)
        {
            date = Regex.Replace(date, @"^(.{4})(.{2})(.{2})$", "$1-$2-$3");
            //date = Convert.ToDateTime(date).ToString("yyyy-MM-dd").ToString();
            List<Patient> patients = new List<Patient>();
            Patient pt = null;

            using (var con = new NpgsqlConnection(Constants.PostgreSQL))
            {
                con.Open();
                using (var cmd = new NpgsqlCommand(QueryString.GetPatient(date), con))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            pt = new Patient();
                            pt.PatientInfo = Helper.JsonStringToPatientInfo(reader["JsonData"].ToString());
                            pt.PatientLab = Helper.JsonStringToPatientLab(reader["Labjson"].ToString());
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
            List<Patient> patients = new List<Patient>();
            Patient pt = null;

            using (var con = new NpgsqlConnection(Constants.PostgreSQL))
            {
                con.Open();
                using (var cmd = new NpgsqlCommand(QueryString.GetPatient(date, locCode), con))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            pt = new Patient();
                            pt.PatientInfo = Helper.JsonStringToPatientInfo(reader["JsonData"].ToString());
                            pt.PatientLab = Helper.JsonStringToPatientLab(reader["Labjson"].ToString());
                            patients.Add(pt);
                        }

                    }
                }
            }

            return patients;
        }

        public List<Patient> GetPatientByHN(string hn)
        {
            List<Patient> patients = new List<Patient>();
            Patient pt = null;

            if (!string.IsNullOrEmpty(hn))
            {
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
                                pt = new Patient();
                                pt.PatientInfo = Helper.JsonStringToPatientInfo(reader["JsonData"].ToString());
                                pt.PatientLab = Helper.JsonStringToPatientLab(reader["Labjson"].ToString());
                                patients.Add(pt);
                            }
                        }
                    }
                }

            }

            return patients;
        }

        public PatientHN GetPatientHN(string epiNo)
        {
            PatientHN ptHN = null;

            using (var con = new CacheConnection(Constants.Cache89))
            {
                con.Open();
                using (var cmd = new CacheCommand(QueryString.GetPatientHN(epiNo)))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            ptHN.HN = reader["Papmi_No"].ToString();
                        }
                    }
                }
            }

            return ptHN;
        }

        public PatientInfo GetPatientInfo(string hn)
        {
            string epiNo = GetData.GetLastEpisode(hn);

            PatientInfo ptInfo = null;

            if (!string.IsNullOrEmpty(hn))
            {

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
                            if (reader.Read())
                            {
                                ptInfo = new PatientInfo();
                                string url = "http://10.104.10.45/CRM_API/api/crm/GetListCRM?hn=" + HttpUtility.UrlEncode(hn);
                                ptInfo.PAPMI_No = reader["PAPMI_No"].ToString();
                                ptInfo.Name = string.IsNullOrEmpty(reader["TTL_Desc"].ToString()) ? reader["PAPMI_Name"].ToString() + " " + reader["PAPMI_Name2"].ToString() : reader["TTL_Desc"].ToString() + " " + reader["PAPMI_Name"].ToString() + " " + reader["PAPMI_Name2"].ToString();
                                ptInfo.PAPMI_DOB = Helper.ConvertDate(reader["PAPMI_DOB"].ToString());
                                ptInfo.PAPER_AgeYr = string.IsNullOrEmpty(reader["PAPER_AgeYr"].ToString()) ? 0 : Convert.ToInt16(reader["PAPER_AgeYr"].ToString());
                                ptInfo.PAPER_AgeMth = string.IsNullOrEmpty(reader["PAPER_AgeMth"].ToString()) ? 0 : Convert.ToInt16(reader["PAPER_AgeMth"].ToString());
                                ptInfo.PAPER_AgeDay = string.IsNullOrEmpty(reader["PAPER_AgeDay"].ToString()) ? 0 : Convert.ToInt16(reader["PAPER_AgeDay"].ToString());
                                ptInfo.CTSEX_Desc = reader["CTSEX_Desc"].ToString();
                                ptInfo.PAPER_StName = reader["PAPER_StName"].ToString();
                                ptInfo.CTCIT_Desc = reader["CTCIT_Desc"].ToString();
                                ptInfo.Address = Helper.GetAddress(reader["PAPER_StName"].ToString(), reader["CITAREA_Desc"].ToString(), reader["CTCIT_Desc"].ToString(), reader["PROV_Desc"].ToString(), reader["CTZIP_Code"].ToString());
                                ptInfo.PAPER_TelH = reader["PAPER_TelH"].ToString();
                                ptInfo.IsImage = GetData.IsImage(hn);
                                ptInfo.Episode = GetData.GetEpisode(hn);
                                ptInfo.Appointments = GetData.GetAppointment(hn);
                                ptInfo.Allergys = GetData.GetAllergys(hn);
                                try
                                {
                                    ptInfo.CRMs = Helper.SerializedJsonData<List<CRM>>(url);
                                }
                                catch (Exception)
                                {
                                    List<CRM> crms = new List<CRM>
                                    {
                                        new CRM { CRM_Type = "Error" , CRM_DES = "Check CRMAPI"}
                                    };
                                    ptInfo.CRMs = crms;
                                }
                            }
                        }
                    }
                }
            }

            return ptInfo;
        }

        public PatientInfo GetPatientInfoPOST(string hn)
        {
            var ptInfo = GetPatientInfo(hn);

            return ptInfo;
        }

        public PatientInfo GetPatientInfoByEpiNo(string epiNo)
        {
            string hn = GetData.GetHN(epiNo);

            PatientInfo ptInfo = null;

            if (!string.IsNullOrEmpty(hn))
            {

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
                            if (reader.Read())
                            {
                                ptInfo = new PatientInfo();
                                string url = "http://10.104.10.45/CRM_API/api/crm/GetListCRM?hn=" + HttpUtility.UrlEncode(hn);
                                ptInfo.PAPMI_No = reader["PAPMI_No"].ToString();
                                ptInfo.Name = string.IsNullOrEmpty(reader["TTL_Desc"].ToString()) ? reader["PAPMI_Name"].ToString() + " " + reader["PAPMI_Name2"].ToString() : reader["TTL_Desc"].ToString() + " " + reader["PAPMI_Name"].ToString() + " " + reader["PAPMI_Name2"].ToString();
                                ptInfo.PAPMI_DOB = Helper.ConvertDate(reader["PAPMI_DOB"].ToString());
                                ptInfo.PAPER_AgeYr = string.IsNullOrEmpty(reader["PAPER_AgeYr"].ToString()) ? 0 : Convert.ToInt16(reader["PAPER_AgeYr"].ToString());
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
                                    List<CRM> crms = new List<CRM>
                                    {
                                        new CRM { CRM_Type = "Error" , CRM_DES = "Check CRMAPI"}
                                    };
                                    ptInfo.CRMs = crms;
                                }
                            }
                        }
                    }
                }
            }

            return ptInfo;
        }

        public List<PatientLab> GetPatientLab(string epiNo)
        {
            string hn = GetData.GetHN(epiNo);
            List<PatientLab> ptLabList = new List<PatientLab>();
            PatientLab ptLab = null;

            Dictionary<string, object> paras = new Dictionary<string, object>();
            paras.Add("iHN", hn);
            paras.Add("iEpiNo", epiNo);

            //string procedureString = "\"Custom_THSV_Report_ZEN_StoredProc\".\"SVNHRptEprLabResult_GetData\"";
            string procedureString = @"Custom_THSV_Report_ZEN_StoredProc.SVNHRptEprLabResult_GetData";

            using (var results = InterSystemsDA.DataTableExecuteProcedure(procedureString, paras, Constants.Cache89))
            {
                foreach (DataRow row in results.Rows)
                {
                    ptLab = new PatientLab();
                    ptLab.DateOfAuth = row["DateOfAuth"].ToString();
                    ptLab.DateOfRec = row["DateOfRec"].ToString();
                    ptLab.LabNo = row["LabNo"].ToString();
                    ptLab.Department = row["Department"].ToString();
                    ptLab.HosCode = row["HosCode"].ToString();
                    ptLab.HosDesc = row["HosDesc"].ToString();
                    ptLab.tsCode = row["tsCode"].ToString();
                    ptLab.tsName = row["tsName"].ToString();
                    ptLab.tcCode = row["tcCode"].ToString();
                    ptLab.tcname = row["tcname"].ToString();
                    ptLab.unit = row["unit"].ToString();
                    ptLab.data = row["data"].ToString();
                    ptLab.flag = row["flag"].ToString();
                    ptLab.low = row["low"].ToString();
                    ptLab.high = row["high"].ToString();
                    ptLab.Reference = row["Reference"].ToString();
                    ptLabList.Add(ptLab);
                }
            }

            return ptLabList;
        }

        public HttpResponseMessage GetPatientImage(string hn)
        {
            var dt = InterSystemsDA.DTBindDataCommand(QueryString.GetPatientImage(hn), Constants.Cache89);
            HttpResponseMessage response = new HttpResponseMessage();

            response = Helper.DataTableToImage(dt, 0, 0);

            return response;
        }

        public bool IsPatientDischarge(string epiNo)
        {
            bool isDischarge;

            string strDischarge = InterSystemsDA.BindDataCommand(QueryString.IsPatientDischarge(epiNo), Constants.Cache89);

            if (string.IsNullOrEmpty(strDischarge))
            {
                isDischarge = false;
            }
            else
            {
                isDischarge = true;
            }

            return isDischarge;
        }

        public HttpResponseMessage GetPatientImage(string hn, int width, int height)
        {
            var dt = InterSystemsDA.DTBindDataCommand(QueryString.GetPatientImage(hn), Constants.Cache89);
            HttpResponseMessage response = new HttpResponseMessage();

            response = Helper.DataTableToImage(dt, width, height);

            return response;
        }
    }
}