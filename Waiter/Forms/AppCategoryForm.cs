using Waiter.Services;
using TuiHub.Protos.Librarian.Sephirah.V1;

namespace Waiter.Forms
{
    public partial class AppCategoryForm : Form
    {
        private readonly LibrarianClientService _clientService;

        private List<AppCategory> _categories = new();

        public AppCategoryForm(LibrarianClientService clientService)
        {
            _clientService = clientService;
            InitializeComponent();
        }

        private void BtnClose_Click(object? sender, EventArgs e)
        {
            this.Close();
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
}
