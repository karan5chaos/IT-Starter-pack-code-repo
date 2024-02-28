using IT_starter_pack;
using IT_Starter_pack.Properties;
using LibGit2Sharp;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows.Forms;
using Application = System.Windows.Forms.Application;
using FileMode = System.IO.FileMode;

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
        }


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

        private void Form1_Load(object sender, EventArgs e)
        {
            comboBox1.Items.Clear();
            update_repos();
        }


        public void DeleteReadOnlyDirectory(string directory)
        {
            try
            {
                dpath = Application.StartupPath + "/apps";
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

        string dpath = Application.StartupPath + "/apps";

        void Download_installAsync(string link, string fname)
        {
            try
            {
                dpath = Application.StartupPath + "/apps";
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

            dpath = Application.StartupPath + "/apps";
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
            //backgroundWorker1.RunWorkerAsync();
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
    }
}
