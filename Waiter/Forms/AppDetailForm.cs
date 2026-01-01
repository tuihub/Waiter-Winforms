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

        // UI Components
        private TextBox _txtName = null!;
        private TextBox _txtDescription = null!;
        private ComboBox _cboType = null!;
        private TextBox _txtPublisher = null!;
        private Button _btnSave = null!;
        private Button _btnCancel = null!;
        private Button _btnDelete = null!;

        public App? ResultApp { get; private set; }
        public bool IsDeleted { get; private set; }

        public AppDetailForm(LibrarianClientService clientService, App? existingApp = null)
        {
            _clientService = clientService;
            _existingApp = existingApp;
            _isEditMode = existingApp != null;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = _isEditMode ? "Edit App" : "Add New App";
            this.Size = new Size(550, 380);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(27, 40, 56);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            CreateFormFields();
            CreateButtonPanel();

            this.Load += AppDetailForm_Load;
        }

        private void CreateFormFields()
        {
            int y = 20;
            int labelWidth = 100;
            int inputWidth = 380;

            // Name
            var lblName = new Label
            {
                Text = "Name:",
                ForeColor = Color.White,
                Location = new Point(20, y + 3),
                Size = new Size(labelWidth, 20)
            };
            _txtName = new TextBox
            {
                Location = new Point(120, y),
                Size = new Size(inputWidth, 25),
                BackColor = Color.FromArgb(45, 60, 80),
                ForeColor = Color.White
            };
            this.Controls.AddRange(new Control[] { lblName, _txtName });
            y += 40;

            // Type
            var lblType = new Label
            {
                Text = "Type:",
                ForeColor = Color.White,
                Location = new Point(20, y + 3),
                Size = new Size(labelWidth, 20)
            };
            _cboType = new ComboBox
            {
                Location = new Point(120, y),
                Size = new Size(200, 25),
                BackColor = Color.FromArgb(45, 60, 80),
                ForeColor = Color.White,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            _cboType.Items.AddRange(new object[] { "Game", "Other" });
            _cboType.SelectedIndex = 0;
            this.Controls.AddRange(new Control[] { lblType, _cboType });
            y += 40;

            // Publisher
            var lblPublisher = new Label
            {
                Text = "Publisher:",
                ForeColor = Color.White,
                Location = new Point(20, y + 3),
                Size = new Size(labelWidth, 20)
            };
            _txtPublisher = new TextBox
            {
                Location = new Point(120, y),
                Size = new Size(inputWidth, 25),
                BackColor = Color.FromArgb(45, 60, 80),
                ForeColor = Color.White
            };
            this.Controls.AddRange(new Control[] { lblPublisher, _txtPublisher });
            y += 40;

            // Description
            var lblDescription = new Label
            {
                Text = "Description:",
                ForeColor = Color.White,
                Location = new Point(20, y + 3),
                Size = new Size(labelWidth, 20)
            };
            _txtDescription = new TextBox
            {
                Location = new Point(120, y),
                Size = new Size(inputWidth, 100),
                BackColor = Color.FromArgb(45, 60, 80),
                ForeColor = Color.White,
                Multiline = true,
                ScrollBars = ScrollBars.Vertical
            };
            this.Controls.AddRange(new Control[] { lblDescription, _txtDescription });
        }

        private void CreateButtonPanel()
        {
            var buttonPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 60,
                BackColor = Color.FromArgb(23, 29, 37)
            };

            // Position buttons relative to button panel, not form
            _btnSave = new Button
            {
                Text = _isEditMode ? "Save" : "Create",
                Size = new Size(100, 35),
                BackColor = Color.FromArgb(76, 175, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Anchor = AnchorStyles.Right | AnchorStyles.Top
            };
            _btnSave.Click += BtnSave_Click;

            _btnCancel = new Button
            {
                Text = "Cancel",
                Size = new Size(100, 35),
                BackColor = Color.FromArgb(60, 60, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Anchor = AnchorStyles.Right | AnchorStyles.Top
            };
            _btnCancel.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };

            if (_isEditMode)
            {
                _btnDelete = new Button
                {
                    Text = "Delete",
                    Location = new Point(20, 15),
                    Size = new Size(100, 35),
                    BackColor = Color.FromArgb(200, 50, 50),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Anchor = AnchorStyles.Left | AnchorStyles.Bottom
                };
                _btnDelete.Click += BtnDelete_Click;
                buttonPanel.Controls.Add(_btnDelete);
            }

            buttonPanel.Controls.AddRange(new Control[] { _btnSave, _btnCancel });
            this.Controls.Add(buttonPanel);

            // Position buttons after panel is added
            buttonPanel.SizeChanged += (s, e) =>
            {
                _btnCancel.Location = new Point(buttonPanel.Width - 120, 15);
                _btnSave.Location = new Point(buttonPanel.Width - 230, 15);
            };
            // Initial positioning
            _btnCancel.Location = new Point(buttonPanel.Width - 120, 15);
            _btnSave.Location = new Point(buttonPanel.Width - 230, 15);
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
