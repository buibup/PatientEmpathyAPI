using Newtonsoft.Json;
using PatientEmpathy.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using InterSystems.Data.CacheClient;
using PatientEmpathy.DA;
using static System.Convert;
using DBNull = System.DBNull;
using System.Runtime.Serialization.Formatters.Binary;

namespace PatientEmpathy.Common
{
    public static class Helper
    {
        private static readonly string ConStr = Constants.Cache89;
        public static Episode DataTableToEpisode(DataTable dtEpiIn, CacheConnection con)
        {
            var epi = new Episode();
            var epiInqList = new List<EpisodeInquiry>();

            foreach (DataRow row in dtEpiIn.Rows)
            {
                var PAADM_ADMNo = row["PAADM_ADMNo"].ToString();
                var epiInq = new EpisodeInquiry()
                {
                    PAADM_ADMNo = PAADM_ADMNo,
                    PAADM_AdmDate = ConvertDate(row["PAADM_AdmDate"].ToString()),
                    PAADM_AdmTime = ToDateTime(row["PAADM_AdmTime"].ToString()).ToString("HH:mm"),
                    CTLOC_Code = row["CTLOC_Code"].ToString(),
                    CTLOC_Desc = row["CTLOC_Desc"].ToString(),
                    CTPCP_Code = row["CTPCP_Code"].ToString(),
                    CTPCP_Desc = row["CTPCP_Desc"].ToString(),
                    PAADM_Type = row["PAADM_Type"].ToString(),
                    PAADM_VisitStatus = row["PAADM_VisitStatus"].ToString(),
                    WARD_Code = row["WARD_Code"].ToString(),
                    WARD_Desc = row["WARD_Desc"].ToString(),
                    ROOM_Code = row["ROOM_Code"].ToString()
                };
                var dtIcd10 = InterSystemsDa.DtBindDataCommand(QueryString.GetIcd10(epiInq.PAADM_ADMNo), con);
                var dtIcd9 = InterSystemsDa.DtBindDataCommand(QueryString.GetIcd9(epiInq.PAADM_ADMNo), con);



                epiInq.ICD10 = DataTableToIcd10(dtIcd10);
                epiInq.ICD9 = DataTableToIcd9(dtIcd9);
                epiInq.AppConsults = InterSystemsDa.GetApptConsult(PAADM_ADMNo, con);

                epiInqList.Add(epiInq);
            }
            epi.EpisodeInquiry = epiInqList;

            return epi;
        }

        private static List<ICD10> DataTableToIcd10(DataTable dtIcd10)
        {
            var icd10List = new List<ICD10>();
            for (var index = 0; index < dtIcd10.Rows.Count; index++)
            {
                var rowIcd10 = dtIcd10.Rows[index];
                var icd10 = new ICD10()
                {
                    MRCID_Code = rowIcd10["MRCID_Code"].ToString(),
                    MRCID_Desc = rowIcd10["MRCID_Desc"].ToString()
                };
                icd10List.Add(icd10);
            }
            return icd10List;
        }

        static List<ICD9> DataTableToIcd9(DataTable dtIcd9)
        {
            var icd9List = new List<ICD9>();
            for (var index = 0; index < dtIcd9.Rows.Count; index++)
            {
                var rowIcd9 = dtIcd9.Rows[index];
                var icd9 = new ICD9()
                {
                    OPER_Code = rowIcd9["OPER_Code"].ToString(),
                    OPER_Desc = rowIcd9["OPER_Desc"].ToString()
                };
                icd9List.Add(icd9);
            }

            return icd9List;
        }

        public static List<Allergy> DataTableToAllergy(DataTable dt, CacheConnection con)
        {
            var algs = new List<Allergy>();

            foreach (DataRow row in dt.Rows)
            {
                var alg = new Allergy();
                string algName;
                if (!string.IsNullOrEmpty(row["ALG_Comments"].ToString()))
                {
                    algName = row["ALG_Comments"].ToString();
                    alg.Name = algName;
                    alg.Category = GetData.GetAllergyCategory(algName, con);
                    algs.Add(alg);
                }
                else if (!string.IsNullOrEmpty(row["PHCGE_Name"].ToString()))
                {
                    algName = row["PHCGE_Name"].ToString();
                    alg.Name = algName;
                    alg.Category = GetData.GetAllergyCategory(algName, con);
                    algs.Add(alg);
                }
                else if (!string.IsNullOrEmpty(row["ALG_Desc"].ToString()))
                {
                    algName = row["ALG_Desc"].ToString();
                    alg.Name = algName;
                    alg.Category = GetData.GetAllergyCategory(algName, con);
                    algs.Add(alg);
                }
                else if (!string.IsNullOrEmpty(row["ALGR_Desc"].ToString()))
                {
                    algName = row["ALGR_Desc"].ToString();
                    alg.Name = algName;
                    alg.Category = GetData.GetAllergyCategory(algName, con);
                    algs.Add(alg);
                }
                else if (!string.IsNullOrEmpty(row["PHCD_Name"].ToString()))
                {
                    algName = row["PHCD_Name"].ToString();
                    alg.Name = algName;
                    alg.Category = GetData.GetAllergyCategory(algName, con);
                    algs.Add(alg);
                }
                else if (!string.IsNullOrEmpty(row["INGR_Desc"].ToString()))
                {
                    algName = row["INGR_Desc"].ToString();
                    alg.Name = algName;
                    alg.Category = GetData.GetAllergyCategory(algName, con);
                    algs.Add(alg);
                }

            }

            return algs;
        }

        public static string GetAddress(string paperStName, string citareaDesc, string ctcitDesc, string provDesc, string ctzipCode)
        {
            var result = string.IsNullOrEmpty(paperStName) ? "" : paperStName + " ";
            result += string.IsNullOrEmpty(citareaDesc) ? "" : citareaDesc + " ";
            result += string.IsNullOrEmpty(ctcitDesc) ? "" : ctcitDesc + " ";
            result += string.IsNullOrEmpty(provDesc) ? "" : provDesc + " ";
            result += string.IsNullOrEmpty(ctzipCode) ? "" : ctzipCode;

            return result;
        }

        public static T SerializedJsonData<T>(string url) where T : new()
        {

            var oAuthUrl = string.Format(format: url);

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(oAuthUrl);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "GET";

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                return !string.IsNullOrEmpty(value: result) ? JsonConvert.DeserializeObject<T>(value: result) : new T();
            }
        }

        public static PatientInfo JsonStringToPatientInfo(string json)
        {
            PatientInfo ptInfo = null;
            if (!string.IsNullOrEmpty(json))
            {
                ptInfo = JsonConvert.DeserializeObject<PatientInfo>(json);
            }


            return ptInfo;
        }

        public static List<PatientLab> JsonStringToPatientLab(string json)
        {
            List<PatientLab> ptLab = null;
            if (!string.IsNullOrEmpty(json))
            {
                ptLab = JsonConvert.DeserializeObject<List<PatientLab>>(json);
            }

            return ptLab;
        }

        public static byte[] ImageToByteArray(Image imageIn)
        {
            var ms = new MemoryStream();
            imageIn.Save(ms, ImageFormat.Jpeg);

            return ms.ToArray();
        }
        

        public static Image ResizeImage(Image image, Size size, bool preserveAspectRatio = true)
        {
            int newWidth;
            int newHeight;
            if (preserveAspectRatio)
            {
                var originalWidth = image.Width;
                var originalHeight = image.Height;
                var percentWidth = size.Width / (float)originalWidth;
                var percentHeight = size.Height / (float)originalHeight;
                var percent = percentHeight < percentWidth ? percentHeight : percentWidth;
                newWidth = (int)(originalWidth * percent);
                newHeight = (int)(originalHeight * percent);
            }
            else
            {
                newWidth = size.Width;
                newHeight = size.Height;
            }
            Image newImage = new Bitmap(newWidth, newHeight);
            using (var graphicsHandle = Graphics.FromImage(newImage))
            {
                graphicsHandle.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphicsHandle.DrawImage(image, 0, 0, newWidth, newHeight);
            }

            return newImage;
        }

        public static byte[] DownloadData(string url)
        {
            try
            {
                var req = WebRequest.Create(url);
                var res = req.GetResponse();
                var stream = res.GetResponseStream();

                using (var memoryStream = new MemoryStream())
                {
                    stream?.CopyTo(memoryStream);
                    return memoryStream.ToArray();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public static HttpResponseMessage NotfoundImage(int width, int hegiht)
        {
            HttpResponseMessage response;

            const string url = "http://10.104.10.45/Images/ImageNotFound.jpg";
            var imageData = DownloadData(url);
            var msData = new MemoryStream(imageData);

            using (var img = Image.FromStream(msData))
            {
                var imgResize = ResizeImage(img, new Size(width, hegiht));
                var imgBytenewsize = ImageToByteArray(imgResize);
                var ms = new MemoryStream(imgBytenewsize);

                response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StreamContent(ms)
                };
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
            }

            return response;
        }

        public static HttpResponseMessage DataTableToImage(DataTable dt, int width, int height)
        {
            width = (width == 0) ? 100 : width;
            height = (height == 0) ? 100 : height;

            HttpResponseMessage response;
            if (dt.Rows.Count > 0)
            {
                if (string.IsNullOrEmpty(dt.Rows[0][0].ToString()))
                {
                    //response = NotfoundImage(width, hegiht);
                    response = new HttpResponseMessage(HttpStatusCode.NotFound);
                    return response;
                }
                var imgByteOrigin = (byte[])dt.Rows[0][0];

                var imgOrigin = (Bitmap)((new ImageConverter()).ConvertFrom(imgByteOrigin));
                var imgResize = ResizeImage(imgOrigin, new Size(width, height));
                var imgBytenewsize = ImageToByteArray(imgResize);
                var ms = new MemoryStream(imgBytenewsize);

                response = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StreamContent(ms)
                };
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
            }
            else
            {
                //response = NotfoundImage(width, hegiht);
                response = new HttpResponseMessage(HttpStatusCode.NotFound);
            }

            return response;
        }

        public static HttpResponseMessage ByteArrayToImage(byte[] data, int width, int height)
        {
            width = (width == 0) ? 100 : width;
            height = (height == 0) ? 100 : height;

            HttpResponseMessage response;
            if (data == null)
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }
            else
            {
                try
                {

                    var imgOrigin = (Bitmap)((new ImageConverter()).ConvertFrom(data));
                    var imgResize = ResizeImage(imgOrigin, new Size(width, height));
                    var imgBytenewsize = ImageToByteArray(imgResize);
                    var ms = new MemoryStream(imgBytenewsize);

                    response = new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StreamContent(ms)
                    };
                    response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");

                    return response;
                }
                catch (Exception)
                {

                }

            }

            return new HttpResponseMessage(HttpStatusCode.NotFound);
        }

        public static HttpResponseMessage ObjectToImage(object obj, int width, int height)
        {

            width = (width == 0) ? 100 : width;
            height = (height == 0) ? 100 : height;

            HttpResponseMessage response;
            if (obj == null)
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }
            else
            {
                try
                {
                    //var imgByteOrigin = (byte[])obj;

                    var imgOrigin = (Bitmap)((new ImageConverter()).ConvertFrom(obj));
                    var imgResize = ResizeImage(imgOrigin, new Size(width, height));
                    var imgBytenewsize = ImageToByteArray(imgResize);
                    var ms = new MemoryStream(imgBytenewsize);

                    response = new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StreamContent(ms)
                    };
                    response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");

                    return response;
                }
                catch (Exception ex)
                {

                }
 
            }

            return new HttpResponseMessage(HttpStatusCode.NotFound);
        }

        public static string DataTableToBase64(DataTable dt, int width, int hegiht)
        {
            width = (width == 0) ? 100 : width;
            hegiht = (hegiht == 0) ? 100 : hegiht;

            string result;
            if (dt.Rows.Count > 0)
            {
                if (string.IsNullOrEmpty(dt.Rows[0][0].ToString()))
                {
                    //response = NotfoundImage(width, hegiht);
                    result = string.Empty;
                    return result;
                }
                var imgByteOrigin = (byte[])dt.Rows[0][0];

                var imgOrigin = (Bitmap)((new ImageConverter()).ConvertFrom(imgByteOrigin));
                var imgResize = ResizeImage(imgOrigin, new Size(width, hegiht));
                var imgBytenewsize = ImageToByteArray(imgResize);
                result = Convert.ToBase64String(imgBytenewsize);
            }
            else
            {
                //response = NotfoundImage(width, hegiht);
                result = string.Empty;
            }

            return result;
        }

        public static bool CheckPatientImage(DataTable dt)
        {
            if (dt.Rows.Count <= 0) return false;
            if (string.IsNullOrEmpty(dt.Rows[0][0].ToString()))
            {
                return false;
            }
            try
            {
                var data = (byte[])dt.Rows[0][0];
                if (!(data != null && data.Length > 0))
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        public static string ConvertDate(string paramDate)
        {
            if (string.IsNullOrEmpty(paramDate))
            {
                return "";
            }
            string convertDate;
            try
            {
                var dt = ToDateTime(paramDate);
                convertDate = dt.ToString("dd/MM/") + dt.Year.ToString();
            }
            catch (Exception)
            {
                convertDate = string.Empty;
            }
            return convertDate;
        }

        public static string ConvertToISO_8601(string date, string time)
        {
            if (string.IsNullOrEmpty(date)) return "";

            var dt = new DateTime();
            try
            {
                var dte = ConvertToDateTime(date);
                var tme = TimeSpan.Parse(time);

                dt = dte.Date.Add(tme);
            }
            catch (Exception)
            {
                return date;
            }

            return dt.ToString("O");
        }

        public static string ConvertTime(string paramTime)
        {
            if (string.IsNullOrEmpty(paramTime))
            {
                return "";
            }
            string convertTime;
            try
            {
                var dt = ToDateTime(paramTime);
                convertTime = dt.ToString("HH:mm:ss");
            }
            catch (Exception)
            {
                convertTime = string.Empty;
            }
            return convertTime;
        }

        public static object ReturnEmptyIfNull(this object value)
        {
            return value == DBNull.Value ? string.Empty : (value ?? string.Empty);
        }

        public static List<T> ToList<T>(this DataTable dataTable) where T : new()
        {
            var dataList = new List<T>();

            //Define what attributes to be read from the class
            const BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;

            //Read Attribute Names and Types
            var objFieldNames = typeof(T).GetProperties(flags).Cast<System.Reflection.PropertyInfo>().
                Select(item => new
                {
                    Name = item.Name,
                    Type = Nullable.GetUnderlyingType(item.PropertyType) ?? item.PropertyType
                }).ToList();

            //Read Datatable column names and types
            var dtlFieldNames = dataTable.Columns.Cast<DataColumn>().
                Select(item => new
                {
                    Name = item.ColumnName,
                    Type = item.DataType
                }).ToList();

            foreach (var dataRow in dataTable.AsEnumerable().ToList())
            {
                var classObj = new T();

                foreach (var dtField in dtlFieldNames)
                {
                    var propertyInfos = classObj.GetType().GetProperty(dtField.Name);

                    var field = objFieldNames.Find(x => x.Name == dtField.Name);

                    if (field == null) continue;
                    if (propertyInfos != null && propertyInfos.PropertyType == typeof(DateTime))
                    {
                        propertyInfos.SetValue
                            (classObj, ConvertToDateTime(dataRow[dtField.Name]), null);
                    }
                    else if (propertyInfos != null && propertyInfos.PropertyType == typeof(int))
                    {
                        propertyInfos.SetValue
                            (classObj, ConvertToInt(dataRow[dtField.Name]), null);
                    }
                    else if (propertyInfos != null && propertyInfos.PropertyType == typeof(long))
                    {
                        propertyInfos.SetValue
                            (classObj, ConvertToLong(dataRow[dtField.Name]), null);
                    }
                    else if (propertyInfos != null && propertyInfos.PropertyType == typeof(decimal))
                    {
                        propertyInfos.SetValue
                            (classObj, ConvertToDecimal(dataRow[dtField.Name]), null);
                    }
                    else if (propertyInfos != null && propertyInfos.PropertyType == typeof(string))
                    {
                        propertyInfos.SetValue
                        (classObj,
                            dataRow[dtField.Name] is DateTime
                                ? ConvertToDateString(dataRow[dtField.Name])
                                : ConvertToString(dataRow[dtField.Name]), null);
                    }
                }
                dataList.Add(classObj);
            }
            return dataList;
        }

        private static string ConvertToDateString(object date)
        {
            return date == null ? string.Empty : ToDateTime(date).ToString(CultureInfo.InvariantCulture);
        }

        private static string ConvertToString(object value)
        {
            return Convert.ToString(HelperFunctions.ReturnEmptyIfNull(value));
        }

        private static int ConvertToInt(object value)
        {
            return ToInt32(value.ReturnZeroIfNull());
        }

        private static long ConvertToLong(object value)
        {
            return ToInt64(value.ReturnZeroIfNull());
        }

        private static decimal ConvertToDecimal(object value)
        {
            return ToDecimal(value.ReturnZeroIfNull());
        }

        private static DateTime ConvertToDateTime(object date)
        {
            return ToDateTime(date.ReturnDateTimeMinIfNull());
        }

        public static bool IsMedDischs(List<MedDisch> lst)
        {
            var isCount = lst.Count > 0;
            var result = isCount && lst.All(m => m.IsMedDisch);

            return result;
        }

        public static bool IsFinDischs(List<FinDisch> lst)
        {
            var isCount = lst.Count > 0;
            var result = isCount && lst.All(m => m.IsFinDisch);

            return result;
        }

        public static bool IsDischgs(List<Dischg> lst)
        {
            var isCount = lst.Count > 0;
            var result = isCount && lst.All(m => m.IsDischg);

            return result;
        }

        public static List<PatientDischarges> DtToPatientDischargesList(DataTable dt)
        {
            var patientDischargesList = new List<PatientDischarges>();

            foreach (DataRow row in dt.Rows)
            {
                var epiAllDischList = new List<EPIAllDisch>();
                var hn = row["PAPMI_NO"].ToString();

                #region test
                //if (hn == "11-17-047098")
                //{
                //    var mtest = patientDischargesList.Find(h => h.PAPMI_No == hn);
                //}
                #endregion

                // find data from list by hn
                var m = patientDischargesList.Find(h => h.PAPMI_No == hn);
                if (m != null) continue;
                var drHn = dt.Select($"PAPMI_NO = '{hn}'");
                var flagMedDisch = false;
                var flagFinDisch = false;
                var flagDischg = false;
                var flagMedDischAll = true;

                foreach (var rowHn in drHn)
                {

                    var isMedDisch = !string.IsNullOrEmpty(rowHn["PAADM_MedDischDate"].ToString());
                    var isFinDisch = !string.IsNullOrEmpty(rowHn["PAADM_FinDischDate"].ToString());
                    var isDischg = !string.IsNullOrEmpty(rowHn["PAADM_DischgDate"].ToString());

                    flagMedDisch = flagMedDisch || isMedDisch;
                    flagFinDisch = flagFinDisch || isFinDisch;
                    flagDischg = flagDischg || isDischg;
                    flagMedDischAll = flagMedDischAll && isMedDisch;

                    var medDatetime = string.IsNullOrEmpty(rowHn["PAADM_MedDischDate"].ToString()) ? new Tuple<string, string>("", "") : new Tuple<string, string>(rowHn["PAADM_MedDischDate"].ToString(), rowHn["PAADM_MedDischTime"].ToString());
                    var finDatetime = string.IsNullOrEmpty(rowHn["PAADM_FinDischDate"].ToString()) ? new Tuple<string, string>("", "") : new Tuple<string, string>(rowHn["PAADM_FinDischDate"].ToString(), rowHn["PAADM_FinDischTime"].ToString());
                    var dischgDatetime = string.IsNullOrEmpty(rowHn["PAADM_DischgDate"].ToString()) ? new Tuple<string, string>("", "") : new Tuple<string, string>(rowHn["PAADM_DischgDate"].ToString(), rowHn["PAADM_DischgTime"].ToString());

                    var epiDischs = new EPIAllDisch()
                    {
                        PAADM_ADMNo = rowHn["PAADM_ADMNO"].ToString(),
                        IsMedDisch = isMedDisch,
                        PAADM_MedDischDateTime = ConvertToISO_8601(medDatetime.Item1, medDatetime.Item2),
                        IsFinDisch = isFinDisch,
                        PAADM_FinDischDateTime = ConvertToISO_8601(finDatetime.Item1, finDatetime.Item2),
                        IsDischg = isDischg,
                        PAADM_DischgDateTime = ConvertToISO_8601(dischgDatetime.Item1, dischgDatetime.Item2)
                    };
                    epiAllDischList.Add(epiDischs);
                }

                var patientDischarges = new PatientDischarges()
                {
                    PAPMI_No = hn,
                    EPIAllDisch = epiAllDischList,
                    FlagMedDisch = flagMedDisch,
                    FlagFinDisch = flagFinDisch,
                    FlagDischg = flagDischg,
                    FlagMedDischALL = flagMedDischAll
                };
                patientDischargesList.Add(patientDischarges);
            }

            return patientDischargesList;
        }

        public static List<PatientSeeDoctor> DtToPatientSeeDoctorList(DataTable dt)
        {
            var listPatientSeeDoctor = new List<Models.PatientSeeDoctor>();

            foreach (DataRow row in dt.Rows)
            {
                var hn = row["PAPMI_No"].ToString();

                // find data from list by hn
                var data = listPatientSeeDoctor.Find(h => h.PAPMI_No == hn);


                var patientSeeDoctor = new PatientSeeDoctor()
                {

                };
                listPatientSeeDoctor.Add(patientSeeDoctor);
            }

            return listPatientSeeDoctor;
        }

        public static List<PatientBilled> DtToPatientBilledList(DataTable dt)
        {
            var listPatientBilled = new List<PatientBilled>();

            // เอาข้อมูลจาก datatable ใส่ List PatientBilled
            foreach (DataRow row in dt.Rows)
            {
                var listEpiBilled = new List<EpiBilled>();
                var hn = row["PAPMI_No"].ToString();

                var lastDateTimePrinted = DateTime.MinValue.ToString("O");

                #region test
                //if (hn == "11-00-023472")
                //{
                //    var v = hn;
                //}
                #endregion

                // find data from list by hn
                var data = listPatientBilled.Find(h => h.PAPMI_No == hn);
                if (data != null) continue;
                var flagBilled = true;

                // find hn from datable
                var drHn = dt.Select($"PAPMI_NO = '{hn}'");

                // เอาข้อมูลใส่ List EpiBilled
                foreach (var rowHn in drHn)
                {
                    var isBilledEpi = true;
                    var lstBilled = new List<Billed>();

                    var epi = rowHn["PAADM_ADMNO"].ToString();

                    // find epi from datable
                    var drEpi = drHn.AsEnumerable()
                        .Where(d => d.Field<string>("PAADM_ADMNO") == epi);

                    var dEpi = listEpiBilled.Find(e => e.PAADM_ADMNO == epi);
                    if (dEpi != null) continue;
                    foreach (var rowEpi in drEpi)
                    {
                        var isBilled = !string.IsNullOrEmpty(rowEpi["ARPBL_BillNo"].ToString());

                        isBilledEpi = isBilledEpi && isBilled;
                        flagBilled = flagBilled && isBilled;

                        var tupleDatetimePrinted = string.IsNullOrEmpty(rowEpi["ARPBL_DatePrinted"].ToString()) ? new Tuple<string, string>("", "") : new Tuple<string, string>(rowEpi["ARPBL_DatePrinted"].ToString(), rowEpi["ARPBL_TimePrinted"].ToString());
                        var datetimePrinted = string.IsNullOrEmpty(tupleDatetimePrinted.Item1) ? DateTime.MinValue.ToString("O") : ConvertToISO_8601(tupleDatetimePrinted.Item1, tupleDatetimePrinted.Item2);

                        if (DateTime.Parse(datetimePrinted) > DateTime.Parse(lastDateTimePrinted))
                        {
                            lastDateTimePrinted = datetimePrinted;
                        }

                        var billed = new Billed()
                        {
                            ARPBL_BillNo = rowEpi["ARPBL_BillNo"].ToString(),
                            ARPBL_DatePrinted = ConvertDate(tupleDatetimePrinted.Item1),
                            ARPBL_TimePrinted = ConvertTime(tupleDatetimePrinted.Item2),
                            ARPBL_DateTimePrinted = datetimePrinted == DateTime.MinValue.ToString("O") ? "" : datetimePrinted
                        };
                        lstBilled.Add(billed);
                    }
                    var epiBilled = new EpiBilled()
                    {
                        PAADM_ADMNO = epi,
                        ListBilled = lstBilled,
                        IsBilled = isBilledEpi
                    };
                    listEpiBilled.Add(epiBilled);
                }

                var patientBilled = new PatientBilled()
                {
                    PAPMI_No = row["PAPMI_No"].ToString(),
                    ListEpiBilled = listEpiBilled,
                    FlagBilled = flagBilled,
                    LastDateTimePrinted = lastDateTimePrinted == DateTime.MinValue.ToString("O") ? "" : lastDateTimePrinted
                };
                listPatientBilled.Add(patientBilled);
            }

            return listPatientBilled;
        }

        public static List<PharCollect> DtToPatientPharCollectList(DataTable dt)
        {
            #region OrderStatus
            /**
             * select * from OEC_OrderStatus
             */
            #endregion

            var pharCollectList = new List<PharCollect>();
            foreach (DataRow row in dt.Rows)
            {

                var pharCollect = new PharCollect();
                var epiPharCollectList = new List<EpiPharCollect>();
                var hn = row["PAPMI_No"].ToString();
                var lastCollectDateTime = DateTime.MinValue.ToString("O");

                // find data from list by hn
                var data = pharCollectList.Find(h => h.PAPMI_No == hn);
                if (data != null) continue;
                var hnCollect = true;

                #region test
                //if (hn == "11-09-043707")
                //{
                //    hn = "11-09-043707";
                //}
                #endregion

                // find hn from datatable
                IEnumerable<DataRow> drHn;

                try
                {
                    //1,6,10
                    drHn = dt.Rows.Cast<DataRow>()
                    .Where(x => x.Field<string>("PAPMI_NO") == hn
                    && !string.IsNullOrEmpty(x.Field<string>("OEORI_PrescNo"))
                    && ( x.Field<int>("OEORI_ItemStat_DR") == 1 || x.Field<int>("OEORI_ItemStat_DR") == 6 || x.Field<int>("OEORI_ItemStat_DR") == 10)
                    );
                }
                catch (Exception)
                {

                    drHn = dt.Select($"PAPMI_NO = '{hn}'");
                }

                if (!drHn.Any())
                {
                    hnCollect = false;
                }

                // เอาข้อมูลใส่ List EpiPharCollect
                foreach (var rowHn in drHn)
                {

                    var epiPharCollect = new EpiPharCollect();
                    var pharPrescNoList = new List<PharPrescNo>();

                    var epi = rowHn["PAADM_ADMNO"].ToString();
                    var epiCollect = true;

                    epiPharCollect.PAADM_ADMNo = epi;

                    // find epi from datable
                    DataTable dtEpi;

                    var dataRow = drHn
                        .AsEnumerable()
                        .Where(d => d.Field<string>("PAADM_ADMNO") == epi)
                        .FirstOrDefault(d => d.Field<int>("OEORI_ItemStat_DR") != 4);

                    if (dataRow == null)
                    {
                        dtEpi = new DataTable();
                        epiCollect = false;
                        hnCollect = false;
                    }
                    else
                    {
                        dtEpi = drHn.AsEnumerable()
                        .Where(d => d.Field<string>("PAADM_ADMNO") == epi)
                        .Where(d => d.Field<int>("OEORI_ItemStat_DR") != 4)
                        .CopyToDataTable();
                    }

                    var findEpi = epiPharCollectList.Find(e => e.PAADM_ADMNo == epi);
                    if (findEpi != null) continue;

                    // count distinct OEORI_PrescNo
                    epiPharCollect.TotalPharPrescNo = dtEpi
                        .AsEnumerable()
                        .Select(r => r.Field<string>("OEORI_PrescNo"))
                        .Distinct()
                        .Count();

                    foreach (DataRow rowEpi in dtEpi.Rows)
                    {
                        var pharPrescNoCollect = true;
                        int.TryParse(rowEpi["OEORI_ItemStat_DR"].ToString(), out int ItemStat_DR_EPI);
                        var collect = !string.IsNullOrEmpty(rowEpi["OEORI_PrescNo"].ToString()) && (rowEpi["OEORI_PharmacyStatus"].ToString() == "A" || ItemStat_DR_EPI == 6);
                        epiCollect = epiCollect && collect;
                        hnCollect = hnCollect && epiCollect;

                        var prescNo = rowEpi["OEORI_PrescNo"].ToString();

                        var drPrescNo = drHn.AsEnumerable()
                            .Where(d => d.Field<string>("OEORI_PrescNo") == prescNo)
                            .Where(d => d.Field<int>("OEORI_ItemStat_DR") != 4);


                        var findprescNo = pharPrescNoList.Find(p => p.OEORI_PrescNo == prescNo);
                        if (findprescNo != null) continue;

                        var lastPrescNoUpdate = DateTime.MinValue.ToString("O");

                        foreach (var rowPrescNo in drPrescNo)
                        {
                            var tuplePrescNoUpdate = string.IsNullOrEmpty(rowPrescNo["OEORI_UpdateDate"].ToString()) ? new Tuple<string, string>("", "") : new Tuple<string, string>(rowPrescNo["OEORI_UpdateDate"].ToString(), rowPrescNo["OEORI_UpdateTime"].ToString());
                            var prescNoUpdate = ConvertToISO_8601(tuplePrescNoUpdate.Item1, tuplePrescNoUpdate.Item2);

                            if (DateTime.Parse(prescNoUpdate) > DateTime.Parse(lastPrescNoUpdate))
                            {
                                // update last prescno
                                lastPrescNoUpdate = prescNoUpdate;


                                if (DateTime.Parse(lastPrescNoUpdate) > DateTime.Parse(lastCollectDateTime))
                                {
                                    // update last collect
                                    lastCollectDateTime = lastPrescNoUpdate;
                                }
                            }
                            int.TryParse(rowPrescNo["OEORI_ItemStat_DR"].ToString(), out int ItemStat_DR_PrescNo);
                            pharPrescNoCollect = pharPrescNoCollect && (rowPrescNo["OEORI_PharmacyStatus"].ToString() == "A" || ItemStat_DR_PrescNo == 6);
                        }

                        var pharPrescNo = new PharPrescNo()
                        {
                            OEORI_PrescNo = prescNo,
                            PharPrescNoCollect = pharPrescNoCollect,
                            OEORI_UpdateDateTime = lastPrescNoUpdate
                        };
                        pharPrescNoList.Add(pharPrescNo);
                    }

                    if (!(pharPrescNoList.Count > 0))
                    {
                        pharPrescNoList = new List<PharPrescNo>();
                    }

                    epiPharCollect.PharPrescNoList = pharPrescNoList;
                    epiPharCollect.EpiCollect = epiCollect;

                    epiPharCollectList.Add(epiPharCollect);
                }



                pharCollect.PAPMI_No = hn;
                pharCollect.EpiPharCollectList = epiPharCollectList;
                pharCollect.HnCollect = hnCollect;
                pharCollect.LastCollectDateTime = lastCollectDateTime;
                pharCollectList.Add(pharCollect);
            }

            return pharCollectList;
        }

        public static string HnFormat(string hn)
        {
            if (!hn.Contains("-"))
            {
                return hn = Regex.Replace(hn, @"^(.{2})(.{2})(.{6})$", "$1-$2-$3");
            }
            return hn;
        }

        public static DataTable DataTableClearNull(DataTable dt, string fieldName)
        {
            var dataRow = dt
            .AsEnumerable()
            .FirstOrDefault(d => d.Field<string>(fieldName) != null);

            var rows = dt.AsEnumerable()
                .Where(d => d.Field<string>(fieldName) != null);

            return dataRow != null ? rows.CopyToDataTable() : new DataTable();
        }

        public static bool IsAny<T>(this IEnumerable<T> data)
        {
            return data != null && data.Any();
        }
    }
}