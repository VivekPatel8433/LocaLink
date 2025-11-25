using System;
using System.Drawing;
using System.Windows.Forms;

namespace ChatMessenger
{
    public partial class Form2 : Form
    {
        Panel panelLeft;
        Panel panelMain;
        Panel panelRight;

        public Form2()
        {
            InitializeComponent();
            BuildUI();
        }


        private void Form2_Click(object sender, EventArgs e)
        {
            try
            {
                Form3 main = new Form3();
                main.StartPosition = FormStartPosition.CenterScreen;
                main.Show();
                this.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void BuildUI()
        {
            this.Text = "RandomChat";
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
            panelMain.Click += Form2_Click;
        }

        private void BuildLeftSidebar()
        {
            // Avatar
            PictureBox avatar = new PictureBox()
            {
                Size = new Size(60, 60),
                Location = new Point(20, 10),
                SizeMode = PictureBoxSizeMode.Zoom,
                //Image = Properties.Resources. Assets.ProfileAvatar)
                Image = Image.FromFile(@"Resources\Assets\ProfileAvatar.png")
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
                    Image = Image.FromFile(@"Resources\Assets\RecentChats.jpg") // replace with avatar
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
                Image = Image.FromFile(@"Resources\Assets\ProfileSearch.png")
            };
            panelRight.Controls.Add(avatar);

            // Status
            panelRight.Controls.Add(new Label()
            {
                Text = "Searching...",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(80, 100)

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
            int y = 130;
            panelRight.Controls.Add(CreateToggle("Anonymous Mode", ref y, true));
            panelRight.Controls.Add(CreateToggle("Auto-match", ref y));
            panelRight.Controls.Add(CreateToggle("Share Location", ref y));

            // Quick actions
            // Quick Actions label
            Label quickLabel = new Label()
            {
                Text = "Quick Actions",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(75, y + 20)
            };
            panelRight.Controls.Add(quickLabel);

            y += 50; // move y below the label

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
            y += 45;

            // Share Screen button
            Button shareScreenBtn = new Button()
            {
                Text = "Share Screen",
                Size = new Size(220, 35),
                Location = new Point(25, y),
                BackColor = ColorTranslator.FromHtml("#0F172A"),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            shareScreenBtn.FlatAppearance.BorderSize = 0;
            panelRight.Controls.Add(shareScreenBtn);
            y += 45;

            // Send File button
            Button sendFileBtn = new Button()
            {
                Text = "Send File",
                Size = new Size(220, 35),
                Location = new Point(25, y),
                BackColor = ColorTranslator.FromHtml("#0F172A"),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            sendFileBtn.FlatAppearance.BorderSize = 0;
            panelRight.Controls.Add(sendFileBtn);
            y += 45;


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

            // Users online
            Label usersOnline = new Label()
            {
                Text = "Users Online: 12",
                Font = new Font("Segoe UI", 9),
                AutoSize = true,
                Location = new Point(5, 65)
            };
            networkStatus.Controls.Add(usersOnline);

        }

        private CheckBox CreateToggle(string text, ref int y, bool on = false)
        {
            CheckBox toggle = new CheckBox()
            {
                Text = "  " + text,
                AutoSize = true,
                Location = new Point(5, y),
                Font = new Font("Segoe UI", 10),
                Checked = on
            };
            y += 35;
            return toggle;
        }

        private void BuildMainArea()
        {
            // Header
            Panel header = new Panel()
            {
                Dock = DockStyle.Top,
                Height = 55,
                BackColor = Color.White
            };
            panelMain.Controls.Add(header);

            header.Controls.Add(new Label()
            {
                Text = "Random Chat",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(20, 15)
            });

            // Center Icon
            PictureBox icon = new PictureBox()
            {
                Size = new Size(65, 65),
                SizeMode = PictureBoxSizeMode.Zoom,
                Image = Image.FromFile(@"Resources\Assets\Search.png"),
                Location = new Point((panelMain.Width / 2) - 30, 200),
                Anchor = AnchorStyles.None
            };
            panelMain.Controls.Add(icon);
            icon.Click += Form2_Click;

            // Center Text
            panelMain.Controls.Add(new Label()
            {
                Text = "Looking for a random chat partner...",
                Font = new Font("Segoe UI", 12),
                AutoSize = true,
                Location = new Point((panelMain.Width / 2) - 180, 280),
                Anchor = AnchorStyles.None
            });

            panelMain.Controls.Add(new Label()
            {
                Text = "We're matching you with someone nearby",
                Font = new Font("Segoe UI", 12),
                AutoSize = true,
                Location = new Point((panelMain.Width / 2) - 180, 310),
                Anchor = AnchorStyles.None
            });

            // Cancel Button
            Button cancelBtn = new Button()
            {
                Text = "Cancel Search",
                Size = new Size(160, 40),
                Location = new Point((panelMain.Width / 2) - 80, 350),
                BackColor = ColorTranslator.FromHtml("#0F172A"),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            cancelBtn.FlatAppearance.BorderSize = 0;
            panelMain.Controls.Add(cancelBtn);
        }

        private void pictureBox2_Load(object sender, EventArgs e)
        {

        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }

        private void Form2_Load_1(object sender, EventArgs e)
        {

        }
    }
}
