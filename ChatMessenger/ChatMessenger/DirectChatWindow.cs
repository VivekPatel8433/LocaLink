using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace ChatMessenger
{
    public partial class DirectChatWindow : Form
    {
        private string targetUsername;
        private int serverPort;
        private bool isServerChat = false;
        private JsonWebSocketClient wsClient;

        // UI controls
        private TextBox chatDisplay;
        private TextBox chatInput;
        private Button sendButton;

        // Direct DM constructor
        public DirectChatWindow(string username, int port)
        {
            InitializeComponent();
            targetUsername = username;
            serverPort = port;
            BuildUI();
        }

        // Server chat constructor
        public DirectChatWindow(User currentUser, string serverKey, bool isLocal, ServerIPPort serverInfo = null)
        {
            InitializeComponent();
            isServerChat = true;
            BuildUI();

            if (isLocal)
            {
                this.Text = $"Local Server: {serverKey}";
                // TODO: load local server chat history here
            }
            else if (serverInfo != null)
            {
                this.Text = $"Server: {serverInfo.Name}";
                wsClient = new JsonWebSocketClient($"ws://{serverInfo.IP}:{serverInfo.Port}");
                wsClient.OnMessage += (msg) =>
                {
                    if (msg.Type == "chat")
                        this.Invoke(() => AppendMessage("Friend", msg.Data?.ToString()));
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
        }

        private void BuildUI()
        {
            this.Size = new Size(800, 450);
            this.BackColor = Color.White;

            chatDisplay = new TextBox()
            {
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                Location = new Point(20, 20),
                Size = new Size(740, 330),
                Font = new Font("Segoe UI", 10),
            };
            this.Controls.Add(chatDisplay);

            chatInput = new TextBox()
            {
                Location = new Point(20, 370),
                Size = new Size(640, 30),
                Font = new Font("Segoe UI", 10),
            };
            this.Controls.Add(chatInput);

            sendButton = new Button()
            {
                Text = "Send",
                Location = new Point(670, 370),
                Size = new Size(90, 30),
            };
            sendButton.Click += SendButton_Click;
            this.Controls.Add(sendButton);
        }

        private async void SendButton_Click(object sender, EventArgs e)
        {
            string text = chatInput.Text.Trim();
            if (text.Length == 0) return;

            AppendMessage("You", text);

            if (isServerChat && wsClient != null)
            {
                await wsClient.SendAsync(new WsMessage
                {
                    Type = "chat",
                    Data = text
                });
            }

            chatInput.Text = "";
        }

        public void AppendMessage(string sender, string message)
        {
            chatDisplay.AppendText($"{sender}: {message}\r\n");
        }
    }
}
