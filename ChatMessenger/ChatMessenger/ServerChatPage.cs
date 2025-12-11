using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace ChatMessenger
{
    public partial class ServerChatPage : Form
    {

        private User currentUser;
        private DiscoveryClient discoveryClient;
        private DiscoveryServer discoveryServer;
        private Action<string> OpenServerChatCallback;
        private Button createServerBtn;
        private FlowLayoutPanel serversPanel;

        public ServerChatPage(User user, DiscoveryClient client, DiscoveryServer server, Action<string> openServerChat)
        {
            InitializeComponent();
            currentUser = user;
            discoveryClient = client;
            discoveryServer = server;
            OpenServerChatCallback = openServerChat;

            BuildUI();
            this.Load += ServerChatPage_Load;
        }

        private void BuildUI()
        {
            this.Text = "Server Chat";
            this.Size = new Size(800, 600);
            this.BackColor = ColorTranslator.FromHtml("#F5F7FA");

            Label title = new Label()
            {
                Text = "Available Servershmm",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                Location = new Point(20, 20),
                AutoSize = true
            };
            this.Controls.Add(title);

            createServerBtn = new Button()
            {
                Text = "Create server",
                Location = new Point(375, 675),
                AutoSize = true,
                Font = new Font("Segoe UI", 14, FontStyle.Bold)
            };
            this.Controls.Add(createServerBtn);

            serversPanel = new FlowLayoutPanel()
            {
                Location = new Point(20, 60),
                Size = new Size(this.ClientSize.Width - 40, this.ClientSize.Height - 80),
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };
            this.Controls.Add(serversPanel);


        }

        private async void ServerChatPage_Load(object sender, EventArgs e)
        {
            while (true)
            {
                await UpdateServerList();
                await Task.Delay(5000);
            }
        }

        private async Task UpdateServerList()
        {
            // Clear panel
            this.Invoke(() => serversPanel.Controls.Clear());

            // Local server
            if (discoveryServer != null)
            {
                string myServerKey = $"Local-{Environment.MachineName}-6000";
                Button localBtn = new Button()
                {
                    Text = "My Server",
                    Size = new Size(serversPanel.Width - 10, 40),
                    BackColor = ColorTranslator.FromHtml("#334155"),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };
                localBtn.Click += (s, e) =>
                {
                    OpenServerChatCallback?.Invoke(myServerKey); //
                };
                this.Invoke(() => serversPanel.Controls.Add(localBtn));
            }


            // Discovered servers
            if (discoveryClient != null)
            {
                await discoveryClient.DiscoverAsync();
                var servers = discoveryClient.GetDiscoveredServers();

                foreach (var server in servers)
                {
                    Button serverBtn = new Button()
                    {
                        Text = server.ToString(),
                        Size = new Size(serversPanel.Width - 10, 40),
                        BackColor = ColorTranslator.FromHtml("#0F172A"),
                        ForeColor = Color.White,
                        FlatStyle = FlatStyle.Flat
                    };

                    serverBtn.Click += (s, e) =>
                    {
                        OpenServerChatCallback?.Invoke($"{server.Name}-{server.IP}:{server.Port}");
                    };

                    this.Invoke(() => serversPanel.Controls.Add(serverBtn));
                }
            }
        }
    }
}
