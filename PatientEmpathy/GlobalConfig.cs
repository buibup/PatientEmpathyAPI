using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace PatientEmpathy
{
    public static class GlobalConfig
    {
        public static void SetLogAppName()
        {
            log4net.GlobalContext.Properties["AppName"] = Assembly.GetExecutingAssembly().FullName;
        }

        public static ILog GetLogManager(Type c) 
        {
            return log4net.LogManager.GetLogger(c);
        }
    }
}
