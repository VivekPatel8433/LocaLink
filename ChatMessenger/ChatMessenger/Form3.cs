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
            // Avatar
            PictureBox avatar = new PictureBox()
            {
                Size = new Size(90, 90),
                SizeMode = PictureBoxSizeMode.Zoom,
                Location = new Point(90, 10),
                Image = Image.FromFile(@"C:\Users\vivek\OneDrive\Desktop\TERM 3\OBJECT ORIENTED PROGRAMMING\Final Project\ChatMessenger\Profile Search.png")
            };
            panelRight.Controls.Add(avatar);

            panelRight.Controls.Add(new Label()
            {
                Text = "Anonymous User",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(40, 100)
            });

            panelRight.Controls.Add(new Label()
            {
                Text = "Random Chat Partner",
                Font = new Font("Segoe UI", 12),
                AutoSize = true,
                Location = new Point(30,120)
            });


            // Separator
            panelRight.Controls.Add(new Panel()
            {
                Height = 1,
                Dock = DockStyle.Top,
                BackColor = Color.LightGray,
                Margin = new Padding(0, 20, 0, 20)
            });

            // Toggles
            int y = 180;

            // Start Drawing Game button
            Button drawingBtn = new Button()
            {
                Text = "Start Drawing Game",
                Size = new Size(220, 35),
                Location = new Point(25, y),
                BackColor = ColorTranslator.FromHtml("#0F172A"),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            drawingBtn.FlatAppearance.BorderSize = 0;
            panelRight.Controls.Add(drawingBtn);
            y += 60;

            // Add Friend button
            Button shareScreenBtn = new Button()
            {
                Text = "Add A Friend",
                Size = new Size(220, 35),
                Location = new Point(25, y),
                BackColor = ColorTranslator.FromHtml("#0F172A"),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            shareScreenBtn.FlatAppearance.BorderSize = 0;
            panelRight.Controls.Add(shareScreenBtn);
            y += 65;

            // Block User button
            Button sendFileBtn = new Button()
            {
                Text = "Block User",
                Size = new Size(220, 35),
                Location = new Point(25, y),
                BackColor = ColorTranslator.FromHtml("#0F172A"),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            sendFileBtn.FlatAppearance.BorderSize = 0;
            panelRight.Controls.Add(sendFileBtn);
            y += 70;

            // Quick Actions
            Label quickLabel = new Label()
            {
                Text = "Quick Actions",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(5, y - 30)
            };
            panelRight.Controls.Add(quickLabel);


            // Find New Partner
            Button newPartnerBtn = new Button()
            {
                Text = "Find New Partner",
                Size = new Size(220, 35),
                Location = new Point(25, y),
                BackColor = ColorTranslator.FromHtml("#EF4444"),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            newPartnerBtn.FlatAppearance.BorderSize = 0;
            panelRight.Controls.Add(newPartnerBtn);
            y += 60;

            // Find New Partner
            Button muteChatBtn = new Button()
            {
                Text = "Mute Chat",
                Size = new Size(220, 35),
                Location = new Point(25, y),
                BackColor = ColorTranslator.FromHtml("#EF4444"),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            muteChatBtn.FlatAppearance.BorderSize = 0;
            panelRight.Controls.Add(muteChatBtn);
            y += 60;

            // Find New Partner
            Button exportChatBtn = new Button()
            {
                Text = "Export Chat",
                Size = new Size(220, 35),
                Location = new Point(25, y),
                BackColor = ColorTranslator.FromHtml("#EF4444"),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            exportChatBtn.FlatAppearance.BorderSize = 0;
            panelRight.Controls.Add(exportChatBtn);
            y += 60;

            // Network Status Panel
            Panel networkStatus = new Panel()
            {
                Height = 120,
                Dock = DockStyle.Bottom,
                BackColor = Color.FromArgb(230, 230, 230),
                Padding = new Padding(10)
            };
            panelRight.Controls.Add(networkStatus);

            // Title
            Label networkTitle = new Label()
            {
                Text = "Network Status",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(5, 5)
            };
            networkStatus.Controls.Add(networkTitle);

            // Connection status
            Label connectionStatus = new Label()
            {
                Text = "Connection: Connected",
                Font = new Font("Segoe UI", 9),
                AutoSize = true,
                Location = new Point(5, 25),
                ForeColor = Color.Green
            };
            networkStatus.Controls.Add(connectionStatus);

            // Server IP
            Label serverIP = new Label()
            {
                Text = "Server: 198.165.1.25",
                Font = new Font("Segoe UI", 9),
                AutoSize = true,
                Location = new Point(5, 45)
            };
            networkStatus.Controls.Add(serverIP);

        }


        private void BuildMainArea()
        {

        }
        private void Form3_Load(object sender, EventArgs e)
        {

        }
    }
}
