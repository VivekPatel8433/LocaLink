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
                BackColor = Color.MidnightBlue,
                Padding = new Padding(15, 20, 15, 20)
            };
            this.Controls.Add(PLeft);
            int leftpanelheight = 150;
            BuildLeftSidebar(ref leftpanelheight);


            // RIGHT PANEL
            PRight = new Panel()
            {
                Dock = DockStyle.Right,
                Width = 800,
                BackColor = Color.White,
                Padding = new Padding(20, 20, 20, 20)
            };
            this.Controls.Add(PRight);
            int rightpanelheight = 100;
            BuildRightSidebar(ref rightpanelheight);
        }

        public void BuildLeftSidebar(ref int y)
        {
            Label title = new Label()
            {
                Text = "LocaLink",
                Font = new Font("Segoe UI", 30, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(125, y)
            };
            Label subTitle = new Label()
            {
                Text = "Local Network Chat System",
                Font = new Font("Segoe UI", 15),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(130, y+80)
            };
            Label text1 = new Label()
            {
                Text = "Discover nearby servers automatically",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.Beige,
                AutoSize = true,
                Location = new Point(150, y + 160)
            };
            Label text2 = new Label()
            {
                Text = "Real-time messaging with multiple users",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.Beige,
                AutoSize = true,
                Location = new Point(150, y + 190)
            };
            Label text3 = new Label()
            {
                Text = "Self-hosted servers on local network",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.Beige,
                AutoSize = true,
                Location = new Point(150, y + 220)
            };
            Label text4 = new Label()
            {
                Text = "Collaborative drawing mini-games",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.Beige,
                AutoSize = true,
                Location = new Point(150, y + 250)
            };

            PLeft.Controls.Add(title);
            PLeft.Controls.Add(subTitle);
            PLeft.Controls.Add(text1);
            PLeft.Controls.Add(text2);
            PLeft.Controls.Add(text3);
            PLeft.Controls.Add(text4);
        }

        //right panel info carryover
        TextBox username;
        TextBox phoneNumber;
        RadioButton customUser;
        RadioButton anonymousUser;
        Button login;
        User newuser;
        public void BuildRightSidebar(ref int y)
        {

            //Title
            Label title = new Label()
            {
                Text = "Welcome",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(200, y)
            };
            Label subTitle = new Label()
            {
                Text = "Join Your Local Chat",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.Gray,
                AutoSize = true,
                Location = new Point(200, y + 50)
            };

            //Login/Signup
            Label labelUsername = new Label()
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
            };

            Label labelPhoneNumber = new Label()
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
            Label sublabelPhone = new Label()
            {
                Text = "Register with mobile for persisent data",
                Font = new Font("Segoe UI", 5, FontStyle.Bold),
                ForeColor = Color.Gray,
                AutoSize = true,
                Location = new Point(200, y + 250)
            };

            //user join mode
            Label labelUsermode = new Label()
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

            anonymousUser = new RadioButton()
            {
                Text = "Anonymous User",
                Font = new Font("Segoe UI", 10),
                AutoSize = true,
                Size = new Size(400, 10),
                Location = new Point(200, y + 360)
            };

            //to enable and disable input for anonymous
            customUser.CheckedChanged += (sender, args) =>
            {
                username.Enabled = true;
                phoneNumber.Enabled = true;
            };
            anonymousUser.CheckedChanged += (sender, args) =>
            {
                username.Enabled = false;
                phoneNumber.Enabled = false;
            };

            login = new Button()
            {
                Text = "Login",
                Font = new Font("Segoe UI", 10),
                AutoSize = true,
                Location = new Point(200, y + 400)
            };
            login.Click += (sender, args) =>
            {
            
                if (customUser.Checked)
                {
                    newuser = new User(username.Text, phoneNumber.Text);
                }
                else
                {
                    newuser = new User("Anonymous", "1234567890");
                }
                Form2 form2 = new Form2(newuser);
                form2.StartPosition = FormStartPosition.CenterScreen;
                form2.Show();
                this.Hide();
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
            PRight.Controls.Add(login);
        }
    }
}
