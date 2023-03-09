using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
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
            Label_MAC.Text = manager.GetMACAddress();
            manager.GetCurrentVirtualBoxVersion();
            if (manager.GetCurrentVirtualBoxVersion() != "N/A")
            {
                Label_VBInstalled.Text = "Yes";
            }
            else
            {
                Label_VBInstalled.Text = "No";
            }
            Console.WriteLine(manager.GetWifiSsid());
        }
    }
}