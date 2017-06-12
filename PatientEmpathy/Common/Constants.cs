using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace PatientEmpathy.Common
{
    public class Constants
    {
        public static string Cache89 = ConfigurationManager.ConnectionStrings["Chache89"].ToString();
        public static string Chache112 = ConfigurationManager.ConnectionStrings["Chache112"].ToString();
        public static string svhsql3 = ConfigurationManager.ConnectionStrings["svh-sql3"].ToString();
        public static string PostgreSQL = ConfigurationManager.ConnectionStrings["PostgreSQL"].ToString();
    }
}