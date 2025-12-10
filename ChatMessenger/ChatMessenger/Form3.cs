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

        // Stores chat history for each server the user created
        private Dictionary<string, List<string>> localServerChats = new Dictionary<string, List<string>>();


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
            LoadHomePage();


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

        private void LoadHomePage()
        {
            HomePage home = new HomePage();
            home.CreateServerClicked += (s, e) => CreateBtn_Click(s, e);
            LoadPageIntoMain(home);
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
            panelLeft.Controls.Add(CreateMenuButton("Direct Messages", ref y, DirectMessage_Click));
            panelLeft.Controls.Add(CreateMenuButton("Server Chat", ref y, ServerChat_Click));
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

        private void DirectMessage_Click(object sender, EventArgs e)
        {
            LoadPageIntoMain(new DirectMessagePage(currentUser));
        }

        private void ServerChat_Click(object sender, EventArgs e)
        {
            LoadPageIntoMain(new ServerChatPage(currentUser, discoveryClient, discoveryServer, OpenServerChat));
        }

        private void OpenServerChat(string serverKey)
        {
            ShowLocalServerChat(serverKey);
        }



        private void LoadPageIntoMain(Form page)
        {
            panelMain.Controls.Clear();
            page.TopLevel = false;
            page.FormBorderStyle = FormBorderStyle.None;
            page.Dock = DockStyle.Fill;
            panelMain.Controls.Add(page);
            page.Show();
        }



        private Button CreateMenuButton(string text, ref int y, EventHandler onClick = null)
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
                BackColor = Color.Transparent,
                Font = new Font("Segoe UI", 10)
            };
            btn.FlatAppearance.BorderSize = 0;

            if (onClick != null)
                btn.Click += onClick;

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
            serverListPanel.Controls.Clear();

            if (discoveryServer != null)
            {
                string myServerKey = $"Local-{Environment.MachineName}-6000"; // ✅ Use "Local-" prefix

                Button myServerButton = new Button()
                {
                    Text = "My Server",
                    Size = new Size(serverListPanel.Width - 10, 40),
                    BackColor = ColorTranslator.FromHtml("#334155"),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };

                myServerButton.Click += (s, e) => ShowLocalServerChat(myServerKey);
                serverListPanel.Controls.Add(myServerButton);
            }

            // Add discovered servers
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

                serverBtn.Click += async (s, e) =>
                {
                    wsClient = new JsonWebSocketClient($"ws://{server.IP}:{server.Port}");
                    wsClient.OnMessage += (msg) =>
                    {
                        if (msg.Type == "chat" && chatPanel != null)
                        {
                            this.Invoke(() => AddChatMessage($"Friend: {msg.Data}", Color.LightGray));
                        }
                    };

                    await wsClient.StartAsync();
                    await wsClient.SendAsync(new WsMessage { Type = "join", Name = currentUser.Username });

                    ShowChatPanel();
                };

                serverListPanel.Controls.Add(serverBtn);
            }
        }




        private void CreateBtn_Click(object sender, EventArgs e)
        {
            if (discoveryServer != null)
            {
                MessageBox.Show("Server already running.");
                return;
            }

            string myServerKey = $"Local-{Environment.MachineName}-6000";

            // Create history list if not exists
            if (!localServerChats.ContainsKey(myServerKey))
                localServerChats[myServerKey] = new List<string>();

            // Start server
            discoveryServer = new DiscoveryServer(8888, 6000);
            discoveryServer.Enable();
            Task.Run(async () => await discoveryServer.StartAsync());

            // Show the chat panel
            ShowLocalServerChat(myServerKey);

            StartWebSocket();
        }


        private void ShowLocalServerChat(string serverKey)
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

            // Ensure the server key exists in the dictionary
            if (!localServerChats.ContainsKey(serverKey))
                localServerChats[serverKey] = new List<string>();

            // Load saved chat history
            foreach (var msg in localServerChats[serverKey])
            {
                AddChatMessage(msg, Color.LightGray);
            }

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

            sendButton.Click += async (s, e) => await SendMessageToLocal(serverKey);

            chatInput.KeyDown += async (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true;
                    await SendMessageToLocal(serverKey);
                }
            };
        }


        private async Task SendMessageToLocal(string serverKey)
        {
            if (!string.IsNullOrWhiteSpace(chatInput.Text))
            {
                string message = $"You: {chatInput.Text}";

                // Add message to chat history
                localServerChats[serverKey].Add(message);

                AddChatMessage(message, Color.LightBlue);

                if (wsClient != null)
                {
                    await wsClient.SendAsync(new WsMessage
                    {
                        Type = "chat",
                        Data = chatInput.Text
                    });
                }

                chatInput.Clear();
            }
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
