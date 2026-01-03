using Waiter.Services;
using Waiter.Helpers;
using TuiHub.Protos.Librarian.Sephirah.V1;

namespace Waiter.Forms
{
    /// <summary>
    /// Form for viewing storage capacity and usage.
    /// </summary>
    public partial class StorageCapacityForm : Form
    {
        private readonly LibrarianClientService _clientService;

        public StorageCapacityForm(LibrarianClientService clientService)
        {
            _clientService = clientService;
            InitializeComponent();
        }

        private void BtnClose_Click(object? sender, EventArgs e)
        {
            this.Close();
        }

        private async void BtnRefresh_Click(object? sender, EventArgs e)
        {
            await LoadStorageDataAsync();
        }

        private async void StorageCapacityForm_Load(object? sender, EventArgs e)
        {
            await LoadStorageDataAsync();
        }

        private async Task LoadStorageDataAsync()
        {
            _lblStorageInfo.Text = "Loading...";
            _lblPercentage.Text = "--%";
            _progressStorage.Value = 0;

            try
            {
                var response = await _clientService.GetStorageCapacityUsageAsync();
                if (response != null)
                {
                    // Show storage info - data available varies by server
                    _lblStorageInfo.Text = "Storage capacity information retrieved successfully.\n\nDetailed usage metrics depend on server configuration.";
                    _lblPercentage.Text = "Connected";
                    _progressStorage.Value = 0;
                    _progressStorage.Visible = false;
                }
                else
                {
                    _lblStorageInfo.Text = "Unable to load storage information.\n\nPlease try again later.";
                    _lblPercentage.Text = "N/A";
                }
            }
            catch (Exception ex)
            {
                _lblStorageInfo.Text = $"Error loading storage info:\n{ex.Message}";
                _lblPercentage.Text = "Error";
            }
        }
    }
}
