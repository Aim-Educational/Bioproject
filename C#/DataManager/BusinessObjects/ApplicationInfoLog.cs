using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.IO;
using System.Net.NetworkInformation;

using DataManager.Model;
using StandardUtils;

namespace BusinessObjects
{
    public class ApplicationInfoObject : application_log
    {
        public enum Severity
        {
            Warning = 1,
            Error = 2,
            Audit = 3
        }

        public DateTime date;
        public string message;
        public Severity severity;
        public int applicationID;

        public string toCSVString()
        {
            var builder = new StringBuilder(128);
            builder.AppendFormat("{0},", date.ToString());
            builder.AppendFormat("{0},", (int)severity);
            builder.AppendFormat("\"{0}\",", message);
            builder.AppendFormat("{0}", applicationID);
            builder.AppendLine("");

            // I want to test this quickly
            return builder.ToString();
        }

        public static ApplicationInfoObject fromCSVString(string csv)
        {
            return null;
        }
    }

    public class ApplicationInfoLog
    {
        private static void createFileEntry(ApplicationInfoObject info)
        {
        }

        public static void createEntry()
        {
            var databaseIP = ConfigurationManager.AppSettings["databaseIP"];
            if (databaseIP == null)
                throw new Exception("No key called 'databaseIP' in App.config *(make sure it's in the right one)*");

            if(!RSSHelper.isConnectedToTheInternet())
            {
            }

            var ping = new Ping().Send("However we get this");
            if (ping.Status != IPStatus.Success)
            {

            }

            using (var context = new PlanningContext())
            {
                if(!context.Database.Exists())
                {

                }
            }
        }
    }
}
