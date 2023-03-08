using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MuchosDicenDique
{
    class AppManager
    {
        IPInterfaceProperties ipProps;
        bool ipPropsLoaded;
        string virtualBoxVersion;
        public AppManager()
        {
            virtualBoxVersion = "N/A";
            foreach (ManagementObject program in new ManagementObjectSearcher("SELECT * FROM Win32_Product WHERE name LIKE 'Oracle VM VirtualBox%'").Get())
            {
                if (program != null) { virtualBoxVersion = program.GetPropertyValue("Version").ToString(); }
            }
            Console.WriteLine(virtualBoxVersion);
            NetworkInterface[] returnNet = NetworkInterface.GetAllNetworkInterfaces().Where(net => net.OperationalStatus == OperationalStatus.Up && (net.Name == "Wi-Fi" || net.Name == "Ethernet")).ToArray();
            if (returnNet.Length > 0)
            {
                ipProps = returnNet[0].GetIPProperties();
                ipPropsLoaded = true;
            }
            else { throw new Exception("Interafaz IP No Encontrada :("); }
        }
        public bool isRdInstalled()
        {
            ManagementObjectSearcher p = new ManagementObjectSearcher("SELECT * FROM Win32_Product");
            foreach (ManagementObject program in p.Get())
            {
                if (program != null && program.GetPropertyValue("Name") != null && program.GetPropertyValue("Name").ToString().Contains("Microsoft Visual Studio 2012 Remote Debugger"))
                {
                    return true;
                }
                if (program != null && program.GetPropertyValue("Name") != null)
                {
                    Trace.WriteLine(program.GetPropertyValue("Name"));
                }
            }
            return false;
        }
        public string GetNetworkAddress()
        {
            IPAddress[] IPs = ipProps.UnicastAddresses.Select(addr => addr.Address).Where(addr => addr.AddressFamily == AddressFamily.InterNetwork).ToArray();
            return IPs.Length > 0 ? IPs[0].ToString() : "---";
        }
        public string GetNetworkGateway()
        {
            IPAddress[] IPs = ipProps.GatewayAddresses.Select(addr => addr.Address).Where(addr => addr.AddressFamily == AddressFamily.InterNetwork).ToArray();
            return IPs.Length > 0 ? IPs[0].ToString() : "---";
        }
        public enum PingResults { Failed, Unstable, Success };
        public PingResults PingHost()
        {
            using (Ping ping = new Ping())
            {
                const int total = 4;
                int successes = 0;
                PingReply reply;
                try
                {
                    for (int i = 0; i < total; i++)
                    {
                        reply = ping.Send("1.1.1.1");
                        if (reply.Status == IPStatus.Success) { successes++; }
                    }
                }
                catch (PingException) { return PingResults.Failed; }
                return successes == total ? PingResults.Success : successes > 0 ? PingResults.Unstable : PingResults.Failed;
            }
        }
        public string GetLastVirtualBoxVersion()
        {
            string s = "";
            using (WebClient wc = new WebClient()) { s = wc.DownloadString("https://www.virtualbox.org/wiki/Downloads"); }
            Match m = Regex.Match(s, @"VirtualBox</a> (\d\.\d(?:\.\d)?)");
            if (m.Success && m.Groups.Count > 1) { return m.Groups[1].Value; }
            return "---";
        }
    }
}