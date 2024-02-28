using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace IT_starter_pack
{
    public partial class about_form : Form
    {
        public about_form()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://www.linkedin.com/in/karanpiprani");
        }
    }
}
