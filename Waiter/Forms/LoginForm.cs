using Waiter.Services;

namespace Waiter.Forms
{
    public partial class LoginForm : Form
    {
        private readonly LibrarianClientService _clientService;
        private readonly TokenService _tokenService;
        private readonly ConfigService _configService;

        private TextBox _txtUsername = null!;
        private TextBox _txtPassword = null!;
        private TextBox _txtServerUrl = null!;
        private Button _btnLogin = null!;
        private Button _btnCancel = null!;
        private Label _lblStatus = null!;
        private ProgressBar _progressBar = null!;

        public bool LoginSuccessful { get; private set; }

        public LoginForm(LibrarianClientService clientService, TokenService tokenService, ConfigService configService)
        {
            _clientService = clientService;
            _tokenService = tokenService;
            _configService = configService;

            InitializeComponent();
            _txtServerUrl.Text = _configService.ServerUrl;
        }

        private void InitializeComponent()
        {
            this.Text = "TuiHub - Login";
            this.Size = new Size(400, 320);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(30, 30, 30);

            // Title Label
            var lblTitle = new Label
            {
                Text = "TuiHub Login",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 15),
                AutoSize = true
            };
            this.Controls.Add(lblTitle);

            // Server URL
            var lblServer = new Label
            {
                Text = "Server URL:",
                ForeColor = Color.LightGray,
                Location = new Point(20, 60),
                AutoSize = true
            };
            this.Controls.Add(lblServer);

            _txtServerUrl = new TextBox
            {
                Location = new Point(20, 80),
                Size = new Size(340, 25),
                BackColor = Color.FromArgb(45, 45, 45),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            this.Controls.Add(_txtServerUrl);

            // Username
            var lblUsername = new Label
            {
                Text = "Username:",
                ForeColor = Color.LightGray,
                Location = new Point(20, 115),
                AutoSize = true
            };
            this.Controls.Add(lblUsername);

            _txtUsername = new TextBox
            {
                Location = new Point(20, 135),
                Size = new Size(340, 25),
                BackColor = Color.FromArgb(45, 45, 45),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            this.Controls.Add(_txtUsername);

            // Password
            var lblPassword = new Label
            {
                Text = "Password:",
                ForeColor = Color.LightGray,
                Location = new Point(20, 170),
                AutoSize = true
            };
            this.Controls.Add(lblPassword);

            _txtPassword = new TextBox
            {
                Location = new Point(20, 190),
                Size = new Size(340, 25),
                BackColor = Color.FromArgb(45, 45, 45),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                UseSystemPasswordChar = true
            };
            _txtPassword.KeyPress += (s, e) =>
            {
                if (e.KeyChar == (char)Keys.Enter)
                {
                    e.Handled = true;
                    BtnLogin_Click(s, e);
                }
            };
            this.Controls.Add(_txtPassword);

            // Status Label
            _lblStatus = new Label
            {
                Text = "",
                ForeColor = Color.OrangeRed,
                Location = new Point(20, 220),
                Size = new Size(340, 20),
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(_lblStatus);

            // Progress Bar
            _progressBar = new ProgressBar
            {
                Location = new Point(20, 220),
                Size = new Size(340, 20),
                Style = ProgressBarStyle.Marquee,
                Visible = false
            };
            this.Controls.Add(_progressBar);

            // Login Button
            _btnLogin = new Button
            {
                Text = "Login",
                Location = new Point(120, 250),
                Size = new Size(80, 30),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            _btnLogin.Click += BtnLogin_Click;
            this.Controls.Add(_btnLogin);

            // Cancel Button
            _btnCancel = new Button
            {
                Text = "Cancel",
                Location = new Point(210, 250),
                Size = new Size(80, 30),
                BackColor = Color.FromArgb(60, 60, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            _btnCancel.Click += (s, e) => this.Close();
            this.Controls.Add(_btnCancel);

            this.AcceptButton = _btnLogin;
            this.CancelButton = _btnCancel;
        }

        private async void BtnLogin_Click(object? sender, EventArgs e)
        {
            var username = _txtUsername.Text.Trim();
            var password = _txtPassword.Text;
            var serverUrl = _txtServerUrl.Text.Trim();

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                _lblStatus.Text = "Please enter username and password";
                return;
            }

            if (string.IsNullOrEmpty(serverUrl))
            {
                _lblStatus.Text = "Please enter server URL";
                return;
            }

            // Update server URL if changed
            if (serverUrl != _configService.ServerUrl)
            {
                _configService.ServerUrl = serverUrl;
            }

            SetLoadingState(true);

            try
            {
                var (accessToken, refreshToken) = await _clientService.LoginAsync(username, password);
                LoginSuccessful = true;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Grpc.Core.RpcException ex)
            {
                _lblStatus.Text = $"Login failed: {ex.Status.Detail}";
            }
            catch (Exception ex)
            {
                _lblStatus.Text = $"Error: {ex.Message}";
            }
            finally
            {
                SetLoadingState(false);
            }
        }

        private void SetLoadingState(bool isLoading)
        {
            _txtUsername.Enabled = !isLoading;
            _txtPassword.Enabled = !isLoading;
            _txtServerUrl.Enabled = !isLoading;
            _btnLogin.Enabled = !isLoading;
            _progressBar.Visible = isLoading;
            _lblStatus.Visible = !isLoading;
        }
    }
}
