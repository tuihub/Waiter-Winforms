using TuiHub.Protos.Librarian.Sephirah.V1;

namespace Waiter.Forms
{
    /// <summary>
    /// Dialog for adding/editing category
    /// </summary>
    public partial class CategoryEditDialog : Form
    {
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
    }
}
