using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maintainer.Util
{
    public static class Config
    {
        public static string DeviceName  => Environment.MachineName;
        public static string PathToRoot  => ConfigurationManager.AppSettings.Get("path_to_root");
        public static string NetworkUser => ConfigurationManager.AppSettings.Get("network_user");
        public static string NetworkPass => ConfigurationManager.AppSettings.Get("network_pass");
        public static string ServerName  => ConfigurationManager.AppSettings.Get("server_name");
    }
}
