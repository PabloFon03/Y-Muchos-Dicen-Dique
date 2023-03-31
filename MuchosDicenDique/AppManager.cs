﻿using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
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
            CreateVM("test", "Ubuntu_64", "C:\\Users\\Pablo\\Downloads\\ubuntu-mate-22.04.2-desktop-amd64.iso");
        }
        #region [Entrega 1]
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
                ethernetConnection = net.NetworkInterfaceType != NetworkInterfaceType.Wireless80211;
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
        #endregion
        #region [Entrega 2]
        string RunVBoxCommand(string _cmd)
        {
            ProcessStartInfo psi = new ProcessStartInfo(@"C:\Program Files\Oracle\VirtualBox\VBoxManage.exe", _cmd);
            psi.RedirectStandardOutput = true;
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;
            Process process = new Process();
            process.StartInfo = psi;
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            return output;
        }
        string[] ListAllVMs()
        {
            string result = RunVBoxCommand("list vms");
            MatchCollection m = Regex.Matches(result, '"' + "(.+)" + '"');
            string[] returnArr = new string[m.Count];
            for (int i = 0; i < returnArr.Length; i++) { returnArr[i] = m[i].Groups[1].Value; }
            return returnArr;
        }
        void ShowVMInfo(string _name) { Console.WriteLine(RunVBoxCommand($"showvminfo \"{_name}\"")); }
        void CreateVM(string _name, string _osType, string _ideRoute)
        {
            // Remove Previous VM
            //RunVBoxCommand($"unregistervm \"{_name}\" --delete");
            string baseRoute = $@"C:\Users\{Environment.UserName}\Desktop\VirtualBox\";
            // Create New VM
            RunVBoxCommand($"createvm --name \"{_name}\" --ostype \"{_osType}\" --register --basefolder {baseRoute}");
            RunVBoxCommand($"modifyvm \"{_name}\" --boot1 dvd --boot2 disk --boot3 none --boot4 none");
            Task storageTask = Task.Factory.StartNew(() =>
            {
                string diskRoute = $"{baseRoute}\\{_name}\\{_name}_DISK.vmdk";
                RunVBoxCommand($"createmedium disk --filename {diskRoute} --size 80000 --format VMDK");
                RunVBoxCommand($"storagectl \"{_name}\" --name \"SATA Controller\" --add sata --controller IntelAhci");
                RunVBoxCommand($"storageattach \"{_name}\" --storagectl \"SATA Controller\" --port 0 --device 0 --type hdd --medium {diskRoute}");
                RunVBoxCommand($"storagectl \"{_name}\" --name \"IDE Controller\" --add ide --controller PIIX4");
                RunVBoxCommand($"storageattach \"{_name}\" --storagectl \"IDE Controller\" --port 1 --device 0 --type dvddrive --medium {_ideRoute}");
            });
            RunVBoxCommand($"modifyvm \"{_name}\" --ioapic on");
            RunVBoxCommand($"modifyvm \"{_name}\" --graphicscontroller VMSVGA");
            RunVBoxCommand($"modifyvm \"{_name}\" --memory 512 --vram 32");
            RunVBoxCommand($"modifyvm \"{_name}\" --nic1 nat");
            storageTask.Wait();
            RunVBoxCommand($"startvm \"{_name}\"");
        }
        #endregion
    }
}