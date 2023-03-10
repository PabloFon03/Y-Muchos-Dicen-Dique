using System;
using System.Drawing;
using System.Windows.Forms;

namespace MuchosDicenDique
{
    public partial class Form1 : Form
    {
        AppManager manager;
        public Form1()
        {
            manager = new AppManager();
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
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
    }
}