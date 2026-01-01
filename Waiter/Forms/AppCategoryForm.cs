using Waiter.Services;
using TuiHub.Protos.Librarian.Sephirah.V1.Sephirah;

namespace Waiter.Forms
{
    public partial class AppCategoryForm : Form
    {
        private readonly LibrarianClientService _clientService;

        private ListView _listViewCategories = null!;
        private Button _btnAdd = null!;
        private Button _btnEdit = null!;
        private Button _btnDelete = null!;
        private Button _btnRefresh = null!;
        private Button _btnClose = null!;
        private Label _lblStatus = null!;

        private List<AppCategory> _categories = new();

        public AppCategoryForm(LibrarianClientService clientService)
        {
            _clientService = clientService;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.Text = "App Category Management";
            this.Size = new Size(600, 450);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(30, 30, 30);

            // Title
            var lblTitle = new Label
            {
                Text = "App Categories",
                Font = new Font("Segoe UI", 16, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(20, 15),
                AutoSize = true
            };
            this.Controls.Add(lblTitle);

            // ListView
            _listViewCategories = new ListView
            {
                Location = new Point(20, 55),
                Size = new Size(440, 300),
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                BackColor = Color.FromArgb(45, 45, 45),
                ForeColor = Color.White
            };
            _listViewCategories.Columns.Add("ID", 80);
            _listViewCategories.Columns.Add("Name", 200);
            _listViewCategories.Columns.Add("Apps Count", 80);
            _listViewCategories.Columns.Add("Version", 60);
            _listViewCategories.SelectedIndexChanged += ListView_SelectedIndexChanged;
            this.Controls.Add(_listViewCategories);

            // Buttons Panel
            _btnAdd = new Button
            {
                Text = "Add",
                Location = new Point(480, 55),
                Size = new Size(90, 30),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            _btnAdd.Click += BtnAdd_Click;
            this.Controls.Add(_btnAdd);

            _btnEdit = new Button
            {
                Text = "Edit",
                Location = new Point(480, 95),
                Size = new Size(90, 30),
                BackColor = Color.FromArgb(60, 60, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Enabled = false
            };
            _btnEdit.Click += BtnEdit_Click;
            this.Controls.Add(_btnEdit);

            _btnDelete = new Button
            {
                Text = "Delete",
                Location = new Point(480, 135),
                Size = new Size(90, 30),
                BackColor = Color.FromArgb(180, 50, 50),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Enabled = false
            };
            _btnDelete.Click += BtnDelete_Click;
            this.Controls.Add(_btnDelete);

            _btnRefresh = new Button
            {
                Text = "Refresh",
                Location = new Point(480, 185),
                Size = new Size(90, 30),
                BackColor = Color.FromArgb(60, 60, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            _btnRefresh.Click += BtnRefresh_Click;
            this.Controls.Add(_btnRefresh);

            // Status Label
            _lblStatus = new Label
            {
                Text = "",
                ForeColor = Color.LightGray,
                Location = new Point(20, 365),
                Size = new Size(440, 20)
            };
            this.Controls.Add(_lblStatus);

            // Close Button
            _btnClose = new Button
            {
                Text = "Close",
                Location = new Point(480, 370),
                Size = new Size(90, 30),
                BackColor = Color.FromArgb(60, 60, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            _btnClose.Click += (s, e) => this.Close();
            this.Controls.Add(_btnClose);

            this.Load += AppCategoryForm_Load;
        }

        private async void AppCategoryForm_Load(object? sender, EventArgs e)
        {
            await LoadCategoriesAsync();
        }

        private async Task LoadCategoriesAsync()
        {
            _lblStatus.Text = "Loading categories...";
            _listViewCategories.Items.Clear();

            try
            {
                var response = await _clientService.ListAppCategoriesAsync();
                if (response != null)
                {
                    _categories = response.AppCategories.ToList();
                    foreach (var category in _categories)
                    {
                        var item = new ListViewItem(category.Id.Id.ToString());
                        item.SubItems.Add(category.Name);
                        item.SubItems.Add(category.AppIds.Count.ToString());
                        item.SubItems.Add(category.VersionNumber.ToString());
                        item.Tag = category;
                        _listViewCategories.Items.Add(item);
                    }
                    _lblStatus.Text = $"Loaded {_categories.Count} categories";
                }
                else
                {
                    _lblStatus.Text = "Failed to load categories";
                }
            }
            catch (Exception ex)
            {
                _lblStatus.Text = $"Error: {ex.Message}";
            }
        }

        private void ListView_SelectedIndexChanged(object? sender, EventArgs e)
        {
            var hasSelection = _listViewCategories.SelectedItems.Count > 0;
            _btnEdit.Enabled = hasSelection;
            _btnDelete.Enabled = hasSelection;
        }

        private async void BtnAdd_Click(object? sender, EventArgs e)
        {
            using var dialog = new CategoryEditDialog(null);
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                var category = new AppCategory
                {
                    Name = dialog.CategoryName
                };
                // Note: AppIds would need to be added separately

                try
                {
                    var id = await _clientService.CreateAppCategoryAsync(category);
                    if (id != null)
                    {
                        _lblStatus.Text = "Category created successfully";
                        await LoadCategoriesAsync();
                    }
                    else
                    {
                        _lblStatus.Text = "Failed to create category";
                    }
                }
                catch (Exception ex)
                {
                    _lblStatus.Text = $"Error: {ex.Message}";
                }
            }
        }

        private async void BtnEdit_Click(object? sender, EventArgs e)
        {
            if (_listViewCategories.SelectedItems.Count == 0) return;

            var selectedItem = _listViewCategories.SelectedItems[0];
            if (selectedItem.Tag is not AppCategory category) return;

            using var dialog = new CategoryEditDialog(category);
            if (dialog.ShowDialog(this) == DialogResult.OK)
            {
                category.Name = dialog.CategoryName;

                try
                {
                    var success = await _clientService.UpdateAppCategoryAsync(category);
                    if (success)
                    {
                        _lblStatus.Text = "Category updated successfully";
                        await LoadCategoriesAsync();
                    }
                    else
                    {
                        _lblStatus.Text = "Failed to update category";
                    }
                }
                catch (Exception ex)
                {
                    _lblStatus.Text = $"Error: {ex.Message}";
                }
            }
        }

        private async void BtnDelete_Click(object? sender, EventArgs e)
        {
            if (_listViewCategories.SelectedItems.Count == 0) return;

            var selectedItem = _listViewCategories.SelectedItems[0];
            if (selectedItem.Tag is not AppCategory category) return;

            var result = MessageBox.Show(
                $"Are you sure you want to delete category '{category.Name}'?",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                try
                {
                    var success = await _clientService.DeleteAppCategoryAsync(category.Id);
                    if (success)
                    {
                        _lblStatus.Text = "Category deleted successfully";
                        await LoadCategoriesAsync();
                    }
                    else
                    {
                        _lblStatus.Text = "Failed to delete category";
                    }
                }
                catch (Exception ex)
                {
                    _lblStatus.Text = $"Error: {ex.Message}";
                }
            }
        }

        private async void BtnRefresh_Click(object? sender, EventArgs e)
        {
            await LoadCategoriesAsync();
        }
    }

    /// <summary>
    /// Dialog for adding/editing category
    /// </summary>
    public class CategoryEditDialog : Form
    {
        private TextBox _txtName = null!;
        private Button _btnOk = null!;
        private Button _btnCancel = null!;

        public string CategoryName => _txtName.Text.Trim();

        public CategoryEditDialog(AppCategory? category)
        {
            InitializeComponent();

            if (category != null)
            {
                this.Text = "Edit Category";
                _txtName.Text = category.Name;
            }
            else
            {
                this.Text = "Add Category";
            }
        }

        private void InitializeComponent()
        {
            this.Size = new Size(350, 150);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.FromArgb(30, 30, 30);
            this.StartPosition = FormStartPosition.CenterParent;

            var lblName = new Label
            {
                Text = "Category Name:",
                ForeColor = Color.LightGray,
                Location = new Point(20, 20),
                AutoSize = true
            };
            this.Controls.Add(lblName);

            _txtName = new TextBox
            {
                Location = new Point(20, 40),
                Size = new Size(290, 25),
                BackColor = Color.FromArgb(45, 45, 45),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            this.Controls.Add(_txtName);

            _btnOk = new Button
            {
                Text = "OK",
                Location = new Point(130, 75),
                Size = new Size(80, 30),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.OK
            };
            this.Controls.Add(_btnOk);

            _btnCancel = new Button
            {
                Text = "Cancel",
                Location = new Point(220, 75),
                Size = new Size(80, 30),
                BackColor = Color.FromArgb(60, 60, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.Cancel
            };
            this.Controls.Add(_btnCancel);

            this.AcceptButton = _btnOk;
            this.CancelButton = _btnCancel;
        }
    }
}
