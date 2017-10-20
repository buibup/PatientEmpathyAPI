using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterSystems.Data.CacheClient;

namespace ConnectCache
{
    public class DBUtils
    {
        public static CacheConnection GetDBConnection()
        {
            var server = ConfigurationManager.AppSettings["server89"];
            var port = ConfigurationManager.AppSettings["port"];
            var _namespace = ConfigurationManager.AppSettings["namespace"];
            var username = ConfigurationManager.AppSettings["username"];
            var password = ConfigurationManager.AppSettings["password"];
            bool.TryParse(ConfigurationManager.AppSettings["pooling"], out bool _pooling);
            var pooling = _pooling;

            return DBCacheUtils.GetDBConnection(server, port, _namespace, username, password, pooling);
        }

        public static void RemoveAllIdleConnections()
        {
            DBCacheUtils.RemoveAllIdleConnections();
        }
    }
}
