using System;
using System.Drawing;
using System.Windows.Forms;

namespace MuchosDicenDique
{
    public partial class Form1 : Form
    {
        AppManager manager;
        string vmPath = "";
        string vmDiskPath = "";
        string vmIdePath = "";
        string gfxController = "";
        public Form1()
        {
            manager = new AppManager();
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            Entrega1();
        }
        #region [Entrega 1]
        void Entrega1()
        {
            Label_IPHost.Text = manager.GetNetworkAddress();
            Label_IPGateway.Text = manager.GetNetworkGateway();
            Label_Username.Text = manager.GetCurrentUserName();
            Label_Hostname.Text = manager.GetCurrentHostName();
            string netName = manager.GetNetworkName();
            Label_SSIDName.Text = netName;
            if (netName == "---")
            {
                Label_SSIDStatus.Text = "Not Connected";
                Label_SSIDStatus.ForeColor = Color.Red;
            }
            else
            {
                Label_SSIDStatus.Text = $"Connected ({(manager.IsConnectedViaEthernet() ? "Ethernet" : "Wi-Fi")})";
                Label_SSIDStatus.ForeColor = Color.Green;
            }
            Label_MAC.Text = manager.GetMACAddress();
            switch (manager.PingHost())
            {
                case AppManager.PingResults.Failed:
                    Label_InternetStatus.Text = "Disconnected";
                    Label_InternetStatus.ForeColor = Color.Red;
                    break;
                case AppManager.PingResults.Unstable:
                    Label_InternetStatus.Text = "Unstable";
                    Label_InternetStatus.ForeColor = Color.Goldenrod;
                    break;
                case AppManager.PingResults.Success:
                    Label_InternetStatus.Text = "Connected";
                    Label_InternetStatus.ForeColor = Color.Green;
                    break;
                default:
                    Label_InternetStatus.Text = "---";
                    Label_InternetStatus.ForeColor = Color.DarkGray;
                    break;
            }
            string currentVersion = manager.GetCurrentVirtualBoxVersion();
            string lastVersion = manager.GetLastVirtualBoxVersion();
            if (currentVersion == "N/A")
            {
                Label_VBInstalled.Text = "Not Installed";
                Label_VBInstalled.ForeColor = Color.Red;
            }
            else
            {
                Label_VBInstalled.Text = "Installed";
                Label_VBInstalled.ForeColor = Color.Green;
            }
            Label_VBVersion.Text = $"{currentVersion} ({lastVersion})";
            Label_VBVersion.ForeColor = currentVersion == lastVersion ? Color.Green : currentVersion == "N/A" ? Color.Red : Color.Goldenrod;
        }
        #endregion
        #region [Entrega 2]
        private void ComboBox_NewVMOSType_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox_NewVMOSVersion.Items.Clear();
            switch (ComboBox_NewVMOSType.SelectedIndex)
            {
                // Microsoft Windows
                case 0:
                    ComboBox_NewVMOSVersion.Items.AddRange(new object[]
                    {
                        "Windows 3.1",
                        "Windows 95",
                        "Windows 98",
                        "Windows ME",
                        "Windows NT 3.x",
                        "Windows NT 4",
                        "Windows 2000",
                        "Windows XP (32-bit)",
                        "Windows XP (64-bit)",
                        "Windows 2003 (32-bit)",
                        "Windows 2003 (64-bit)",
                        "Windows Vista (32-bit)",
                        "Windows Vista (64-bit)",
                        "Windows 2008 (32-bit)",
                        "Windows 2008 (64-bit)",
                        "Windows 7 (32-bit)",
                        "Windows 7 (64-bit)",
                        "Windows 8 (32-bit)",
                        "Windows 8 (64-bit)",
                        "Windows 8.1 (32-bit)",
                        "Windows 8.1 (64-bit)",
                        "Windows 2012 (64-bit)",
                        "Windows 10 (32-bit)",
                        "Windows 10 (64-bit)",
                        "Windows 2016 (64-bit)",
                        "Windows 2019 (64-bit)",
                        "Windows 1 (64-bit)",
                        "Other Windows (32-bit)",
                        "Other Windows (64-bit)"
                    });
                    break;
                // Linux
                case 1:
                    ComboBox_NewVMOSVersion.Items.AddRange(new object[]
                    {
                        "Linux 2.2",
                        "Linux 2.4 (32-bit)",
                        "Linux 2.4 (64-bit)",
                        "Linux 2.6 / 3.x / 4.x (32-bit)",
                        "Linux 2.6 / 3.x / 4.x (64-bit)",
                        "Arch Linux (32-bit)",
                        "Arch Linux (64-bit)",
                        "Debian (32-bit)",
                        "Debian (64-bit)",
                        "Fedora (32-bit)",
                        "Fedora (64-bit)",
                        "Gentoo (32-bit)",
                        "Gentoo (64-bit)",
                        "Madriva (32-bit)",
                        "Madriva (64-bit)",
                        "Oracle (32-bit)",
                        "Oracle (64-bit)",
                        "Red Hat (32-bit)",
                        "Red Hat (64-bit)",
                        "openSUSE (32-bit)",
                        "openSUSE (64-bit)",
                        "Turbolinux (32-bit)",
                        "Turbolinux (64-bit)",
                        "Ubuntu (32-bit)",
                        "Ubuntu (64-bit)",
                        "Xandros (32-bit)",
                        "Xandros (64-bit)",
                        "Other Linux (32-bit)",
                        "Other Linux (64-bit)"
                    });
                    break;
                // Solaris
                case 2:
                    ComboBox_NewVMOSVersion.Items.AddRange(new object[]
                    {
                        "Oracle Solaris 10 5/09 and earlier (32-bit)",
                        "Oracle Solaris 10 5/09 and earlier (64-bit)",
                        "Oracle Solaris 10 10/09 and earlier (32-bit)",
                        "Oracle Solaris 10 10/09 and earlier (64-bit)",
                        "Oracle Solaris 11 (64-bit)",
                        "OpenSolaris / Illumos / OpenIndiana (32-bit)",
                        "OpenSolaris / Illumos / OpenIndiana (64-bit)"
                    });
                    break;
                // BSD
                case 3:
                    ComboBox_NewVMOSVersion.Items.AddRange(new object[]
                    {
                        "FreeBSD (32-bit)",
                        "FreeBSD (64-bit)",
                        "OpenBSD (32-bit)",
                        "OpenBSD (64-bit)",
                        "NetBSD (32-bit)",
                        "NetBSD (64-bit)"
                    });
                    break;
                // IBM OS/2
                case 4:
                    ComboBox_NewVMOSVersion.Items.AddRange(new object[]
                    {
                        "OS/2 1.x",
                        "OS/2 Warp 3",
                        "OS/2 Warp 4",
                        "OS/2 Warp 4.5",
                        "eComStation",
                        "ArcaOS",
                        "Other OS/2"
                    });
                    break;
                // Mac OS X
                case 5:
                    ComboBox_NewVMOSVersion.Items.AddRange(new object[]
                    {
                        "Mac OS X (32-bit)",
                        "Mac OS X (64-bit)",
                        "Mac OS X 10.6 Snow Leopard (32-bit)",
                        "Mac OS X 10.6 Snow Leopard (64-bit)",
                        "Mac OS X 10.7 Lion (64-bit)",
                        "Mac OS X 10.8 Mountain Lion (64-bit)",
                        "Mac OS X 10.9 Mavericks (64-bit)",
                        "Mac OS X 10.10 Yosemite (64-bit)",
                        "Mac OS X 10.11 El Capitan (64-bit)",
                        "macOS 10.12 Sierra (64-bit)",
                        "macOS 10.13 High Sierra (64-bit)"
                    });
                    break;
                // Other
                default:
                    ComboBox_NewVMOSVersion.Items.AddRange(new object[]
                    {
                        "DOS",
                        "Netware",
                        "L4",
                        "QNX",
                        "JRockitVE",
                        "VirtualBox Bootsector Test (64-bit)",
                        "Other/Unknown",
                        "Other/Unknow (64-bit)"
                    });
                    break;
            }
        }
        string TruncatePath(string _path) { return _path.Length > 24 ? _path.Substring(0, 21) + "..." : _path; }
        private void Button_SelectVMLocation_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();
            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                vmPath = folderDialog.SelectedPath;
                Button_SelectVMLocation.Text = TruncatePath(vmPath);
            }
        }
        private void Button_SelectVMDisk_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                Console.WriteLine(fileDialog.FileName);
                vmDiskPath = fileDialog.FileName;
                Button_SelectVMDisk.Text = TruncatePath(vmDiskPath);
            }
        }
        private void Button_SelectExistingIDE_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "ISO Disk Files|*.iso";
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                Console.WriteLine(fileDialog.FileName);
                vmIdePath = fileDialog.FileName;
                Button_SelectExistingIDE.Text = TruncatePath(vmIdePath);
            }
        }
        private void ComboBox_NewVMGraphicsController_SelectedIndexChanged(object sender, EventArgs e)
        {
            gfxController = ComboBox_NewVMGraphicsController.SelectedText == "None" ? "null" : ComboBox_NewVMGraphicsController.SelectedText.ToLower();
        }
        private void Button_CreateVM_Click(object sender, EventArgs e)
        {
            manager.CreateVM
            (
                // Base Route
                vmPath,
                // Name
                TextBox_NewVMName.Text,
                // OS Type
                ComboBox_NewVMOSType.SelectedText,
                // Virtual Disk Memory
                RadioButton_CreateNewDisk.Checked ? (int)NumericUpDown_CreateVirtualDiskSize.Value : -1,
                // Virtual Disk Type
                ComboBox_CreateNewDiskFormat.SelectedText,
                // Virtual Disk Route
                RadioButton_UseExistingDisk.Checked ? vmDiskPath : "",
                // IDE Route
                RadioButton_UseExistingIDE.Checked ? vmIdePath : "",
                // IDE Download OS
                RadioButton_DownloadLatestIDE.Checked ? ComboBox_DownloadIDEType.SelectedText : "",
                // RAM Memory
                (int)NumericUpDown_RAMMemory.Value,
                // CPU Cores
                (int)NumericUpDown_CPUCores.Value,
                // GFX Controller
                gfxController,
                // Video Memory
                (int)NumericUpDown_VideoMemory.Value,
                // Start Up VM
                CheckBox_StartVM.Checked
            );
        }
        #endregion
    }
}