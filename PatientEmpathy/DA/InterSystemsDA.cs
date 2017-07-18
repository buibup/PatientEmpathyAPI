using InterSystems.Data.CacheClient;
using PatientEmpathy.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace CRMWebApi.DA
{
    public class InterSystemsDA
    {
        public static DataTable DTBindDataCommandWithValues(string cmdString, string conString, string hn)
        {
            DataTable dt = new DataTable();

            using (var con = new CacheConnection(conString))
            {
                con.Open();
                using (var cmd = new CacheCommand(cmdString, con))
                {
                    cmd.AddInputParameters(new { PAPMI_No = hn });
                    using (CacheDataReader reader = cmd.ExecuteReader())
                    {
                        dt.Load(reader);
                    }
                }
            }

            return dt;
        }

        public static DataTable DTBindDataCommandWithValuesMultiple(string cmdString, string conString, string hn)
        {
            DataTable dt = new DataTable();

            using (var con = new CacheConnection(conString))
            {
                con.Open();
                using (var cmd = new CacheCommand(cmdString, con))
                {
                    cmd.AddInputParameters(new { PAPMI_No = hn, PAPMI_No1 = hn });
                    using (CacheDataReader reader = cmd.ExecuteReader())
                    {
                        dt.Load(reader);
                    }
                }
            }

            return dt;
        }

        public static DataTable DTBindDataCommandWihDictionary(string cmdString, string conString, Dictionary<string, string> dics)
        {
            DataTable dt = new DataTable();
            using (var con = new CacheConnection(conString))
            {
                con.Open();

                using (var cmd = new CacheCommand(cmdString, con))
                {
                    foreach(KeyValuePair<string,string> pair in dics)
                    {
                        var key = pair.Key;
                        cmd.AddInputParameters(new { key = pair.Value });
                    }
                    using(var reader = cmd.ExecuteReader())
                    {
                        dt.Load(reader);
                    }
                }
            }

            return dt;
        }


        public static DataTable DTBindDataCommand(string cmdString, string conString)
        {
            DataTable dt = new DataTable();

            using (var con = new CacheConnection(conString))
            {
                using (var adp = new CacheDataAdapter(cmdString, con))
                {
                    adp.Fill(dt);
                }
            }

            return dt;
        }

        public static DataSet DSBindDataCommand(string cmdString, string conString)
        {
            DataSet ds = new DataSet();

            using (var con = new CacheConnection(conString))
            {
                using (var adp = new CacheDataAdapter(cmdString, conString))
                {
                    adp.Fill(ds);
                }
            }

            return ds;
        }

        public static string BindDataCommand(string cmdString, string conString)
        {
            string result = string.Empty;

            using (var con = new CacheConnection(conString))
            {
                con.Open();
                using (var cmd = new CacheCommand(cmdString, con))
                {
                    try
                    {
                        result = cmd.ExecuteScalar().ToString();
                    }
                    catch (Exception)
                    {

                        return result;
                    }

                }
            }

            return result;
        }

        public static DataTable DataTableExecuteProcedure(string cmdString, Dictionary<string, object> paras, string conString)
        {
            DataTable dt = new DataTable();
            using (var con = new CacheConnection(conString))
            {
                con.Open();
                using (var cmd = new CacheCommand(cmdString, con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (paras != null)
                    {
                        foreach (KeyValuePair<string, object> kvp in paras)
                            cmd.Parameters.Add(new CacheParameter(kvp.Key, kvp.Value));
                        using (CacheDataReader dr = cmd.ExecuteReader())
                        {
                            dt.Load(dr);
                            return dt;
                        }
                    }
                }
            }
            return dt;
        }
    }
}