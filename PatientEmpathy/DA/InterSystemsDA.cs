using System;
using System.Collections.Generic;
using System.Data;
using InterSystems.Data.CacheClient;
using PatientEmpathy.Common;

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
        public static DataTable DtBindDataCommandWithValues(string cmdString, string conString, string hn)
        {
            var dt = new DataTable();

            using (var con = new CacheConnection(conString))
            {
                con.Open();
                using (var cmd = new CacheCommand(cmdString, con))
                {
                    cmd.AddInputParameters(new { PAPMI_No = hn });
                    using (var reader = cmd.ExecuteReader())
                    {
                        try
                        {
                            dt.Load(reader);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            throw;
                        }
                        finally
                        {
                            reader.Close();
                            cmd.Dispose();
                            con.Close();
                        }
                    }
                }
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
        public static DataTable DtBindDataCommandWithValuesMultiple(string cmdString, string conString, string hn)
        {
            var dt = new DataTable();

            using (var con = new CacheConnection(conString))
            {
                con.Open();
                using (var cmd = new CacheCommand(cmdString, con))
                {
                    cmd.AddInputParameters(new { PAPMI_No = hn, PAPMI_No1 = hn });
                    using (var reader = cmd.ExecuteReader())
                    {
                        try
                        {
                            dt.Load(reader);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            throw;
                        }
                        finally
                        {
                            reader.Close();
                            cmd.Dispose();
                            con.Close();
                        }
                        
                    }
                }
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
            using (var con = new CacheConnection(conString))
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
                        try
                        {
                            dt.Load(reader);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            throw;
                        }
                        finally
                        {
                            reader.Close();
                            cmd.Dispose();
                            con.Close();
                        }
                        
                    }
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

            using (var con = new CacheConnection(conString))
            {
                using (var adp = new CacheDataAdapter(cmdString, con))
                {
                    try
                    {
                        con.Open();
                        adp.Fill(dt);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                    finally
                    {
                        adp.Dispose();
                        con.Close();
                    }

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
        /// <returns>DataSet</returns>
        public static DataSet DsBindDataCommand(string cmdString, string conString)
        {
            var ds = new DataSet();

            using (var con = new CacheConnection(conString))
            {
                using (var adp = new CacheDataAdapter(cmdString, conString))
                {
                    try
                    {
                        con.Open();
                        adp.Fill(ds);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                    finally
                    {
                        adp.Dispose();
                        con.Close();
                    }

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
            var result = string.Empty;

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
                    finally
                    {
                        cmd.Dispose();
                        con.Close();
                    }

                }
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
            using (var con = new CacheConnection(conString))
            {

                using (var cmd = new CacheCommand(procedureName, con))
                {
                    try
                    {
                        con.Open();
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
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                    finally
                    {
                        cmd.Dispose();
                        con.Close();
                    }

                }

            }
        }
    }
}