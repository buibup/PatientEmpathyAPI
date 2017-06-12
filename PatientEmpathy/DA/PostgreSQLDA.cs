using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace PatientEmpathy.DA
{
    public class PostgreSQLDA
    {
        public static DataTable DataTableBindDataCommand(string cmdString, string conString)
        {
            DataTable dt = new DataTable();

            using (var con = new NpgsqlConnection(conString))
            {
                con.Open();
                using (var adp = new NpgsqlDataAdapter(cmdString, con))
                {
                    adp.Fill(dt);
                }
            }

            return dt;
        }

        public static DataSet DataSetBindDataCommand(string cmdSrting, string conString)
        {
            DataSet ds = new DataSet();

            using (var con = new NpgsqlConnection(conString))
            {
                con.Open();
                using (var adp = new NpgsqlDataAdapter(conString, con))
                {
                    adp.Fill(ds);
                }
            }

            return ds;
        }
    }
}