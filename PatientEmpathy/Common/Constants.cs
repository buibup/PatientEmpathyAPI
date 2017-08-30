using System.Configuration;

namespace PatientEmpathy.Common
{
    public class Constants
    {
        #region Connectionstring
        public static string Cache89 = ConfigurationManager.ConnectionStrings["Chache89"].ToString();
        public static string Chache112 = ConfigurationManager.ConnectionStrings["Chache112"].ToString();
        public static string svhsql3 = ConfigurationManager.ConnectionStrings["svh-sql3"].ToString();
        public static string PostgreSQL = ConfigurationManager.ConnectionStrings["PostgreSQL"].ToString();
        #endregion

        #region postgresql

        // store procedure
        public static string GetPatientAll = ConfigurationManager.AppSettings["GetPatientAll"];
        public static string GetFastCheckOut = ConfigurationManager.AppSettings["GetFastCheckOut"];
        public static string GetMedDisch = ConfigurationManager.AppSettings["GetMedDisch"];
        public static string GetFinDisc = ConfigurationManager.AppSettings["GetFinDisc"];
        public static string GetDischarge = ConfigurationManager.AppSettings["GetDischarge"];
        public static string GetPatientNoBilled = ConfigurationManager.AppSettings["GetPatientNoBilled"];
        public static string GetPatientSeeDoctor = ConfigurationManager.AppSettings["GetPatientSeeDoctor"];
        public static string GetMedDischForPharCollect = ConfigurationManager.AppSettings["GetMedDischForPharCollect"];

        public static string UpMedDisch = ConfigurationManager.AppSettings["UpMedDisch"];
        public static string UpFinDisc = ConfigurationManager.AppSettings["UpFinDisc"];
        public static string UpDischarge = ConfigurationManager.AppSettings["UpDischarge"];
        public static string UpPromptPay = ConfigurationManager.AppSettings["UpPromptPay"];
        public static string UpAllDischJson = ConfigurationManager.AppSettings["UpAllDischJson"];
        public static string UpRegisLoc = ConfigurationManager.AppSettings["UpRegisLoc"];
        public static string UpNewRegis = ConfigurationManager.AppSettings["UpNewRegis"];
        public static string UpSeeToDoctor = ConfigurationManager.AppSettings["UpSeeToDoctor"];
        public static string UpPatientBilledJson = ConfigurationManager.AppSettings["UpPatientBilledJson"];
        public static string UpPharCollect = ConfigurationManager.AppSettings["UpPharCollect"];

        // parameter
        public static string Hn = ConfigurationManager.AppSettings["hn"];
        public static string Hnlist = ConfigurationManager.AppSettings["hnlist"];
        public static string Jsondata = ConfigurationManager.AppSettings["jsondata"];
        #endregion

    }
}