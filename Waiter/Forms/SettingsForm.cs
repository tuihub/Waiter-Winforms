using Waiter.Services;

namespace Waiter.Forms
{
    public partial class SettingsForm : Form
    {
        private readonly ConfigService _configService;
        private readonly TokenService _tokenService;

        private TextBox _txtServerUrl = null!;
        private Button _btnSave = null!;
        private Button _btnCancel = null!;
        private Button _btnLogout = null!;
        private Label _lblStatus = null!;

        public SettingsForm(ConfigService configService, TokenService tokenService)
        {
            _configService = configService;
            _tokenService = tokenService;

            InitializeComponent();
            LoadSettings();
        }

        private void InitializeComponent()
        {
            this.Text = "Settings";
            this.Size = new Size(450, 250);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.FromArgb(30, 30, 30);

            // Title
            var lblTitle = new Label
            {
                Text = "Settings",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 15),
                AutoSize = true
            };
            this.Controls.Add(lblTitle);

            // Server URL Section
            var lblServerSection = new Label
            {
                Text = "Server Configuration",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = Color.LightGray,
                Location = new Point(20, 55),
                AutoSize = true
            };
            this.Controls.Add(lblServerSection);

            var lblServerUrl = new Label
            {
                Text = "Server URL:",
                ForeColor = Color.LightGray,
                Location = new Point(20, 80),
                AutoSize = true
            };
            this.Controls.Add(lblServerUrl);

            _txtServerUrl = new TextBox
            {
                Location = new Point(20, 100),
                Size = new Size(390, 25),
                BackColor = Color.FromArgb(45, 45, 45),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            this.Controls.Add(_txtServerUrl);

            // Status Label
            _lblStatus = new Label
            {
                Text = "",
                ForeColor = Color.LimeGreen,
                Location = new Point(20, 130),
                Size = new Size(390, 20),
                TextAlign = ContentAlignment.MiddleLeft
            };
            this.Controls.Add(_lblStatus);

            // Buttons
            _btnSave = new Button
            {
                Text = "Save",
                Location = new Point(140, 165),
                Size = new Size(80, 30),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            _btnSave.Click += BtnSave_Click;
            this.Controls.Add(_btnSave);

            _btnCancel = new Button
            {
                Text = "Cancel",
                Location = new Point(230, 165),
                Size = new Size(80, 30),
                BackColor = Color.FromArgb(60, 60, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            _btnCancel.Click += (s, e) => this.Close();
            this.Controls.Add(_btnCancel);

            _btnLogout = new Button
            {
                Text = "Logout",
                Location = new Point(320, 165),
                Size = new Size(80, 30),
                BackColor = Color.FromArgb(180, 50, 50),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            _btnLogout.Click += BtnLogout_Click;
            this.Controls.Add(_btnLogout);

            this.AcceptButton = _btnSave;
            this.CancelButton = _btnCancel;
        }

        private void LoadSettings()
        {
            _txtServerUrl.Text = _configService.ServerUrl;
            _btnLogout.Enabled = _tokenService.IsLoggedIn;
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            var serverUrl = _txtServerUrl.Text.Trim();

            if (string.IsNullOrEmpty(serverUrl))
            {
                _lblStatus.ForeColor = Color.OrangeRed;
                _lblStatus.Text = "Server URL cannot be empty";
                return;
            }

            try
            {
                _configService.ServerUrl = serverUrl;
                _lblStatus.ForeColor = Color.LimeGreen;
                _lblStatus.Text = "Settings saved successfully";
            }
            catch (Exception ex)
            {
                _lblStatus.ForeColor = Color.OrangeRed;
                _lblStatus.Text = $"Error saving settings: {ex.Message}";
            }
        }

        private void BtnLogout_Click(object? sender, EventArgs e)
        {
            var result = MessageBox.Show(
                "Are you sure you want to logout?",
                "Confirm Logout",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                _tokenService.ClearTokens();
                _btnLogout.Enabled = false;
                _lblStatus.ForeColor = Color.LimeGreen;
                _lblStatus.Text = "Logged out successfully";
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
    }
}
