using System;
using System.Collections.Generic;
using System.Data;
using InterSystems.Data.CacheClient;
using PatientEmpathy.Common;
using PatientEmpathy.Models;
using System.Linq;

namespace PatientEmpathy.DA
{
    public class InterSystemsDa
    {
        /// <summary>
        /// Execute command string
        /// Close database connection
        /// </summary>
        /// <param name="cmdString"></param>
        /// <param name="conString"></param>
        /// <param name="hn"></param>
        /// <returns>DataTable</returns>
        public static DataTable DtBindDataCommandWithValues(string cmdString, CacheConnection con, string hn)
        {
            var dt = new DataTable();

            try
            {
                using (var cmd = new CacheCommand(cmdString, con))
                {
                    cmd.AddInputParameters(new { PAPMI_No = hn });
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

            return dt;
        }

        /// <summary>
        /// Execute command string
        /// Close database connection
        /// </summary>
        /// <param name="cmdString"></param>
        /// <param name="conString"></param>
        /// <param name="hn"></param>
        /// <returns>Datatable</returns>
        public static DataTable DtBindDataCommandWithValuesMultiple(string cmdString, CacheConnection con, string hn)
        {
            var dt = new DataTable();

            try
            {
                using (var cmd = new CacheCommand(cmdString, con))
                {
                    cmd.AddInputParameters(new { PAPMI_No = hn, PAPMI_No1 = hn });
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

            return dt;
        }

        /// <summary>
        /// Execute command string
        /// Close database connection
        /// </summary>
        /// <param name="cmdString"></param>
        /// <param name="conString"></param>
        /// <param name="dics"></param>
        /// <returns>DataTable</returns>
        public static DataTable DtBindDataCommandWihDictionary(string cmdString, string conString, Dictionary<string, string> dics)
        {
            var dt = new DataTable();
            using (var con = ConnectCache.DBUtils.GetDBConnection())
            {
                try
                {
                    con.Open();
                    using (var cmd = new CacheCommand(cmdString, con))
                    {
                        foreach (KeyValuePair<string, string> pair in dics)
                        {
                            var key = pair.Key;
                            cmd.AddInputParameters(new { key = pair.Value });
                        }
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

        /// <summary>
        /// Execute command string
        ///  Close connection database
        /// </summary>
        /// <param name="cmdString"></param>
        /// <param name="conString"></param>
        /// <returns>DataTable</returns>
        public static DataTable DtBindDataCommand(string cmdString, string conString)
        {
            var dt = new DataTable();

            using (var con = ConnectCache.DBUtils.GetDBConnection())
            {
                try
                {
                    con.Open();
                    using (var adp = new CacheDataAdapter(cmdString, con))
                    {

                        adp.Fill(dt);
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

        public static DataTable DtBindDataCommand(string cmdString, CacheConnection con)
        {
            var dt = new DataTable();

            try
            {
                using (var adp = new CacheDataAdapter(cmdString, con))
                {
                    adp.Fill(dt);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            return dt;
        }

        /// <summary>
        /// Execute command string
        ///  Close connection database
        /// </summary>
        /// <param name="cmdString"></param>
        /// <param name="conString"></param>
        /// <returns>DataSet</returns>
        public static DataSet DsBindDataCommand(string cmdString, string conString)
        {
            var ds = new DataSet();

            using (var con = ConnectCache.DBUtils.GetDBConnection())
            {
                try
                {
                    con.Open();
                    using (var adp = new CacheDataAdapter(cmdString, conString))
                    {
                        adp.Fill(ds);
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

            return ds;
        }

        /// <summary>
        /// Excete command string
        /// Close connection database
        /// </summary>
        /// <param name="cmdString"></param>
        /// <param name="conString"></param>
        /// <returns>string</returns>
        public static string BindDataCommand(string cmdString, string conString)
        {
            string result;

            using (var con = ConnectCache.DBUtils.GetDBConnection())
            {
                try
                {
                    con.Open();
                    using (var cmd = new CacheCommand(cmdString, con))
                    {
                        result = cmd.ExecuteScalar().ToString();
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

            return result;
        }

        public static string BindDataCommand(string cmdString, CacheConnection con)
        {
            string result;

            try
            {
                using (var cmd = new CacheCommand(cmdString, con))
                {
                    result = cmd.ExecuteScalar().ToString();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                result = string.Empty;
            }

            return result;
        }

        /// <summary>
        /// Execute Store Procedure 
        /// close connection database
        /// </summary>
        /// <param name="procedureName"></param>
        /// <param name="paras"></param>
        /// <param name="conString"></param>
        /// <returns>DataTable</returns>
        public static DataTable DataTableExecuteProcedure(string procedureName, Dictionary<string, object> paras, string conString)
        {
            var dt = new DataTable();
            using (var con = ConnectCache.DBUtils.GetDBConnection())
            {
                try
                {
                    con.Open();
                    using (var cmd = new CacheCommand(procedureName, con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        if (paras == null) return dt;
                        foreach (var kvp in paras)
                            cmd.Parameters.Add(new CacheParameter(kvp.Key, kvp.Value));
                        using (var dr = cmd.ExecuteReader())
                        {
                            dt.Load(dr);
                            return dt;
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
        }


        public static List<Appointment> GetApptConsult(string epi, CacheADOConnection con)
        {
            var apps = new List<Appointment>();
            var query = QueryString.GetApptConsult(epi);

            try
            {
                using (var cmd = new CacheCommand(query.Item1, con))
                {
                    foreach (var pair in query.Item2)
                    {
                        cmd.AddInputParameters(new { key = pair.Value });
                    }
                    using(var reader = cmd.ExecuteReader())
                    {

                        while (reader.Read())
                        {
                            var appDatetime = string.IsNullOrEmpty(reader["AS_Date"].ToString()) ? new Tuple<string, string>("", "") : new Tuple<string, string>(reader["AS_Date"].ToString(), reader["AS_SessStartTime"].ToString());
                            var appDate = string.IsNullOrEmpty(appDatetime.Item1) ? "" : DateTime.Parse(appDatetime.Item1).ToString("dd/MM/yyyy");
                            var appTime = string.IsNullOrEmpty(appDatetime.Item2) ? "" : DateTime.Parse(appDatetime.Item2).ToString("HH:mm");

                            var app = new Appointment()
                            {
                                AS_Date = appDate,
                                AS_SessStartTime = appTime,
                                APPT_Status = reader["APPT_Status"].ToString(),
                                PAADM_VisitStatus = reader["PAADM_VisitStatus"].ToString(),
                                CTLOC_Code = reader["CTLOC_Code"].ToString(),
                                CTLOC_Desc = reader["CTLOC_Desc"].ToString(),
                                CTPCP_Desc = reader["CTPCP_Desc"].ToString(),
                                SER_Desc = reader["SER_Desc"].ToString()
                            };

                            apps.Add(app);
                        }
                    }
                }

                apps = apps.OrderBy(a => string.IsNullOrEmpty(a.AS_Date) ? (DateTime?)null : DateTime.ParseExact(a.AS_Date, "dd/MM/yyyy", null))
                    .ThenBy(a => string.IsNullOrEmpty(a.AS_SessStartTime) ? (DateTime?)null : DateTime.ParseExact(a.AS_SessStartTime, "HH:mm", null)).ToList();

            }
            catch (Exception)
            {
                return apps;
            }

            return apps;
        }
    }
}