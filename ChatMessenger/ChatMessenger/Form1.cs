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

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            RandomChat main = new RandomChat();
            main.StartPosition = FormStartPosition.CenterScreen;
            main.Show();
            this.Hide();
        }
    }
}
