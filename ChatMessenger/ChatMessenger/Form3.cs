using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ChatMessenger
{
    public partial class Form3 : Form
    {
        private Panel panelLeft;
        private Panel panelMain;
        private Panel panelRight;
        private User currentUser;

        //Chat controls
        private FlowLayoutPanel chatPanel;
        private TextBox chatInput;
        private Button sendButton;

        //WebSocket client
        private JsonWebSocketClient wsClient;

        //Discovery
        private DiscoveryClient discoveryClient;
        private DiscoveryServer discoveryServer;
        private FlowLayoutPanel serverListPanel;

        public Form3(User user)
        {
            InitializeComponent();
            currentUser = user;
            BuildUI();
            this.Load += Form3_Load;
        }

        private void BuildUI()
        {
            this.Text = "RandomChatPage";
            this.BackColor = ColorTranslator.FromHtml("#F5F7FA");
            this.Size = new Size(1400, 800);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;

            //MAIN PANEL
            panelMain = new Panel()
            {
                Dock = DockStyle.Fill,
                BackColor = ColorTranslator.FromHtml("#F5F7FA")
            };
            this.Controls.Add(panelMain);
            BuildMainHomepage();

            //LEFT PANEL
            panelLeft = new Panel()
            {
                Dock = DockStyle.Left,
                Width = 230,
                BackColor = Color.White,
                Padding = new Padding(15, 20, 15, 20)
            };
            this.Controls.Add(panelLeft);
            BuildLeftSidebar();

            //RIGHT PANEL
            panelRight = new Panel()
            {
                Dock = DockStyle.Right,
                Width = 270,
                BackColor = Color.White,
                Padding = new Padding(20, 20, 20, 20)
            };
            this.Controls.Add(panelRight);
            BuildRightSidebar();
        }
        private void BuildLeftSidebar()
        {
            //Avatar
            PictureBox avatar = new PictureBox()
            {
                Size = new Size(60, 60),
                Location = new Point(20, 10),
                SizeMode = PictureBoxSizeMode.Zoom,
                Image = Image.FromFile(@"Resources\Assets\ProfileAvatar.png")
            };
            panelLeft.Controls.Add(avatar);

            //Username
            panelLeft.Controls.Add(new Label()
            {
                Text = currentUser.Username,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(20, 80)
            });

            //Status
            panelLeft.Controls.Add(new Label()
            {
                Text = "Online",
                Font = new Font("Segoe UI", 9),
                ForeColor = Color.Green,
                AutoSize = true,
                Location = new Point(20, 105)
            });

            int y = 150;
            panelLeft.Controls.Add(CreateMenuButton("Direct Messages", ref y));
            panelLeft.Controls.Add(CreateMenuButton("Server Chat", ref y));
            panelLeft.Controls.Add(CreateMenuButton("Random Chat", ref y));
            panelLeft.Controls.Add(CreateMenuButton("Drawing Game", ref y));

            //Recent Chats
            Label recentLabel = new Label()
            {
                Text = "Recent Chats",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(20, y + 20)
            };
            panelLeft.Controls.Add(recentLabel);

            FlowLayoutPanel recentChatsPanel = new FlowLayoutPanel()
            {
                Location = new Point(20, y + 50),
                Size = new Size(panelLeft.Width - 40, panelLeft.Height - (y + 50) - 20),
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false
            };
            panelLeft.Controls.Add(recentChatsPanel);

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
                    Image = Image.FromFile(@"Resources\Assets\RecentChats.jpg")
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
            btn.Click += (s, e) => MessageBox.Show($"{text} clicked!");
            y += 45;
            return btn;
        }
        private void BuildRightSidebar()
        {
            panelRight.Controls.Clear();

            Label title = new Label()
            {
                Text = "Available Servers",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(20, 20)
            };
            panelRight.Controls.Add(title);

            serverListPanel = new FlowLayoutPanel()
            {
                Location = new Point(10, 50),
                Size = new Size(panelRight.Width - 20, panelRight.Height - 60),
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false
            };
            panelRight.Controls.Add(serverListPanel);
        }

        private void UpdateAvailableServers(List<ServerIPPort> servers)
        {
            if (serverListPanel == null) return;

            serverListPanel.Controls.Clear();

            foreach (var server in servers)
            {
                Button serverBtn = new Button()
                {
                    Text = server.ToString(),
                    Size = new Size(serverListPanel.Width - 10, 40),
                    BackColor = ColorTranslator.FromHtml("#0F172A"),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };
                serverBtn.FlatAppearance.BorderSize = 0;
                serverBtn.Click += async (s, e) =>
                {
                    //Connect to the server via WebSocket
                    wsClient = new JsonWebSocketClient($"ws://{server.IP}:{server.Port}");
                    wsClient.OnMessage += (msg) =>
                    {
                        if (msg.Type == "chat" && chatPanel != null)
                        {
                            this.Invoke(() => AddChatMessage($"Friend: {msg.Data}", Color.LightGray));
                        }
                    };
                    await wsClient.StartAsync();
                    await wsClient.SendAsync(new WsMessage
                    {
                        Type = "join",
                        Name = currentUser.Username
                    });

                    //Switch to chat panel
                    ShowChatPanel();
                };
                serverListPanel.Controls.Add(serverBtn);
            }
        }
        private void BuildMainHomepage()
        {
            panelMain.Controls.Clear();

            Label title = new Label()
            {
                Text = "Welcome to LocaLink",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleCenter
            };
            panelMain.Controls.Add(title);

            Label description = new Label()
            {
                Text = "Connect with users on your local network.\nCreate a server to start chatting.",
                Font = new Font("Segoe UI", 10),
                AutoSize = true,
                MaximumSize = new Size(500, 0),
                TextAlign = ContentAlignment.MiddleCenter
            };
            panelMain.Controls.Add(description);

            Button createBtn = new Button()
            {
                Text = "Create Server",
                Size = new Size(200, 40),
                BackColor = Color.White,
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat
            };
            createBtn.FlatAppearance.BorderSize = 1;
            createBtn.Click += CreateBtn_Click;
            panelMain.Controls.Add(createBtn);

            //Center the controls after they are added
            panelMain.Resize += (s, e) =>
            {
                title.Location = new Point((panelMain.Width - title.Width) / 2, 150);
                description.Location = new Point((panelMain.Width - description.Width) / 2, title.Bottom + 20);
                createBtn.Location = new Point((panelMain.Width - createBtn.Width) / 2, description.Bottom + 30);
            };

            //Trigger resize once to position them initially
            panelMain.PerformLayout();
            panelMain.Invalidate();
        }


        private void CreateBtn_Click(object sender, EventArgs e)
        {
            // Start local discovery server
            discoveryServer = new DiscoveryServer(8888, 6000); // broadcastPort, wsPort
            discoveryServer.Enable();
            Task.Run(async () => await discoveryServer.StartAsync());

            // Show chat panel locally
            ShowChatPanel();
            StartWebSocket();
        }

        private void ShowChatPanel()
        {
            panelMain.Controls.Clear();

            chatPanel = new FlowLayoutPanel()
            {
                Location = new Point(20, 20),
                Size = new Size(panelMain.Width - 40, panelMain.Height - 80),
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false
            };
            panelMain.Controls.Add(chatPanel);

            chatInput = new TextBox()
            {
                Size = new Size(panelMain.Width - 140, 30),
                Location = new Point(20, panelMain.Height - 50)
            };
            panelMain.Controls.Add(chatInput);

            sendButton = new Button()
            {
                Text = "Send",
                Size = new Size(100, 30),
                Location = new Point(panelMain.Width - 110, panelMain.Height - 50)
            };
            panelMain.Controls.Add(sendButton);

            //Send message on button click
            sendButton.Click += async (s, e) => await SendMessage();

            //Send message when pressing Enter
            chatInput.KeyDown += async (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    await SendMessage();
                }
            };
        }

        private async Task SendMessage()
        {
            if (!string.IsNullOrWhiteSpace(chatInput.Text) && wsClient != null)
            {
                await wsClient.SendAsync(new WsMessage
                {
                    Type = "chat",
                    Data = chatInput.Text
                });

                AddChatMessage($"You: {chatInput.Text}", Color.LightBlue);
                chatInput.Clear();
            }
        }


        private void AddChatMessage(string text, Color backColor)
        {
            Label lbl = new Label()
            {
                Text = text,
                AutoSize = true,
                MaximumSize = new Size(chatPanel.Width - 20, 0),
                BackColor = backColor,
                Padding = new Padding(5)
            };
            chatPanel.Controls.Add(lbl);
            chatPanel.ScrollControlIntoView(lbl);
        }


        private void StartWebSocket()
        {
            wsClient = new JsonWebSocketClient("ws://localhost:6000");
            wsClient.OnMessage += (msg) =>
            {
                if (msg.Type == "chat" && chatPanel != null)
                {
                    this.Invoke(() => AddChatMessage($"Friend: {msg.Data}", Color.LightGray));
                }
            };

            Task.Run(async () =>
            {
                await wsClient.StartAsync();
                await wsClient.SendAsync(new WsMessage
                {
                    Type = "join",
                    Name = currentUser.Username
                });
            });
        }

        private async void Form3_Load(object sender, EventArgs e)
        {
            discoveryClient = new DiscoveryClient(8888);
            await DiscoverServersLoop();
        }

        private async Task DiscoverServersLoop()
        {
            while (true)
            {
                await discoveryClient.DiscoverAsync();
                var servers = discoveryClient.GetDiscoveredServers();
                this.Invoke(() => UpdateAvailableServers(servers));
                await Task.Delay(5000); //scan every 5 sec
            }
        }

    }
}
