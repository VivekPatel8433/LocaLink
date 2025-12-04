using ChatMessenger.Properties;
using System;
using System.Windows.Forms;

namespace ChatMessenger
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            this.Text = "LocaLink";
        }

        private void form1_Click(object sender, EventArgs e)
        {
            WelcomePage main = new WelcomePage();
            main.StartPosition = FormStartPosition.CenterScreen;
            main.Show();
            this.Hide();
        }
    }
}
