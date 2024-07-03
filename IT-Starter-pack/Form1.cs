using IT_starter_pack;
using IT_Starter_pack.Properties;
using LibGit2Sharp;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using Application = System.Windows.Forms.Application;
using FileMode = System.IO.FileMode;
using ManagedNativeWifi;
using System.Linq;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using Microsoft.VisualBasic.ApplicationServices;
using System.Management.Automation;
using CliWrap;
using CliWrap.Buffered;
using System.Web;
using static System.Windows.Forms.LinkLabel;
using System.Windows.Forms.VisualStyles;
using System.Net.Http.Headers;
using Octokit;
using Repository = LibGit2Sharp.Repository;


namespace IT_Starter_pack
{
    public partial class Form1 : Form
    {
        public Form1()
        {



            InitializeComponent();



            progressBar1.Width = comboBox1.Width;
            progressBar1.Height = comboBox1.Height;
            progressBar1.Location = comboBox1.Location;
            progressBar1.Size = comboBox1.Size;


            typeof(DataGridView).InvokeMember("DoubleBuffered", BindingFlags.NonPublic |
            BindingFlags.Instance | BindingFlags.SetProperty, null,
            dataGridView1, new object[] { true });

        }

        string dpath = Application.StartupPath + "/downloads";




        public class ComboboxItem
        {
            public string Text { get; set; }
            public object Value { get; set; }
            public object Link { get; set; }
            public object fname { get; set; }

            public override string ToString()
            {
                return Text;
            }
        }

        void showprogressbar(bool activate)
        {
            if (activate)
            {
                comboBox1.Visible = false;
                label3.Text = "Downloading";
            }
            else
            {
                comboBox1.Visible = true;
                label3.Text = "Select Software:";
            }
            progressBar1.Visible = activate;
            progressBar1.Value = 0;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            groupBox2.Text = (comboBox1.SelectedItem as ComboboxItem).Text.ToString();
            textBox1.Text = (comboBox1.SelectedItem as ComboboxItem).Value.ToString();
        }


        Bitmap printer1;
        Bitmap printer2;

        private void Form1_Load(object sender, EventArgs e)
        {
            check_for_updatesAsync();
            comboBox1.Items.Clear();
            update_repos();
            timer1.Start();

            dataGridView1.Rows.Add(printer1, "Konica Minolta (Colour printer)", "192.168.10.112");
            dataGridView1.Rows.Add(printer2, "Kyocera (B/W printer)", "192.170.80.251");

        }


        public void DeleteReadOnlyDirectory(string directory)
        {
            try
            {
                dpath = Application.StartupPath + "/downloads";
                foreach (var subdirectory in Directory.EnumerateDirectories(directory))
                {
                    DeleteReadOnlyDirectory(subdirectory);
                }
                foreach (var fileName in Directory.EnumerateFiles(directory))
                {
                    var fileInfo = new FileInfo(fileName);
                    fileInfo.Attributes = FileAttributes.Normal;
                    fileInfo.Delete();
                }
                Directory.Delete(directory);
            }
            catch { }
        }

        void update_repos()
        {
            try
            {
                if (Directory.Exists(dpath))
                {
                    DeleteReadOnlyDirectory(dpath);
                    //Directory.Delete(dpath, true);
                }

                Repository.Clone("https://github.com/karan5chaos/IT_starter_pack", dpath);

                var repo = File.ReadAllLines(dpath + "/repo.txt");


                foreach (string line in repo)
                {
                    string application = line;
                    string[] application_info = application.Split(';');

                    string appname = application.Split(';')[0];
                    string description = application.Split(';')[1];
                    string link = application.Split(';')[2];
                    string fname = application.Split(';')[3];

                    addtocombobox(appname, description, link, fname);

                }
            }
            catch { }
        }

        private void Wc_DownloadProgressChanged1(object sender, DownloadProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        ComboboxItem item;
        void addtocombobox(string text, string value, string link, string fname)
        {
            item = new ComboboxItem();
            item.Text = text;
            item.Value = value;
            item.Link = link;
            item.fname = fname;

            comboBox1.Items.Add(item);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Download_installAsync((comboBox1.SelectedItem as ComboboxItem).Link.ToString(), (comboBox1.SelectedItem as ComboboxItem).fname.ToString());
            showprogressbar(true);
        }

        void wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        

        void Download_installAsync(string link, string fname)
        {
            try
            {
                dpath = Application.StartupPath + "/downloads";
                button1.Enabled = false;

                using (WebClient wc = new WebClient())
                {
                    wc.DownloadProgressChanged += wc_DownloadProgressChanged;

                    toolStripStatusLabel1.Text = (comboBox1.SelectedItem as dynamic).Text + " Downloading. Please Wait..";
                    wc.DownloadFileAsync(
                    // Param1 = Link of file
                    new System.Uri(link), dpath += "/" + fname
                    // Param2 = Path to save
                    );

                    wc.DownloadFileCompleted += Wc_DownloadFileCompleted;
                }
            }
            catch { }
        }

        private void Wc_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            try
            {
                toolStripStatusLabel1.Text = (comboBox1.SelectedItem as dynamic).Text + " Downloaded";
                button1.Enabled = true;
                Process.Start(dpath);
                showprogressbar(false);
            }
            catch { }
        }

        private string writeresource(string tempExeName, byte[] resource)
        {

            try
            {
                if (!Directory.Exists(dpath))
                {
                    Directory.CreateDirectory(dpath);
                }
                if (File.Exists(tempExeName))
                {
                    File.Delete(tempExeName);
                }
                using (FileStream fsDst = new FileStream(tempExeName, FileMode.CreateNew, FileAccess.Write))
                {
                    byte[] bytes = resource;
                    fsDst.Write(bytes, 0, bytes.Length);
                }
                return tempExeName;
            }
            catch
            {
                return tempExeName;
            }
        }

        private void microsoftOfficeActivationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                string tempExeName = Path.Combine(dpath, "MAS_AIO.cmd");
                Process.Start(writeresource(tempExeName, Resources.MAS_AIO), "/Ohook");
            }
            catch { }
        }

        private void mincrosoftWindowsActivationToolStripMenuItem_Click(object sender, EventArgs e)
        {

            dpath = Application.StartupPath + "/downloads";
            try
            {
                string tempExeName = Path.Combine(dpath, "MAS_AIO.cmd");
                Process.Start(writeresource(tempExeName, Resources.MAS_AIO), "/HWID");
            }
            catch { }
        }

        private void updateRepositoriesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            update_repos();
        }


        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            DeleteReadOnlyDirectory(dpath);
            // Directory.Delete(dpath, true);
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            about_form about_Form = new about_form();
            about_Form.ShowDialog();
        }

        public static bool PingHost(string nameOrAddress)
        {
            bool pingable = false;
            Ping pinger = null;

            try
            {
                pinger = new Ping();
                PingReply reply = pinger.Send(nameOrAddress);
                pingable = reply.Status == IPStatus.Success;
            }
            catch (PingException)
            {
            }
            finally
            {
                if (pinger != null)
                {
                    pinger.Dispose();
                }
            }

            return pingable;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!backgroundWorker1.IsBusy)
            {
                backgroundWorker1.RunWorkerAsync();
            }

            dataGridView1.Rows[0].Cells[0].Value = printer1;
            dataGridView1.Rows[1].Cells[0].Value = printer2;
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            if (PingHost("192.168.10.112"))
            {
                printer1 = Resources.bullet_green;

            }
            else
            {
                printer1 = Resources.bullet_red;

            }

            if (PingHost("192.170.80.251"))
            {
                printer2 = Resources.bullet_green;

            }
            else
            {
                printer2 = Resources.bullet_red;

            }



        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            dataGridView1.ClearSelection();
        }

        private void launchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                string tempExeName = Path.Combine(dpath, "MAS_AIO.cmd");
                Process.Start(writeresource(tempExeName, Resources.MAS_AIO));
            }
            catch { }
        }

        private void cafeteriaToolsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        public static async Task<bool> ConnectAsync()
        {
            var availableNetwork = NativeWifi.EnumerateAvailableNetworks()
                .Where(x => !string.IsNullOrWhiteSpace(x.ProfileName))
                .OrderByDescending(x => x.SignalQuality)
                .FirstOrDefault();

            if (availableNetwork is null)
                return false;

            return await NativeWifi.ConnectNetworkAsync(
                interfaceId: availableNetwork.Interface.Id,
                profileName: availableNetwork.ProfileName,
                bssType: availableNetwork.BssType,
                timeout: TimeSpan.FromSeconds(10));
        }

        private void launchMASAIOPowershellToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var startInfo = new ProcessStartInfo()
            {
                CreateNoWindow = true,
                FileName = "powershell.exe",
                Arguments = $"irm https://massgrave.dev/get | iex",
                UseShellExecute = false
            };
            Process.Start(startInfo);
        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChangedAsync(object sender, EventArgs e)
        {
          installwsl();
        }

        bool check_WSLinstall()
        {
            bool val=false;
            var checkWSL = new ProcessStartInfo()
            {
                RedirectStandardOutput = true,
                CreateNoWindow = true,
                FileName = "powershell.exe",
                Arguments = $"wsl --version",
                UseShellExecute = false
            };

            var proc = Process.Start(checkWSL);
            proc.OutputDataReceived += (sender, e) =>
            {
                if (e.Data != "" || e.Data != null)
                {
                    val = true;
                }
                else
                { val = false; }
            };
            return val;
        }

        void installwsl()
        {
            if (checkBox1.Checked)
            {

                PowerShell ps = PowerShell.Create();
                
                ps.AddCommand("wsl");
                ps.AddArgument("--install");
                ps.InvocationStateChanged += Ps_InvocationStateChanged;
                ps.Invoke();

            }

        }

        
        private void Ps_InvocationStateChanged(object sender, PSInvocationStateChangedEventArgs e)
        {
            
            string txt = "Enable Windows Subsystem for Linux";
            checkBox1.Text = txt;
            if (e.InvocationStateInfo.State == PSInvocationState.Completed)
            {
                checkBox1.Text = txt + " - Installed";
                progressBar2.Visible = false;
                Properties.Settings.Default.wslinstalled = true;
                Properties.Settings.Default.Save();
                Properties.Settings.Default.Reload();
               // textBox2.Text = "WSL Installed successfully";

            }

            if (e.InvocationStateInfo.State == PSInvocationState.Running)
            {
                checkBox1.Text = txt + " - Installing";
                progressBar2.Visible = true;
                // textBox2.Text = "WSL Installed successfully";

            }

        }

        private void Verbose_DataAdded1(object sender, DataAddedEventArgs e)
        {

        }


        private void tabPage4_Click(object sender, EventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked && Properties.Settings.Default.wslinstalled==false) { installwsl(); }
            
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void checkForUpdatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            check_for_updatesAsync();
        }

        string update_file_setup = "";
        async Task check_for_updatesAsync()
        {

            try
            {

                dpath = Application.StartupPath + "/downloads";

                var client = new GitHubClient(new Octokit.ProductHeaderValue("IT-Starter-pack-code-repo"));

                var releases = await client.Repository.Release.GetAll("karan5chaos", "IT-Starter-pack-code-repo");
                var latest = releases[0];

                if (Properties.Settings.Default.version < Convert.ToDouble(latest.TagName))
                {
                    if (MessageBox.Show("A new update is available for download!\nUpdate now??", "Update available", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                    {

                        checkForUpdatesToolStripMenuItem.Visible = true;
                        WebClient webClient = new WebClient();
                        WebClient webClient2 = new WebClient();

                        var downlink0 = latest.Assets[0].BrowserDownloadUrl;

                        webClient.DownloadFileCompleted += WebClient_DownloadFileCompleted;


                        update_file_setup = dpath + "/" + latest.Assets[0].Name;


                        if (File.Exists(update_file_setup))
                        {
                            File.Delete(update_file_setup);
                        }

                        await webClient.DownloadFileTaskAsync(new Uri(downlink0), update_file_setup);
                    }
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show("Error while downloading update..\n" + ex.Message);
            }
        }

        private void WebClient_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (MessageBox.Show("The application will now close.", "App updater", MessageBoxButtons.OK, MessageBoxIcon.Information) == DialogResult.OK)
            {
                Process.Start(update_file_setup);
                this.Close();
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://docs.google.com/document/d/1IpRzD-yTzItoDIWJ9MPTgkKOlxoR8ADo6HrgYe-8iBE/edit?usp=sharing");
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://next.raiseaticket.com");
        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://next.raiseaticket.com/#/resources/faq/list");
        }

        private void linkLabel4_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://next.raiseaticket.com/#/resources/articles/list");
        }
    }


}


