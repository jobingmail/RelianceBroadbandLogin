using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.NetworkInformation;
using System.Net;
using System.IO;

namespace RelianceBBLogin
{
    public partial class MainForm : Form
    {
        private System.Windows.Forms.WebBrowser WebBrowser1;

        private object _lockObject = new object();

        bool Notify
        {
            get
            {
                return chkNotify.Checked;
            }

            set
            {
                if (chkNotify.Checked != value)
                {
                    AppConfig.Notify = value.ToString();
                }
                
                
            }
        }

        public MainForm()
        {
            InitializeComponent();

            this.WebBrowser1 = new System.Windows.Forms.WebBrowser();
            this.WebBrowser1.NewWindow += WebBrowser1_NewWindow;

            timer1.Enabled = true;

            try
            {
                var secs = int.Parse(AppConfig.TimerIntervalSec);
                timer1.Interval = secs * 1000;
            }
            catch(Exception)
            {
                timer1.Interval = 60000;
            }

            //timer1.Start();

        }

        private void WebBrowser1_NewWindow(object sender, CancelEventArgs e)
        {
            WebBrowser1.Hide();
        }

        private async void MainForm_Load(object sender, EventArgs e)
        {
            this.Icon = Properties.Resources.reliance;
            NotifyIcon1.Icon = Properties.Resources.reliance;

            bool temp;
            if(bool.TryParse(AppConfig.Notify,out temp))
            {
                chkNotify.Checked = temp;
            }

            this.chkNotify.CheckedChanged += new System.EventHandler(this.chkNotify_CheckedChanged);

            this.Hide();

            try
            {
                await RelianceBBLogin();
            }
            catch
            {

            }
        }

        

        async Task RelianceBBLogin()
        {

            lock (_lockObject)
            {

                try
                {

                    if (LoginHelper.IsLAN_Available()) //checking Lan connection
                    {



                        if (LoginHelper.PingHost(AppConfig.DomainIPName)) //ping google
                        {
                            UpdateStatus(ConnectionStatus.Connected);


                        }
                        else //Loggin in
                        {



                            this.WebBrowser1.Navigate(AppConfig.LoginURL, false);

                            while (this.WebBrowser1.ReadyState != WebBrowserReadyState.Complete)
                            {
                                Application.DoEvents();
                            }

                            var userid = this.WebBrowser1.Document.GetElementById("userId");

                            if (userid != null)
                            {
                                this.WebBrowser1.Document.GetElementById("userId").SetAttribute("value", AppConfig.UserId);
                                this.WebBrowser1.Document.GetElementById("password").SetAttribute("value", AppConfig.Password);

                                HtmlElementCollection elementsByTagName = this.WebBrowser1.Document.GetElementsByTagName("form");
                                if (elementsByTagName.Count > 0)
                                {
                                    elementsByTagName[0].InvokeMember("submit");
                                }

                                while (this.WebBrowser1.ReadyState != WebBrowserReadyState.Complete)
                                {
                                    Application.DoEvents();
                                }


                            }
                            else
                            {
                                this.WebBrowser1.Navigate(AppConfig.LoginURL,false);
                            }

                            //setting status

                            if (LoginHelper.PingHost(AppConfig.DomainIPName))
                            {
                                UpdateStatus(ConnectionStatus.Connected);
                            }
                            else
                            {
                                if (LoginHelper.IsLAN_Available() == false)
                                {

                                    UpdateStatus(ConnectionStatus.NoNetwork);

                                }
                                else
                                {
                                    UpdateStatus(ConnectionStatus.Disconnected);
                                }
                            }



                        }
                    }
                    else
                    {
                        UpdateStatus(ConnectionStatus.NoNetwork);

                    }

                }
                catch (Exception)
                {

                }
            }
        }



        private void UpdateStatus(ConnectionStatus status)
        {
            if (status == ConnectionStatus.Connected)
            {
                this.lblStatus.Text = "You are connected";
                if (Notify)
                {
                    this.NotifyIcon1.BalloonTipText = "You are connected";
                    this.NotifyIcon1.ShowBalloonTip(10);
                }
            }
            else if (status == ConnectionStatus.Disconnected)
            {
                this.lblStatus.Text = "You are NOT connected";
                if (Notify)
                {
                    this.NotifyIcon1.BalloonTipText = "Internet Disconnected";
                    this.NotifyIcon1.ShowBalloonTip(10);
                }
            }
            else if (status == ConnectionStatus.NoNetwork)
            {
                this.lblStatus.Text = "Please Check your Ethernet Connection; Network Unvailable";
                if (Notify)
                {
                    this.NotifyIcon1.BalloonTipText = "Please Check your Ethernet Connection";
                    this.NotifyIcon1.ShowBalloonTip(10);
                }
            }
            else if (status == ConnectionStatus.Error)
            {
                this.lblStatus.Text = "Connection Error";
                if (Notify)
                {
                    this.NotifyIcon1.BalloonTipText = "Connection Error";
                    this.NotifyIcon1.ShowBalloonTip(10);
                }
            }
        }


        private void NotifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            

            if (e.CloseReason != CloseReason.ApplicationExitCall)
            {
                //this.WindowState = FormWindowState.Minimized;
                this.Hide();
                e.Cancel = true;
            }

        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void showWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Show();
        }

        private async void timer1_Tick(object sender, EventArgs e)
        {
            
                await RelianceBBLogin();
                
          
        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            
                await RelianceBBLogin();
            
        }


        private void chkNotify_CheckedChanged(object sender, EventArgs e)
        {
            Notify = chkNotify.Checked;
        }

        
        private void chkRunStart_CheckedChanged(object sender, EventArgs e)
        {
            LoginHelper.SetStartup("RelianceLogin", Application.StartupPath, chkRunStart.Checked);
        }

        
    }
}
