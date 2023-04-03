using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using HtmlAgilityPack;
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
            NetworkInterface[] returnNet = NetworkInterface.GetAllNetworkInterfaces().Where(net => net.OperationalStatus == OperationalStatus.Up && net.GetIPProperties().GatewayAddresses.Count > 0).ToArray();
            if (returnNet.Length > 0)
            {
                NetworkInterface net = returnNet[0];
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
        void WriteToDataGridView(DataGridView _logPanel, string _s) { _logPanel.Invoke((Action)(() => { _logPanel.Rows.Add(_s); })); }
        public void CreateVM(string _baseRoute, string _name, string _osType, int _diskMemory, string _diskType, string _diskRoute, string _ideRoute, string _ideOsType, int _memory, int _cores, string _gfxController, int _gfxMemory, string _netController, bool _startVM, DataGridView _logPanel)
        {
            _logPanel.Rows.Clear();
            // Get IDE ISO From Mirror
            Task isoTask = Task.Factory.StartNew(() =>
            {
                if (_ideOsType != "")
                {
                    WriteToDataGridView(_logPanel, "Looking for latest ISO...");
                    string url = "";
                    string filename = "";
                    switch (_ideOsType)
                    {
                        // Debian
                        case "Debian (64-bit)":
                            {
                                url = "https://ftp.caliu.cat/debian-cd/current/amd64/iso-cd/";
                                HtmlWeb web = new HtmlWeb();
                                HtmlAgilityPack.HtmlDocument doc = web.Load(url);
                                HtmlNode[] nodes = doc.DocumentNode.SelectNodes("//a[@href]").ToArray();
                                for (int i = 0; i < nodes.Length && string.IsNullOrEmpty(filename); i++) { if (nodes[i].InnerText == "SHA512SUMS.sign") { filename = nodes[i + 1].InnerText; } }
                            }
                            break;
                        // Ubuntu
                        case "Ubuntu (64-bit)":
                            {
                                url = "https://ftp.caliu.cat/ubuntu-cd/";
                                HtmlWeb web = new HtmlWeb();
                                HtmlAgilityPack.HtmlDocument doc = web.Load(url);
                                HtmlNode[] nodes = doc.DocumentNode.SelectNodes("//a[@href]").ToArray();
                                string baseUrl = url;
                                for (int i = 0; i < nodes.Length; i++)
                                {
                                    Match m = Regex.Match(nodes[i].InnerText, @"(\d+(?:\.\d+){0,2})");
                                    if (m.Success) { url = baseUrl + m.Value + "/"; }
                                }
                                doc = web.Load(url);
                                nodes = doc.DocumentNode.SelectNodes("//a[@href]").ToArray();
                                for (int i = 0; i < nodes.Length && string.IsNullOrEmpty(filename); i++) { if (nodes[i].InnerText == "SHA256SUMS.gpg") { filename = nodes[i + 1].InnerText; } }
                            }
                            break;
                    }
                    if (url != "")
                    {
                        WriteToDataGridView(_logPanel, "Latest ISO found!");
                        _ideRoute = $"{_baseRoute}\\{_osType}\\{filename}";
                        if (!File.Exists(_ideRoute))
                        {
                            WriteToDataGridView(_logPanel, "Downloading ISO...");
                            using (WebClient wc = new WebClient()) { wc.DownloadFile(url, _ideRoute); }
                            WriteToDataGridView(_logPanel, "ISO downloaded successfully!");
                        }
                    }
                    else { _logPanel.Rows.Add("ISO not found, no IDE will be installed."); }
                }
            });
            // Create New VM
            _logPanel.Rows.Add("Creating new VM...");
            RunVBoxCommand($"createvm --name \"{_name}\" --ostype \"{_osType}\" --register --basefolder \"{_baseRoute}\"");
            _logPanel.Rows.Add("VM created successfully!");
            // Enable I/O APIC
            _logPanel.Rows.Add("Enabling I/O APIC...");
            RunVBoxCommand($"modifyvm \"{_name}\" --ioapic on");
            _logPanel.Rows.Add("I/O APIC enabled successfully!");
            // Set CPU Cores
            _logPanel.Rows.Add($"Settings CPU cores ({_cores})...");
            RunVBoxCommand($"modifyvm \"{_name}\" --cpus {_cores}");
            _logPanel.Rows.Add("CPU cores set!");
            // Set RAM Memory
            _logPanel.Rows.Add($"Settings RAM Memory ({_memory}MB)...");
            RunVBoxCommand($"modifyvm \"{_name}\" --memory {_memory}");
            _logPanel.Rows.Add($"RAM Memory set successfully!");
            // Set Video RAM Memory
            _logPanel.Rows.Add($"Settings Video RAM Memory ({_gfxMemory}MB)...");
            RunVBoxCommand($"modifyvm \"{_name}\" --vram {_gfxMemory}");
            _logPanel.Rows.Add($"Video RAM Memory set successfully!");
            // Set GFX Controller
            _logPanel.Rows.Add("Configuring graphics controller...");
            RunVBoxCommand($"modifyvm \"{_name}\" --graphicscontroller {_gfxController}");
            _logPanel.Rows.Add("Graphics controller configured successfully!");
            // Set Net Controller
            _logPanel.Rows.Add("Configuring network controller...");
            RunVBoxCommand($"modifyvm \"{_name}\" --nic1 {_netController}");
            _logPanel.Rows.Add("Network controller configured successfully!");
            // Create Hard Disk Port
            _logPanel.Rows.Add("Adding hard disk port...");
            RunVBoxCommand($"storagectl \"{_name}\" --name \"SATA Controller\" --add sata --controller IntelAhci");
            _logPanel.Rows.Add("Hard disk port added successfully!");
            if (string.IsNullOrEmpty(_diskRoute))
            {
                if (_diskMemory > 0)
                {
                    string diskRoute = $"{_baseRoute}\\{_name}\\{_name}_DISK.{_diskType.ToLower()}";
                    // Create Hard Disk
                    _logPanel.Rows.Add("Creating new hard disk...");
                    RunVBoxCommand($"createmedium disk --filename {diskRoute} --size {_diskMemory} --format {_diskType}");
                    _logPanel.Rows.Add("New hard disk created successfully!");
                    // Attach Hard Disk
                    _logPanel.Rows.Add("Installing new hard disk...");
                    RunVBoxCommand($"storageattach \"{_name}\" --storagectl \"SATA Controller\" --port 0 --device 0 --type hdd --medium {diskRoute}");
                    _logPanel.Rows.Add("New hard disk installed successfully!");
                }
                else { _logPanel.Rows.Add("[Do Nothing] selected. No hard disk will be installed."); }
            }
            else
            {
                // Create Hard Disk Port
                _logPanel.Rows.Add("Adding SATA port...");
                RunVBoxCommand($"storagectl \"{_name}\" --name \"SATA Controller\"--add sata --controller IntelAhci");
                _logPanel.Rows.Add("SATA port added successfully");
                // Attach Hard Disk
                _logPanel.Rows.Add("Installing existing hard disk...");
                RunVBoxCommand($"storageattach \"{_name}\" --storagectl \"SATA Controller\" --port 0 --device 0 --type hdd --medium {_diskRoute}");
                _logPanel.Rows.Add("Existing hard disk installed successfully!");
            }
            // Create IDE
            _logPanel.Rows.Add("Adding IDE port...");
            RunVBoxCommand($"storagectl \"{_name}\" --name \"IDE Controller\" --add ide --controller PIIX4");
            _logPanel.Rows.Add("IDE port added successfully!");
            // Wait For ISO Task
            isoTask.Wait();
            if (!string.IsNullOrEmpty(_ideRoute))
            {
                // Attach ISO
                _logPanel.Rows.Add("Installing IDE ISO...");
                RunVBoxCommand($"storageattach \"{_name}\" --storagectl \"IDE Controller\" --port 1 --device 0 --type dvddrive --medium {_ideRoute}");
                _logPanel.Rows.Add("IDE ISO installed successfully!");
            }
            else { _logPanel.Rows.Add("[Do Nothing] selected. No IDE will be installed."); }
            // Boot Settings
            _logPanel.Rows.Add("Configuring boot settings...");
            RunVBoxCommand($"modifyvm \"{_name}\" --boot1 dvd --boot2 disk --boot3 none --boot4 none");
            _logPanel.Rows.Add("Boot settings configured successfully!");
            // Start VM
            if (_startVM)
            {
                _logPanel.Rows.Add("Booting up VM...");
                RunVBoxCommand($"startvm \"{_name}\"");
                _logPanel.Rows.Add("VM booted successfully!");
            }
            _logPanel.Rows.Add("All done!");
        }
        #endregion
    }
}