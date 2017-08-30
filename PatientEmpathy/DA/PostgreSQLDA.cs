using Newtonsoft.Json;
using Npgsql;
using PatientEmpathy.Common;
using PatientEmpathy.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web.Script.Serialization;

namespace PatientEmpathy.DA
{
    public class PostgreSqlda
    {
        public static DataTable DataTableBindDataCommand(string cmdString, string conString)
        {
            var dt = new DataTable();

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
            var ds = new DataSet();

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

        public static DataTable DtByProcedure(string procedureName, string conString)
        {
            procedureName = $"\"{procedureName}\"";
            var dt = new DataTable();
            NpgsqlConnection conn = null;
            try
            {
                conn = new NpgsqlConnection(conString);
                conn.Open();
                using (var cmd = new NpgsqlCommand(procedureName, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    var dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);

                    dt.Load(dr);

                    return dt;
                };
            }

            catch (NpgsqlException)
            {
                // handle error
                return dt;
            }

            catch (Exception)
            {
                // handle error
                return dt;
            }

            finally
            {
                conn?.Close();
            }
        }

        public static bool ExcecuteProcedure(string procedureName, string conString, Tuple<string, string> tup)
        {
            procedureName = $"\"{procedureName}\"";
            try
            { 
                using(var con = new NpgsqlConnection(conString))
                {
                    con.Open();
                    using(var cmd = new NpgsqlCommand(procedureName, con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue($@"{tup.Item1}", tup.Item2);
                        cmd.ExecuteNonQuery();
                    }
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool ExcecuteProcedures(string procedureName, string conString, List<Tuple<string, string>> lstTup)
        {
            procedureName = $"\"{procedureName}\"";
            try
            {
                using (var con = new NpgsqlConnection(conString))
                {
                    con.Open();
                    using (var cmd = new NpgsqlCommand(procedureName, con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        foreach(var tup in lstTup)
                        {
                            cmd.Parameters.AddWithValue($"@{tup.Item1}", tup.Item2);
                        }
                        cmd.ExecuteNonQuery();
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        public static bool UpJsonDischs(Discharges discharges, string hn)
        {
            var lstTup = new List<Tuple<string, string>>();
            if (discharges == null) return false;
            var tupHn = Tuple.Create(Constants.Hn, hn);
            var json = new JavaScriptSerializer().Serialize(discharges);
            var tupJson = Tuple.Create(Constants.Jsondata, json);

            lstTup.Add(tupHn);
            lstTup.Add(tupJson);

            var upDisJson = ExcecuteProcedures(Constants.UpAllDischJson, Constants.PostgreSQL, lstTup);

            return upDisJson;
        }

        public static bool UpJsonStringDischs(string jsonData)
        {
            var lstTup = new List<Tuple<string, string>>();
            var tupJson = Tuple.Create(Constants.Jsondata, jsonData);
            lstTup.Add(tupJson);
            
            var upDisJson = ExcecuteProcedures(Constants.UpAllDischJson, Constants.PostgreSQL, lstTup);

            return upDisJson;
        }

        public static bool UpRegisLoc(string hn, string loc)
        {
            var lstTup = new List<Tuple<string, string>>();
            var tupHn = Tuple.Create(Constants.Hn, hn);
            var tupLoc = Tuple.Create("loc_opd_list", loc);
            lstTup.Add(tupHn);
            lstTup.Add(tupLoc);

            var upDisJson = ExcecuteProcedures(Constants.UpRegisLoc, Constants.PostgreSQL, lstTup);

            return upDisJson;
        }

        public static bool UpListRegisLoc(string hn, List<LocRegis> lstLocRegis)
        {
            var jsonLstLoc = JsonConvert.SerializeObject(lstLocRegis);
            var lstTup = new List<Tuple<string, string>>();
            var tupHn = Tuple.Create(Constants.Hn, hn);
            var tuplstLoc = Tuple.Create("jsondata", jsonLstLoc);
            lstTup.Add(tupHn);
            lstTup.Add(tuplstLoc);

            var upDisJson = ExcecuteProcedures(Constants.UpNewRegis, Constants.PostgreSQL, lstTup);

            return upDisJson;
        }

        public static bool UpListRegisLoc(string hn, string loc)
        {
            var lstTup = new List<Tuple<string, string>>();
            var tupHn = Tuple.Create(Constants.Hn, hn);
            var tuplstLoc = Tuple.Create("loc_newregis", loc);
            lstTup.Add(tupHn);
            lstTup.Add(tuplstLoc);

            var upDisJson = ExcecuteProcedures(Constants.UpNewRegis, Constants.PostgreSQL, lstTup);

            return upDisJson;
        }

        public static bool UpPatientBilledJson(string jsonData)
        {
            var lstTup = new List<Tuple<string, string>>();
            var tupJson = Tuple.Create(Constants.Jsondata, jsonData);
            lstTup.Add(tupJson);

            var upDisJson = ExcecuteProcedures(Constants.UpPatientBilledJson, Constants.PostgreSQL, lstTup);

            return upDisJson;
        }

        public static bool UpPharCollect(string jsonData)
        {
            var lstTup = new List<Tuple<string, string>>();
            var tupJson = Tuple.Create(Constants.Jsondata, jsonData);
            lstTup.Add(tupJson);

            var upJson = ExcecuteProcedures(Constants.UpPharCollect, Constants.PostgreSQL, lstTup);

            return upJson;
        }
    }
}