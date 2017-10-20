using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterSystems.Data.CacheClient;
using System.Configuration;
using System.Data;

namespace ConnectCache
{
    public class DBCacheUtils
    {
        public static CacheConnection Conn { get; set; } = new CacheConnection();
        public static CacheConnection 
            GetDBConnection(string server, string port, string _namespace, string username, string password, bool pooling)
        {
            // Server=10.104.10.89; Port=1972; Namespace=PROD-TRAK;Password=sys; User ID=superuser
            var connString = $"Server={server}; Port={port}; Namespace={_namespace}; Password={password}; User ID={username}; Pooling={pooling}";
   
            Conn = new CacheConnection(connString);

            return Conn;
        }

        public static void RemoveAllIdleConnections()
        {
            CachePoolManager.RemoveAllIdleConnections();
        }
    }
}
