using System;
using System.Data;
using System.Data.SqlClient;

namespace PatientEmpathy.DA
{
    public class SqlServerDa
    {
        public static DataTable DataTableBindDataCommand(string cmdString, string conString)
        {
            var dt = new DataTable();

            using (var con = new SqlConnection(conString))
            {
                using (var adp = new SqlDataAdapter(cmdString, con))
                {
                    con.Open();
                    adp.SelectCommand.CommandTimeout = 0;
                    adp.Fill(dt);
                }
            }
            return dt;
        }

        public static DataSet DataSetBindDataCommand(string cmdString, string conString)
        {
            var ds = new DataSet();

            using (var con = new SqlConnection(conString))
            {
                using (var adp = new SqlDataAdapter(cmdString, con))
                {
                    con.Open();
                    adp.Fill(ds);
                }
            }
            return ds;
        }

        public static string ExecuteScalarBindDataCommand(string cmdString, string conString)
        {
            var result = "";

            using (var con = new SqlConnection(conString))
            {
                using (var cmd = new SqlCommand(cmdString, con))
                {
                    con.Open();
                    result = Convert.ToString(cmd.ExecuteScalar());
                }
            }

            return result;
        }
    }
}