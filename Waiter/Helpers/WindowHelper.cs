namespace Waiter.Helpers
{
    /// <summary>
    /// Helper class for window-related operations.
    /// </summary>
    public static class WindowHelper
    {
        /// <summary>
        /// Shows a form centered on the parent form.
        /// </summary>
        public static void ShowCenteredDialog(Form childForm, Form parentForm)
        {
            childForm.StartPosition = FormStartPosition.Manual;
            childForm.Location = new Point(
                parentForm.Location.X + (parentForm.Width - childForm.Width) / 2,
                parentForm.Location.Y + (parentForm.Height - childForm.Height) / 2
            );
            childForm.ShowDialog(parentForm);
        }

        /// <summary>
        /// Shows a form centered on the parent form (non-modal).
        /// </summary>
        public static void ShowCentered(Form childForm, Form parentForm)
        {
            childForm.StartPosition = FormStartPosition.Manual;
            childForm.Location = new Point(
                parentForm.Location.X + (parentForm.Width - childForm.Width) / 2,
                parentForm.Location.Y + (parentForm.Height - childForm.Height) / 2
            );
            childForm.Show(parentForm);
        }

        /// <summary>
        /// Creates and shows a new form centered on the parent form.
        /// </summary>
        public static TForm CreateAndShowCenteredDialog<TForm>(Form parentForm) where TForm : Form, new()
        {
            var form = new TForm();
            ShowCenteredDialog(form, parentForm);
            return form;
        }
    }
}
