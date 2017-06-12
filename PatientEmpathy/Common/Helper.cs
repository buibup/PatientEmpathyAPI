using CRMWebApi.DA;
using Newtonsoft.Json;
using PatientEmpathy.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
//using System.Web.Script.Serialization;

namespace PatientEmpathy.Common
{
    public class Helper
    {
        private static string conStr = Constants.Cache89;
        public static Episode DataTableToEpisode(DataTable dtEpiIn)
        {
            Episode epi = new Episode();
            List<EpisodeInquiry> epiInqList = new List<EpisodeInquiry>();

            foreach (DataRow row in dtEpiIn.Rows)
            {
                EpisodeInquiry epiInq = new EpisodeInquiry();
                epiInq.PAADM_ADMNo = row["PAADM_ADMNo"].ToString();
                epiInq.PAADM_AdmDate = ConvertDate(row["PAADM_AdmDate"].ToString());
                epiInq.PAADM_AdmTime = Convert.ToDateTime(row["PAADM_AdmTime"].ToString()).ToString("HH:mm");
                epiInq.CTLOC_Code = row["CTLOC_Code"].ToString();
                epiInq.CTLOC_Desc = row["CTLOC_Desc"].ToString();
                epiInq.CTPCP_Code = row["CTPCP_Code"].ToString();
                epiInq.CTPCP_Desc = row["CTPCP_Desc"].ToString();
                epiInq.PAADM_Type = row["PAADM_Type"].ToString();
                epiInq.PAADM_VisitStatus = row["PAADM_VisitStatus"].ToString();
                epiInq.WARD_Code = row["WARD_Code"].ToString();
                epiInq.WARD_Desc = row["WARD_Desc"].ToString();
                epiInq.ROOM_Code = row["ROOM_Code"].ToString();

                var dtIcd10 = InterSystemsDA.DTBindDataCommand(QueryString.GetICD10(epiInq.PAADM_ADMNo), conStr);
                var dtIcd9 = InterSystemsDA.DTBindDataCommand(QueryString.GetICD9(epiInq.PAADM_ADMNo), conStr);

                epiInq.ICD10 = DataTableToICD10(dtIcd10);
                epiInq.ICD9 = DataTableToICD9(dtIcd9);

                epiInqList.Add(epiInq);
            }
            epi.EpisodeInquiry = epiInqList;
            
            return epi;
        }

        static List<ICD10> DataTableToICD10(DataTable dtIcd10)
        {
            List<ICD10> icd10List = new List<ICD10>();
            foreach (DataRow rowICD10 in dtIcd10.Rows)
            {
                ICD10 icd10 = new ICD10();
                icd10.MRCID_Code = rowICD10["MRCID_Code"].ToString();
                icd10.MRCID_Desc = rowICD10["MRCID_Desc"].ToString();
                icd10List.Add(icd10);
            }
            return icd10List;
        }

        static List<ICD9> DataTableToICD9(DataTable dtIcd9)
        {
            List<ICD9> icd9List = new List<ICD9>();
            foreach (DataRow rowICD9 in dtIcd9.Rows)
            {
                ICD9 icd9 = new ICD9();
                icd9.OPER_Code = rowICD9["OPER_Code"].ToString();
                icd9.OPER_Desc = rowICD9["OPER_Desc"].ToString();
                icd9List.Add(icd9);
            }

            return icd9List;
        }

        public static List<Allergy> DataTableToAllergy(DataTable dt)
        {
            List<Allergy> algs = new List<Allergy>();

            foreach (DataRow row in dt.Rows)
            {
                Allergy alg = new Allergy();
                string algName = string.Empty;
                if (!string.IsNullOrEmpty(row["ALG_Comments"].ToString()))
                {
                    algName = row["ALG_Comments"].ToString();
                    alg.Name = algName;
                    alg.Category = GetData.GetAllergyCategory(algName);
                    algs.Add(alg);
                }
                else if (!string.IsNullOrEmpty(row["PHCGE_Name"].ToString()))
                {
                    algName = row["PHCGE_Name"].ToString();
                    alg.Name = algName;
                    alg.Category = GetData.GetAllergyCategory(algName);
                    algs.Add(alg);
                }
                else if (!string.IsNullOrEmpty(row["ALG_Desc"].ToString()))
                {
                    algName = row["ALG_Desc"].ToString();
                    alg.Name = algName;
                    alg.Category = GetData.GetAllergyCategory(algName);
                    algs.Add(alg);
                }
                else if (!string.IsNullOrEmpty(row["ALGR_Desc"].ToString()))
                {
                    algName = row["ALGR_Desc"].ToString();
                    alg.Name = algName;
                    alg.Category = GetData.GetAllergyCategory(algName);
                    algs.Add(alg);
                }
                else if (!string.IsNullOrEmpty(row["PHCD_Name"].ToString()))
                {
                    algName = row["PHCD_Name"].ToString();
                    alg.Name = algName;
                    alg.Category = GetData.GetAllergyCategory(algName);
                    algs.Add(alg);
                }
                else if (!string.IsNullOrEmpty(row["INGR_Desc"].ToString()))
                {
                    algName = row["INGR_Desc"].ToString();
                    alg.Name = algName;
                    alg.Category = GetData.GetAllergyCategory(algName);
                    algs.Add(alg);
                }

            }

            return algs;
        }

        public static string GetAddress(string PAPER_StName, string CITAREA_Desc, string CTCIT_Desc, string PROV_Desc, string CTZIP_Code)
        {
            string result = string.Empty;

            result = string.IsNullOrEmpty(PAPER_StName) ? "" : PAPER_StName + " ";
            result += string.IsNullOrEmpty(CITAREA_Desc) ? "" : CITAREA_Desc + " ";
            result += string.IsNullOrEmpty(CTCIT_Desc) ? "" : CTCIT_Desc + " ";
            result += string.IsNullOrEmpty(PROV_Desc) ? "" : PROV_Desc + " ";
            result += string.IsNullOrEmpty(CTZIP_Code) ? "" : CTZIP_Code;

            return result;
        }

        public static T SerializedJsonData<T>(string url) where T : new()
        {

            var oAuthUrl = string.Format(url);

            var httpWebRequest = (HttpWebRequest)WebRequest.Create(oAuthUrl);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "GET";

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                string result = streamReader.ReadToEnd();
                return !string.IsNullOrEmpty(result) ? JsonConvert.DeserializeObject<T>(result) : new T();
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
            MemoryStream ms = new MemoryStream();
            imageIn.Save(ms, ImageFormat.Jpeg);

            return ms.ToArray();
        }

        public static Image ByteArrayToImage(byte[] byteArrayIn)
        {
            MemoryStream ms = new MemoryStream(byteArrayIn);
            Image returnImage = Image.FromStream(ms);

            return returnImage;
        }

        public static Image ResizeImage(Image image, Size size, bool preserveAspectRatio = true)
        {
            int newWidth;
            int newHeight;
            if (preserveAspectRatio)
            {
                int originalWidth = image.Width;
                int originalHeight = image.Height;
                float percentWidth = (float)size.Width / (float)originalWidth;
                float percentHeight = (float)size.Height / (float)originalHeight;
                float percent = percentHeight < percentWidth ? percentHeight : percentWidth;
                newWidth = (int)(originalWidth * percent);
                newHeight = (int)(originalHeight * percent);
            }
            else
            {
                newWidth = size.Width;
                newHeight = size.Height;
            }
            Image newImage = new Bitmap(newWidth, newHeight);
            using (Graphics graphicsHandle = Graphics.FromImage(newImage))
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
                WebRequest req = WebRequest.Create(url);
                WebResponse res = req.GetResponse();
                Stream stream = res.GetResponseStream();

                using (var memoryStream = new MemoryStream())
                {
                    stream.CopyTo(memoryStream);
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
            HttpResponseMessage response = new HttpResponseMessage();

            string url = "http://10.104.10.45/Images/ImageNotFound.jpg";
            byte[] imageData = DownloadData(url);
            MemoryStream msData = new MemoryStream(imageData);

            using (Image img = Image.FromStream(msData))
            {
                Image imgResize = ResizeImage(img, new Size(width, hegiht));
                byte[] imgBytenewsize = ImageToByteArray(imgResize);
                MemoryStream ms = new MemoryStream(imgBytenewsize);

                response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StreamContent(ms);
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
            }

            return response;
        }

        public static HttpResponseMessage DataTableToImage(DataTable dt, int width, int hegiht)
        {
            width = (width == 0) ? 100 : width;
            hegiht = (hegiht == 0) ? 100 : hegiht;

            HttpResponseMessage response = new HttpResponseMessage();
            if (dt.Rows.Count > 0)
            {
                if (string.IsNullOrEmpty(dt.Rows[0][0].ToString()))
                {
                    //response = NotfoundImage(width, hegiht);
                    response = new HttpResponseMessage(HttpStatusCode.NotFound);
                    return response;
                }
                byte[] imgByteOrigin = (byte[])dt.Rows[0][0];

                Image imgOrigin = (Bitmap)((new ImageConverter()).ConvertFrom(imgByteOrigin));
                Image imgResize = ResizeImage(imgOrigin, new Size(width, hegiht));
                byte[] imgBytenewsize = ImageToByteArray(imgResize);
                MemoryStream ms = new MemoryStream(imgBytenewsize);

                response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StreamContent(ms);
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
            }
            else
            {
                //response = NotfoundImage(width, hegiht);
                response = new HttpResponseMessage(HttpStatusCode.NotFound);
            }

            return response;
        }

        public static bool CheckPatientImage(DataTable dt)
        {
            bool flag = false;

            if (dt.Rows.Count > 0)
            {
                if (string.IsNullOrEmpty(dt.Rows[0][0].ToString()))
                {
                    return flag;
                }
                flag = true;
            }

            return flag;
        }

        public static string ConvertDate(String param_Date)
        {
            String ConvertDate = string.Empty;
            try
            {
                DateTime dt = Convert.ToDateTime(param_Date);
                ConvertDate = dt.ToString("dd/MM/") + dt.Year.ToString();
            }
            catch (Exception)
            {
                ConvertDate = string.Empty;
            }
            return ConvertDate;
        }
    }
}