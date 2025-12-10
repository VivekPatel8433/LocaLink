using System;
using System.Drawing;
using System.Windows.Forms;

namespace ChatMessenger
{
    public partial class HomePage : Form
    {
        public event EventHandler CreateServerClicked;  // notify Form3 to run CreateBtn logic

        public HomePage()
        {
            InitializeComponent();
            BuildHomeUI();
        }

        private void BuildHomeUI()
        {
            this.BackColor = ColorTranslator.FromHtml("#F5F7FA");

            Label title = new Label()
            {
                Text = "Welcome to LocaLink",
                Font = new Font("Segoe UI", 20, FontStyle.Bold),
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(title);

            Label description = new Label()
            {
                Text = "Connect with users on your local network.\nCreate a server to start chatting.",
                Font = new Font("Segoe UI", 10),
                AutoSize = true,
                MaximumSize = new Size(500, 0),
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(description);

            Button createBtn = new Button()
            {
                Text = "Create Server",
                Size = new Size(200, 40),
                BackColor = Color.White,
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat
            };
            createBtn.FlatAppearance.BorderSize = 1;
            this.Controls.Add(createBtn);

            // When user clicks Create Server notify Form3
            createBtn.Click += (s, e) => CreateServerClicked?.Invoke(this, EventArgs.Empty);

            // Center everything when resized
            this.Resize += (s, e) =>
            {
                title.Location = new Point((this.Width - title.Width) / 2, 150);
                description.Location = new Point((this.Width - description.Width) / 2, title.Bottom + 20);
                createBtn.Location = new Point((this.Width - createBtn.Width) / 2, description.Bottom + 30);
            };
        }
    }
}
