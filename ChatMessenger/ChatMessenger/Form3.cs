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
           
        }

        private void BuildUI()
        {
            this.Text = "RandomChatPage";
            this.BackColor = ColorTranslator.FromHtml("#F5F7FA");
            this.Size = new Size(1400, 800);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;

            // LEFT PANEL
            panelLeft = new Panel()
            {
                Dock = DockStyle.Left,
                Width = 230,
                BackColor = Color.White,
                Padding = new Padding(15, 20, 15, 20)
            };
            this.Controls.Add(panelLeft);
            BuildLeftSidebar();


            // RIGHT PANEL
            panelRight = new Panel()
            {
                Dock = DockStyle.Right,
                Width = 270,
                BackColor = Color.White,
                Padding = new Padding(20, 20, 20, 20)
            };
            this.Controls.Add(panelRight);
            BuildRightSidebar();

            // MAIN PANEL
            panelMain = new Panel()
            {
                Dock = DockStyle.Fill,
                BackColor = ColorTranslator.FromHtml("#F5F7FA")
            };
            this.Controls.Add(panelMain);
            BuildMainArea();
        }

        private void BuildLeftSidebar()
        {
            // Avatar
            PictureBox avatar = new PictureBox()
            {
                Size = new Size(60, 60),
                Location = new Point(20, 10),
                SizeMode = PictureBoxSizeMode.Zoom,
                Image = Image.FromFile(@"C:\Users\vivek\OneDrive\Desktop\TERM 3\OBJECT ORIENTED PROGRAMMING\Final Project\ChatMessenger\Profile-Avatar-PNG.png")
            };
            panelLeft.Controls.Add(avatar);

            // Name and Status
            panelLeft.Controls.Add(new Label()
            {
                Text = "Vivek Patel",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(20, 80)
            });

            panelLeft.Controls.Add(new Label()
            {
                Text = "Online",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Green,
                AutoSize = true,
                Location = new Point(20, 105)
            });

            // Menu
            int y = 150;
            panelLeft.Controls.Add(CreateMenuButton("Direct Messages", ref y));
            panelLeft.Controls.Add(CreateMenuButton("Server Chat", ref y));
            panelLeft.Controls.Add(CreateMenuButton("Random Chat", ref y, true));
            panelLeft.Controls.Add(CreateMenuButton("Drawing Game", ref y));

            // Recent Chats label
            Label recentLabel = new Label()
            {
                Text = "Recent Chats",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(20, y + 20)
            };
            panelLeft.Controls.Add(recentLabel);

            // FlowLayoutPanel for chat items
            FlowLayoutPanel recentChatsPanel = new FlowLayoutPanel()
            {
                Location = new Point(20, y + 50),
                Size = new Size(panelLeft.Width - 40, 400),
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false
            };
            panelLeft.Controls.Add(recentChatsPanel);

            // Example recent chats
            string[] recentChats = { "Alice", "Bob", "Charlie", "David" };
            foreach (var chatName in recentChats)
            {
                Panel chatItem = new Panel()
                {
                    Size = new Size(recentChatsPanel.Width - 20, 50),
                    BackColor = Color.FromArgb(240, 240, 240),
                    Margin = new Padding(0, 0, 0, 10)
                };

                PictureBox chatAvatar = new PictureBox()
                {
                    Size = new Size(40, 40),
                    Location = new Point(5, 5),
                    SizeMode = PictureBoxSizeMode.Zoom,
                    Image = Image.FromFile(@"C:\Users\vivek\OneDrive\Desktop\TERM 3\OBJECT ORIENTED PROGRAMMING\Final Project\ChatMessenger\Recent chats.jpg") // replace with avatar
                };
                chatItem.Controls.Add(chatAvatar);

                Label chatNameLabel = new Label()
                {
                    Text = chatName,
                    Font = new Font("Segoe UI", 10, FontStyle.Bold),
                    Location = new Point(50, 12),
                    AutoSize = true
                };
                chatItem.Controls.Add(chatNameLabel);

                recentChatsPanel.Controls.Add(chatItem);
            }

        }
        private Button CreateMenuButton(string text, ref int y, bool highlight = false)
        {
            Button btn = new Button()
            {
                Text = text,
                Height = 38,
                Width = 180,
                FlatStyle = FlatStyle.Flat,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(15, 0, 0, 0),
                Location = new Point(15, y),
                BackColor = highlight ? ColorTranslator.FromHtml("#F1F1F1") : Color.Transparent,
                Font = highlight ? new Font("Segoe UI", 10, FontStyle.Bold) : new Font("Segoe UI", 10)
            };
            btn.FlatAppearance.BorderSize = 0;
            y += 45;
            return btn;
        }
        private void BuildRightSidebar()
        {

        }
        private void BuildMainArea()
        {

        }
        private void Form3_Load(object sender, EventArgs e)
        {

        }
    }
}
