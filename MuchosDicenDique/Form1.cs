using System;
using System.Drawing;
using System.Windows.Forms;

namespace MuchosDicenDique
{
    public partial class Form1 : Form
    {
        AppManager manager;
        string osType = "";
        string vmPath = "";
        string vmDiskPath = "";
        string vmIdePath = "";
        string gfxController = "";
        string netController = "";
        public Form1()
        {
            manager = new AppManager();
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            Entrega1();
            ComboBox_NewVMOSType.SelectedIndex = 6;
            ComboBox_NewVMOSVersion.SelectedIndex = 6;
            ComboBox_CreateNewDiskFormat.SelectedIndex = 0;
            ComboBox_NewVMGraphicsController.SelectedIndex = 1;
            ComboBox_NewVMNetController.SelectedIndex = 1;
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
                        "Windows 11 (64-bit)",
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
        private void ComboBox_NewVMOSVersion_SelectedIndexChanged(object sender, EventArgs e)
        {
            string[] osVersionArr = new string[] { };
            switch (ComboBox_NewVMOSType.SelectedIndex)
            {
                // Microsoft Windows
                case 0:
                    osVersionArr = new string[]
                    {
                        "Windows31",
                        "Windows95",
                        "Windows98",
                        "WindowsMe",
                        "WindowsNT3x",
                        "WindowsNT4",
                        "Windows2000",
                        "WindowsXP",
                        "WindowsXP_64",
                        "Windows2003",
                        "Windows2003_64",
                        "WindowsVista",
                        "WindowsVista_64",
                        "Windows2008",
                        "Windows2008_64",
                        "Windows7",
                        "Windows7_64",
                        "Windows8",
                        "Windows8_64",
                        "Windows81",
                        "Windows81_64",
                        "Windows2012_64",
                        "Windows10",
                        "Windows10_64",
                        "Windows2016_64",
                        "Windows2019_64",
                        "Windows11_64",
                        "WindowsNT",
                        "WindowsNT_64"
                    };
                    break;
                // Linux
                case 1:
                    osVersionArr = new string[]
                    {
                        "Linux22",
                        "Linux24",
                        "Linux24_64",
                        "Linux26",
                        "Linux26_64",
                        "ArchLinux",
                        "ArchLinux_64",
                        "Debian",
                        "Debian_64",
                        "Fedora",
                        "Fedora_64",
                        "Gentoo",
                        "Gentoo_64",
                        "Mandriva",
                        "Mandriva_64",
                        "Oracle",
                        "Oracle_64",
                        "RedHat",
                        "RedHat_64",
                        "OpenSUSE",
                        "OpenSUSE_64",
                        "Turbolinux",
                        "Turbolinux_64",
                        "Ubuntu",
                        "Ubuntu_64",
                        "Xandros",
                        "Xandros_64",
                        "Linux",
                        "Linux_64"
                    };
                    break;
                // Solaris
                case 2:
                    osVersionArr = new string[]
                    {
                        "Solaris",
                        "Solaris_64",
                        "Solaris10U8_or_later",
                        "Solaris10U8_or_later_64",
                        "Solaris11_64",
                        "OpenSolaris",
                        "OpenSolaris_64"
                    };
                    break;
                // BSD
                case 3:
                    osVersionArr = new string[]
                    {
                        "FreeBSD",
                        "FreeBSD_64",
                        "OpenBSD",
                        "OpenBSD_64",
                        "NetBSD",
                        "NetBSD_64"
                    };
                    break;
                // IBM OS/2
                case 4:
                    osVersionArr = new string[]
                    {
                        "OS21x",
                        "OS2Warp3",
                        "OS2Warp4",
                        "OS2Warp45",
                        "OS2eCS",
                        "OS2ArcaOS",
                        "OS2"
                    };
                    break;
                // Mac OS X
                case 5:
                    osVersionArr = new string[]
                    {
                        "MacOS",
                        "MacOS_64",
                        "MacOS106",
                        "MacOS106_64",
                        "MacOS107_64",
                        "MacOS108_64",
                        "MacOS109_64",
                        "MacOS1010_64",
                        "MacOS1011_64",
                        "MacOS1012_64",
                        "MacOS1013_64"
                    };
                    break;
                // Other
                default:
                    osVersionArr = new string[]
                    {
                        "DOS",
                        "Netware",
                        "L4",
                        "QNX",
                        "JRockitVE",
                        "VBoxBS_64",
                        "Other",
                        "Other_64"
                    };
                    break;
            }
            osType = osVersionArr[ComboBox_NewVMOSVersion.SelectedIndex];
        }
        string GetComboBoxSelectedText(ComboBox _cbox, string _defVal = "") { return _cbox.SelectedItem is null ? _defVal : _cbox.SelectedItem.ToString(); }
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
            string selection = GetComboBoxSelectedText(ComboBox_NewVMGraphicsController);
            gfxController = selection == "None" ? "null" : selection.ToLower();
        }
        private void ComboBox_NewVMNetController_SelectedIndexChanged(object sender, EventArgs e)
        {
            netController = new string[] { "null", "nat", "bridged", "intnet", "hostonly", "generic", "natnetwork" }[ComboBox_NewVMNetController.SelectedIndex];
        }
        private void Button_CreateVM_Click(object sender, EventArgs e)
        {
            Button_CreateVM.Enabled = false;
            manager.CreateVM
            (
                // Base Route
                vmPath,
                // Name
                TextBox_NewVMName.Text,
                // OS Type
                osType,
                // Virtual Disk Memory
                RadioButton_CreateNewDisk.Checked ? (int)NumericUpDown_CreateVirtualDiskSize.Value : -1,
                // Virtual Disk Type
                GetComboBoxSelectedText(ComboBox_CreateNewDiskFormat),
                // Virtual Disk Route
                RadioButton_UseExistingDisk.Checked ? vmDiskPath : "",
                // IDE Route
                RadioButton_UseExistingIDE.Checked ? vmIdePath : "",
                // IDE Download OS
                RadioButton_DownloadLatestIDE.Checked ? GetComboBoxSelectedText(ComboBox_DownloadIDEType) : "",
                // RAM Memory
                (int)NumericUpDown_RAMMemory.Value,
                // CPU Cores
                (int)NumericUpDown_CPUCores.Value,
                // GFX Controller
                gfxController,
                // Video Memory
                (int)NumericUpDown_VideoMemory.Value,
                // Net Controller
                netController,
                // Start Up VM
                CheckBox_StartVM.Checked,
                // Log Panel Reference
                DataGridView_LogPanel
            );
            Button_CreateVM.Enabled = true;
        }
        private void tabPage2_Enter(object sender, EventArgs e)
        {
            manager.DisplayVMList(DataGridView_VMsList);
        }
        #endregion
    }
}