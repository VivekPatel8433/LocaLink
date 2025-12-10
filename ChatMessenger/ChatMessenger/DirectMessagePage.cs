using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace ChatMessenger
{
    public partial class DirectMessagePage : Form
    {
        private User currentUser;
        private List<User> onlineUsers = new List<User>();  // dynamic list

        public DirectMessagePage(User user)
        {
            InitializeComponent();
            currentUser = user;

            BuildUI();
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

            // A flow panel to display users
            FlowLayoutPanel userListPanel = new FlowLayoutPanel()
            {
                Name = "UserListPanel",
                FlowDirection = FlowDirection.TopDown,
                AutoScroll = true,
                Location = new Point(20, 60),
                Size = new Size(340, 480)
            };

            Controls.Add(userListPanel);
        }

        // Call this every time the online users list updates
        public void UpdateOnlineUsers(List<User> users)
        {
            onlineUsers = users;

            FlowLayoutPanel panel = Controls["UserListPanel"] as FlowLayoutPanel;
            panel.Controls.Clear();

            foreach (var u in onlineUsers)
            {
                if (u.Username == currentUser.Username)
                    continue; // don't show yourself

                Button btn = new Button()
                {
                    Text = u.Username,
                    Width = 300,
                    Height = 40,
                    Tag = u
                };

                btn.Click += (s, e) =>
                {
                    User targetUser = (s as Button).Tag as User;

                    // Create a DM server
                    int dmPort = DMServerManager.CreateDMServer(currentUser.Username, targetUser.Username);

                    // Open chat window
                    DirectChatWindow chat = new DirectChatWindow(targetUser.Username, dmPort);
                    chat.Show();
                };

                panel.Controls.Add(btn);
            }
        }
    }
}
