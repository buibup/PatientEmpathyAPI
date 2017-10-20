using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using log4net.Appender;
using log4net.Core;
using Npgsql;

namespace PatientEmpathy.Logs.Log4NetPostgreSQL
{
    public class PostgresAppender : AppenderSkeleton
    {
        protected override void Append(LoggingEvent loggingEvent)
        {
            //custom implementation
            using (var conn = new NpgsqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["postgres"].ConnectionString))
            {
                conn.Open();
                using (var command = new NpgsqlCommand("insert into logs(app_name,thread,level,location,message,log_date,exception) values(:app_name,:thread,:level,:location,:message,:log_date,:exception)", conn))
                {
                    var appName = command.CreateParameter();
                    appName.Direction = System.Data.ParameterDirection.Input;
                    appName.DbType = System.Data.DbType.String;
                    appName.ParameterName = ":app_name";
                    appName.Value = loggingEvent.LookupProperty("AppName");
                    command.Parameters.Add(appName);

                    var thread = command.CreateParameter();
                    thread.Direction = System.Data.ParameterDirection.Input;
                    thread.DbType = System.Data.DbType.String;
                    thread.ParameterName = ":thread";
                    thread.Value = loggingEvent.ThreadName;
                    command.Parameters.Add(thread);

                    var level = command.CreateParameter();
                    level.Direction = System.Data.ParameterDirection.Input;
                    level.DbType = System.Data.DbType.String;
                    level.ParameterName = ":level";
                    level.Value = loggingEvent.Level;
                    command.Parameters.Add(level);

                    var location = command.CreateParameter();
                    location.Direction = System.Data.ParameterDirection.Input;
                    location.DbType = System.Data.DbType.String;
                    location.ParameterName = ":location";
                    location.Value = loggingEvent.LocationInformation.FullInfo;
                    command.Parameters.Add(location);

                    var message = command.CreateParameter();
                    message.Direction = System.Data.ParameterDirection.Input;
                    message.DbType = System.Data.DbType.String;
                    message.ParameterName = ":message";
                    message.Value = loggingEvent.RenderedMessage;
                    command.Parameters.Add(message);

                    var logDate = command.CreateParameter();
                    logDate.Direction = System.Data.ParameterDirection.Input;
                    logDate.DbType = System.Data.DbType.DateTime2;
                    logDate.ParameterName = ":log_date";
                    logDate.Value = loggingEvent.TimeStamp;
                    command.Parameters.Add(logDate);

                    var exception = command.CreateParameter();
                    exception.Direction = System.Data.ParameterDirection.Input;
                    exception.DbType = System.Data.DbType.String;
                    exception.ParameterName = ":exception";
                    exception.Value = loggingEvent.GetExceptionString();
                    command.Parameters.Add(exception);

                    command.ExecuteNonQuery();
                }
                conn.Close();
            }
        }
    }
}