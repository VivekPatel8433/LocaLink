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
            Form2 main = new Form2();
            main.StartPosition = FormStartPosition.CenterScreen;
            main.Show();
            this.Hide();
        }
    }
}
