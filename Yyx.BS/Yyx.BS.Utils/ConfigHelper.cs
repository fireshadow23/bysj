using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yyx.BS.Utils
{
    public class ConfigHelper
    {
        public const string VERSION = "version"; //版本号
        public static string GetVersion()
        {
            var version = ConfigurationManager.AppSettings[VERSION];

            return version;
        }
    }
}
