using MySql.Data.MySqlClient;
using PatientEmpathy.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;

namespace PatientEmpathy.DA
{
    public class MySqlDA
    {
        public static HttpResponseMessage GetImageLineByUserId(string userId, int w, int h)
        {
            byte[] imageBytes = Convert.FromBase64String(GetData.GetNoImageBase64());

            MySqlConnection conn = new MySqlConnection
            {
                ConnectionString = Constants.MySql
            };

            try
            {
                conn.Open();
                using(var cmd = new MySqlCommand(Constants.GetLineImageByUserId, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@usrId", userId);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var data = reader["pictureBase64"].ToString();

                            try
                            {
                                if (string.IsNullOrEmpty(data))
                                {
                                    return Helper.ByteArrayToImage(imageBytes, w, h);
                                }
                                else
                                {
                                    var stringBase64 = Encoding.ASCII.GetString((byte[])(reader["pictureBase64"]));
                                    imageBytes = Convert.FromBase64String(stringBase64);

                                    return Helper.ByteArrayToImage(imageBytes, w, h);
                                }
                            }
                            catch (Exception)
                            {
                                return Helper.ByteArrayToImage(imageBytes, w, h);
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                return Helper.ByteArrayToImage(imageBytes, w, h);
            }
            finally
            {
                conn.Close();
            }

            return Helper.ByteArrayToImage(imageBytes, w, h);
        }
    }
}