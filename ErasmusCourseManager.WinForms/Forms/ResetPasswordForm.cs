using ErasmusCourseManager.WinForms.Services;
using ErasmusCourseManager.WinForms.Styling;

namespace ErasmusCourseManager.WinForms.Forms;

public class ResetPasswordForm : Form
{
    private readonly UserManager _users;
    private readonly int _userId;
    private readonly TextBox _password = new() { UseSystemPasswordChar = true };
    private readonly TextBox _confirm = new() { UseSystemPasswordChar = true };

    public ResetPasswordForm(UserManager users, int userId)
    {
        _users = users;
        _userId = userId;
        var user = users.GetUserById(userId);
        AppTheme.Apply(this);
        Text = $"Reset Password - {user.FullName}";
        ClientSize = new Size(530, 300);
        MinimumSize = new Size(500, 330);

        var fields = FormLayout.CreateFields();
        FormLayout.AddField(fields, "New Password", _password);
        FormLayout.AddField(fields, "Confirm New Password", _confirm);
        var actions = new FlowLayoutPanel
        {
            Dock = DockStyle.Bottom,
            Height = 72,
            Padding = new Padding(24, 12, 24, 12)
        };
        var save = FormLayout.CreateButton("Reset Password", SavePassword);
        var cancel = FormLayout.CreateButton("Cancel", (_, _) => Close());
        cancel.DialogResult = DialogResult.Cancel;
        actions.Controls.Add(save);
        actions.Controls.Add(cancel);
        Controls.Add(fields);
        Controls.Add(actions);
        AcceptButton = save;
        CancelButton = cancel;
    }

    private void SavePassword(object? sender, EventArgs e)
    {
        try
        {
            if (_password.Text != _confirm.Text)
                throw new InvalidOperationException("Passwords do not match.");

            _users.ChangePassword(_userId, _password.Text);
            DialogResult = DialogResult.OK;
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Could not reset password",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        finally
        {
            _password.Clear();
            _confirm.Clear();
        }
    }
}
