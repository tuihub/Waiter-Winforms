using Waiter.Services;

namespace Waiter.Forms
{
    public partial class SettingsForm : Form
    {
        private readonly ConfigService _configService;
        private readonly TokenService _tokenService;

        public SettingsForm(ConfigService configService, TokenService tokenService)
        {
            _configService = configService;
            _tokenService = tokenService;

            InitializeComponent();
            LoadSettings();
        }

        private void LoadSettings()
        {
            _txtServerUrl.Text = _configService.ServerUrl;
            _btnLogout.Enabled = _tokenService.IsLoggedIn;
        }

        private void BtnCancel_Click(object? sender, EventArgs e)
        {
            this.Close();
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
