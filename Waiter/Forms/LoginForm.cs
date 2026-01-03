using Waiter.Services;

namespace Waiter.Forms
{
    public partial class LoginForm : Form
    {
        private readonly LibrarianClientService _clientService;
        private readonly TokenService _tokenService;
        private readonly ConfigService _configService;

        public bool LoginSuccessful { get; private set; }

        public LoginForm(LibrarianClientService clientService, TokenService tokenService, ConfigService configService)
        {
            _clientService = clientService;
            _tokenService = tokenService;
            _configService = configService;

            InitializeComponent();
            _txtServerUrl.Text = _configService.ServerUrl;
        }

        private void TxtPassword_KeyPress(object? sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                BtnLogin_Click(sender, e);
            }
        }

        private void BtnCancel_Click(object? sender, EventArgs e)
        {
            this.Close();
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
