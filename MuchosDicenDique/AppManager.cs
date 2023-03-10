using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using Microsoft.WindowsAPICodePack.Net;

namespace MuchosDicenDique
{
    class AppManager
    {
        IPInterfaceProperties ipProps;
        bool ipPropsLoaded;
        string macAddress;
        string virtualBoxVersion;
        bool ethernetConnection;
        public AppManager()
        {
            LoadVirtualBoxVersion();
            LoadNetworkData();
        }
        void LoadVirtualBoxVersion()
        {
            virtualBoxVersion = "N/A";
            string path = @"C:\Program Files\Oracle\VirtualBox\VirtualBox.exe";
            if (File.Exists(path))
            {
                string rawVersion = FileVersionInfo.GetVersionInfo(path).FileVersion;
                Match m = Regex.Match(rawVersion, @"(\d+(?:\.\d+){0,2})");
                if (m.Success) { virtualBoxVersion = m.Value; }
            }
        }
        void LoadNetworkData()
        {
            NetworkInterface[] returnNet = NetworkInterface.GetAllNetworkInterfaces().Where(net => net.OperationalStatus == OperationalStatus.Up && (net.Name == "Wi-Fi" || net.Name == "Ethernet")).ToArray();
            if (returnNet.Length > 0)
            {
                NetworkInterface net = returnNet[0];
                Console.WriteLine(net.Name);
                ethernetConnection = net.Name == "Ethernet";
                macAddress = net.GetPhysicalAddress().ToString();
                ipProps = net.GetIPProperties();
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
        public string GetCurrentVirtualBoxVersion() { return virtualBoxVersion; }
        public string GetLastVirtualBoxVersion()
        {
            string s = "";
            using (WebClient wc = new WebClient()) { s = wc.DownloadString("https://download.virtualbox.org/virtualbox/"); }
            MatchCollection m = Regex.Matches(s, @"<a.+?>(\d+(?:\.\d+){0,2})/");
            if (m.Count > 0 && m[m.Count - 1].Groups.Count > 1) { return m[m.Count - 1].Groups[1].Value; }
            return "N/A";
        }
        public string GetCurrentUserName() { return Environment.UserName; }
        public string GetCurrentHostName() { return Environment.MachineName; }
        public string GetMACAddress() { return ipPropsLoaded ? macAddress : "---"; }
        public string GetNetworkName()
        {
            string[] networks = NetworkListManager.GetNetworks(NetworkConnectivityLevels.Connected).Where(net => net.IsConnected).Select(net => net.Name).ToArray();
            return networks.Length > 0 ? networks[0] : "---";
        }
        public bool IsConnectedViaEthernet() { return ethernetConnection; }
    }
}