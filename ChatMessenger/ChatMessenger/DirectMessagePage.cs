using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace ChatMessenger
{
    public partial class DirectMessagePage : Form
    {
        private User currentUser;
        private DiscoveryClient discoveryClient;
        private DiscoveryServer discoveryServer;
        private FlowLayoutPanel userListPanel;

        // Keeps track of currently online users
        private List<User> onlineUsers = new List<User>();

        private JsonWebSocketServer mainServer;

        public DirectMessagePage(User user, DiscoveryClient client, DiscoveryServer server, JsonWebSocketServer wsServer)
        {
            InitializeComponent();
            currentUser = user;
            discoveryClient = client;
            discoveryServer = server;
            mainServer = wsServer; // ✅ must be a server

            BuildUI();
            StartDiscoverLoop();
        }


        private void BuildUI()
        {
            this.Text = "Direct Messages";
            this.Size = new Size(400, 600);

            Label lbl = new Label()
            {
                Text = "Online Users",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(20, 20)
            };
            Controls.Add(lbl);

            userListPanel = new FlowLayoutPanel()
            {
                Name = "UserListPanel",
                FlowDirection = FlowDirection.TopDown,
                AutoScroll = true,
                Location = new Point(20, 60),
                Size = new Size(340, 480)
            };
            Controls.Add(userListPanel);
        }

        private async void StartDiscoverLoop()
        {
            // Keep discovering servers every 5 seconds
            while (true)
            {
                try
                {
                    await discoveryClient.DiscoverAsync();
                    UpdateOnlineUsersFromDiscovery();
                }
                catch { }
                await Task.Delay(5000);
            }
        }

        private void UpdateOnlineUsersFromDiscovery()
        {
            var servers = discoveryClient.GetDiscoveredServers();

            // Convert discovered servers into Users
            onlineUsers = servers
                .Where(s => s.Name != currentUser.Username) // don't include yourself
                .Select(s => new User(s.Name, "", s.IP, s.Port))
                .ToList();

            RefreshUserListPanel();
        }

        private void RefreshUserListPanel()
        {
            if (userListPanel == null) return;

            userListPanel.Controls.Clear();

            foreach (var u in onlineUsers)
            {
                Button btn = new Button()
                {
                    Text = u.Username,
                    Width = 300,
                    Height = 40,
                    Tag = u
                };

                btn.Click += async (s, e) =>
                {
                    User targetUser = (s as Button).Tag as User;

                    // Create a private DM server
                    int dmPort = await PrivateDmHelper.CreatePrivateServerAndInvite(
                        mainServer,
                        targetUser.Username,
                        currentUser.Username
                    );

                    if (dmPort > 0)
                    {
                        // Open chat window
                        DirectChatWindow chat = new DirectChatWindow(targetUser.Username, dmPort);
                        chat.Show();
                    }
                    else
                    {
                        MessageBox.Show("Failed to start private chat.");
                    }
                };

                userListPanel.Controls.Add(btn);
            }
        }
    }
}
