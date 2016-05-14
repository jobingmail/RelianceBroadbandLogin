using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace RelianceBBLogin
{
    enum ConnectionStatus
    {
        Connected=1,
        Disconnected,
        NoNetwork,
        Error
    }


    class LoginHelper
    {

        public static bool IsLAN_Available()
        {
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                return true;
            }
            else return false;
        }


        public static bool PingHost(string hostNameOrAddress)
        {
            bool pingable = false;
            Ping pinger = new Ping();


            try
            {
                PingReply reply = pinger.Send(hostNameOrAddress, 2000);

                pingable = reply.Status == IPStatus.Success;

            }
            catch (PingException)
            {
                return false;
            }

            return pingable;
        }

        public static void SetStartup(string AppName,string AppPath, bool enable)
        {
            try
            {
                string runKey = "SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run";

                Microsoft.Win32.RegistryKey startupKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(runKey);

                if (enable)
                {
                    if (startupKey.GetValue(AppName) == null)
                    {
                        startupKey.Close();
                        startupKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(runKey, true);
                        // Add startup reg key
                        startupKey.SetValue(AppName, AppPath);
                        startupKey.Close();
                    }
                }
                else
                {
                    // remove startup
                    startupKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(runKey, true);
                    startupKey.DeleteValue(AppName, false);
                    startupKey.Close();
                }
            }
            catch (Exception) { }
        }
    }
}
