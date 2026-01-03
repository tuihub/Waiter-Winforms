using Waiter.Services;
using Waiter.Helpers;
using TuiHub.Protos.Librarian.Sephirah.V1;

namespace Waiter.Forms
{
    /// <summary>
    /// Form for viewing and editing app details.
    /// </summary>
    public partial class AppDetailForm : Form
    {
        private readonly LibrarianClientService _clientService;
        private readonly App? _existingApp;
        private readonly bool _isEditMode;

        public App? ResultApp { get; private set; }
        public bool IsDeleted { get; private set; }

        public AppDetailForm(LibrarianClientService clientService, App? existingApp = null)
        {
            _clientService = clientService;
            _existingApp = existingApp;
            _isEditMode = existingApp != null;
            InitializeComponent();

            // Update form title and button text based on mode
            this.Text = _isEditMode ? "Edit App" : "Add New App";
            _btnSave.Text = _isEditMode ? "Save" : "Create";
            _btnDelete.Visible = _isEditMode;
            _cboType.SelectedIndex = 0;
        }

        private void BtnCancel_Click(object? sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void AppDetailForm_Load(object? sender, EventArgs e)
        {
            if (_existingApp != null)
            {
                _txtName.Text = _existingApp.Name;
                _txtDescription.Text = _existingApp.Description;
                _txtPublisher.Text = _existingApp.Publisher;

                var typeIndex = _existingApp.Type == AppType.Game ? 0 : 1;
                _cboType.SelectedIndex = typeIndex;
            }
        }

        private async void BtnSave_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_txtName.Text))
            {
                MessageBox.Show("Please enter an app name.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var appType = _cboType.SelectedIndex == 0 ? AppType.Game : AppType.Unspecified;

            var app = new App
            {
                Name = _txtName.Text.Trim(),
                Description = _txtDescription.Text.Trim(),
                Publisher = _txtPublisher.Text.Trim(),
                Type = appType
            };

            try
            {
                if (_isEditMode && _existingApp != null)
                {
                    app.Id = _existingApp.Id;
                    var success = await _clientService.UpdateAppAsync(app);
                    if (success)
                    {
                        ResultApp = app;
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Failed to update app.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    var appId = await _clientService.CreateAppAsync(app);
                    if (appId != null)
                    {
                        app.Id = appId;
                        ResultApp = app;
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Failed to create app.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void BtnDelete_Click(object? sender, EventArgs e)
        {
            if (_existingApp == null) return;

            var result = MessageBox.Show(
                $"Are you sure you want to delete '{_existingApp.Name}'?\n\nThis action cannot be undone.",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                try
                {
                    var success = await _clientService.DeleteAppAsync(_existingApp.Id);
                    if (success)
                    {
                        IsDeleted = true;
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Failed to delete app.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
