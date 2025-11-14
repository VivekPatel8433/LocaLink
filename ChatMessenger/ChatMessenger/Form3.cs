using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChatMessenger
{
    public partial class Form3 : Form
    {
        Panel panelLeft;
        Panel panelMain;
        Panel panelRight;

        public Form3()
        {
            InitializeComponent();
            BuildUI();
            this.Text = "Form3";
            this.Size = new Size(800, 600);
        }

        private void BuildUI()
        {

        }

        private void Form3_Load(object sender, EventArgs e)
        {

        }
    }
}
