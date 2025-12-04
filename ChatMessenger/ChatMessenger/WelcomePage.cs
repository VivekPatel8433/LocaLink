using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChatMessenger.Properties
{
    public partial class WelcomePage : Form
    {
        Panel PLeft;
        Panel PRight;

        public WelcomePage()
        {
            InitializeComponent();
            BuildUI();
        }

        private void BuildUI()
        {
            this.Text = "Welcome";
            this.BackColor = ColorTranslator.FromHtml("#F5F7FA");
            this.Size = new Size(1400, 800);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;

            // LEFT PANEL
            PLeft = new Panel()
            {
                Dock = DockStyle.Left,
                Width = 600,
                BackColor = Color.DarkBlue,
                Padding = new Padding(15, 20, 15, 20)
            };
            this.Controls.Add(PLeft);
            BuildLeftSidebar();


            // RIGHT PANEL
            PRight = new Panel()
            {
                Dock = DockStyle.Right,
                Width = 800,
                BackColor = Color.White,
                Padding = new Padding(20, 20, 20, 20)
            };
            this.Controls.Add(PRight);
            int rightpanelheight = 50;
            BuildRightSidebar(ref rightpanelheight);
        }

        public void BuildLeftSidebar()
        {

        }

        Label title;
        Label subTitle;
        Label labelUsername;
        TextBox username;
        Label labelPhoneNumber;
        TextBox phoneNumber;
        Label sublabelPhone;
        Label labelUsermode;
        RadioButton customUser;
        RadioButton anonymousUser;
        public void BuildRightSidebar(ref int y)
        {

            //Title
            title = new Label()
            {
                Text = "Welcome",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(200, y)
            };
            subTitle = new Label()
            {
                Text = "Join Your Local Chat",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.Gray,
                AutoSize = true,
                Location = new Point(200, y + 50)
            };

            //Login/Signup
            labelUsername = new Label()
            {
                Text = "Username",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(200, y + 120)
            };
            username = new TextBox()
            {
                PlaceholderText = "Enter Your Username",
                Font = new Font("Segoe UI", 10),
                AutoSize = true,
                Size = new Size(400, 10),
                Location = new Point(200, y + 145),
                IsAccessible = true,
                AccessibleName = "Username_textbox"
            };

            labelPhoneNumber = new Label()
            {
                Text = "Phone Number (optional)",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(200, y + 190)
            };
            phoneNumber = new TextBox()
            {
                PlaceholderText = "(123)456-7890",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.Gray,
                AutoSize = true,
                Size = new Size(400, 10),
                Location = new Point(200, y + 215),
                MaxLength = 10
            };
            sublabelPhone = new Label()
            {
                Text = "Register with mobile for persisent data",
                Font = new Font("Segoe UI", 5, FontStyle.Bold),
                ForeColor = Color.Gray,
                AutoSize = true,
                Location = new Point(200, y + 250)
            };

            //user join mode
            labelUsermode = new Label()
            {
                Text = "Join Mode",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(200, y + 300)
            };
            customUser = new RadioButton()
            {
                Text = "Custom User",
                Font = new Font("Segoe UI", 10),
                AutoSize = true,
                Size = new Size(400, 10),
                Location = new Point(200, y + 330),
                Checked = true

            };
            customUser.CheckedChanged += (sender, args) =>
            {
                username.Enabled = true;
                phoneNumber.Enabled = true;
            };

            anonymousUser = new RadioButton()
            {
                Text = "Anonymous User",
                Font = new Font("Segoe UI", 10),
                AutoSize = true,
                Size = new Size(400, 10),
                Location = new Point(200, y + 360),
            };
            anonymousUser.CheckedChanged += (sender, args) =>
            {
                username.Enabled = false;
                phoneNumber.Enabled = false;
            };

            PRight.Controls.Add(title);
            PRight.Controls.Add(subTitle);
            PRight.Controls.Add(labelUsername);
            PRight.Controls.Add(username);
            PRight.Controls.Add(labelPhoneNumber);
            PRight.Controls.Add(phoneNumber);
            PRight.Controls.Add(sublabelPhone);
            PRight.Controls.Add(labelUsermode);
            PRight.Controls.Add(customUser);
            PRight.Controls.Add(anonymousUser);

        }

        private void customUser_CheckedChanged(object sender, EventArgs e)
        {

        }

    }
}
