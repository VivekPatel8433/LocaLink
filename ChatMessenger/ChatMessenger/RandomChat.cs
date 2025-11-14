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
    public partial class RandomChat : Form
    {

        Panel panelLeft;
        Panel panelMain;
        Panel panelRight;

        public RandomChat()
        {
            InitializeComponent();
            BuildUI();
        }

        private void BuildUI()
        {
            this.Text = "RandomChat";
            this.BackColor = ColorTranslator.FromHtml("#F5F7FA");
            this.Size = new Size(1400, 800);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;

            // LEFT PANEL
            panelLeft = new Panel();
            panelLeft.Dock = DockStyle.Left;
            panelLeft.Width = 230;
            panelLeft.BackColor = Color.White;
            panelLeft.Padding = new Padding(15, 20, 15, 20);
            this.Controls.Add(panelLeft);

            BuildLeftSidebar();

            // RIGHT PANEL
            panelRight = new Panel();
            panelRight.Dock = DockStyle.Right;
            panelRight.Width = 270;
            panelRight.BackColor = Color.White;
            panelRight.Padding = new Padding(20, 20, 20, 20);
            this.Controls.Add(panelRight);

            BuildRightSidebar();

            // MAIN PANEL
            panelMain = new Panel();
            panelMain.Dock = DockStyle.Fill;
            panelMain.BackColor = ColorTranslator.FromHtml("#F5F7FA");
            this.Controls.Add(panelMain);

            BuildMainArea();
        }

        private void BuildLeftSidebar()
        {

        }

        private void BuildRightSidebar()
        {

        }

        private void BuildMainArea()
        {

        }
    }

}
