using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public AppManager()
        {
            NetworkInterface[] returnNet = NetworkInterface.GetAllNetworkInterfaces().Where(net => net.OperationalStatus == OperationalStatus.Up && (net.Name == "Wi-Fi" || net.Name == "Ethernet")).ToArray();
            if (returnNet.Length > 0)
            {
                ipProps = returnNet[0].GetIPProperties();
                ipPropsLoaded = true;
            }
            else { throw new Exception("Interafaz IP No Encontrada :("); }
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
        public PingResults PingHost(string nameOrAddress)
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
                        reply = ping.Send(nameOrAddress);
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