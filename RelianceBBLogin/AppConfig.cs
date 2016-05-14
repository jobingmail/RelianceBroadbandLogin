using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RelianceBBLogin
{
    class AppConfig
    {
        public static string DomainIPName
        {
            get { return GetAppSettings("DomainIPName"); }
            set { SetAppSettings("DomainIPName", value); }

        }
        public static string LoginURL
        {
            get { return GetAppSettings("LoginURL"); }
            set { SetAppSettings("LoginURL", value); }
        }

        public static string UserId
        {
            get { return GetAppSettings("UserId"); }
            set { SetAppSettings("UserId", value); }
        }
        public static string Password
        {
            get { return GetAppSettings("Password"); }
            set { SetAppSettings("Password", value); }
        }

        public static string TimerIntervalSec
        {
            get { return GetAppSettings("TimerIntervalSec"); }
            set { SetAppSettings("TimerIntervalSec", value); }
        }

        public static string Notify
        {
            get { return GetAppSettings("Notify"); }
            set { SetAppSettings("Notify", value); }
        }

        public static string GetAppSettings(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        public static void SetAppSettings(string key, string value)
        {
            try
            {
                Configuration oConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

                oConfig.AppSettings.Settings[key].Value = value;
                oConfig.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");
            }
            catch (Exception) { }
        }
    }
}
