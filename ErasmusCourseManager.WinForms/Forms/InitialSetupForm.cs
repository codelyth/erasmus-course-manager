using ErasmusCourseManager.WinForms.Services;
using ErasmusCourseManager.WinForms.Styling;

namespace ErasmusCourseManager.WinForms.Forms;

// This form is built in code; it does not need a Designer.cs or resx file.
public class InitialSetupForm : Form
{
    private readonly UserManager _users;
    private readonly TextBox _fullName = new();
    private readonly TextBox _email = new();
    private readonly TextBox _password = new();
    private readonly TextBox _confirmPassword = new();
    private readonly Button _createButton = new();
    private readonly Label _error = new();

    public InitialSetupForm(UserManager users)
    {
        _users = users;

        AppTheme.Apply(this);
        Text = "Erasmus Course Manager - Initial Setup";
        ClientSize = new Size(560, 620);
        MinimumSize = new Size(540, 640);
        MaximizeBox = false;

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(32),
            ColumnCount = 1,
            RowCount = 13
        };

        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 64));

        for (int i = 0; i < 4; i++)
        {
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));
        }

        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 32));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 52));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        layout.Controls.Add(new Label
        {
            Text = "Create your administrator account",
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft
        }, 0, 0);

        layout.Controls.Add(new Label
        {
            Text = "Set up the first administrator to manage users, " +
                   "courses and registrations. This setup runs only once.",
            Dock = DockStyle.Fill,
            ForeColor = AppTheme.MutedText
        }, 0, 1);

        AddField(layout, "Full name", _fullName, 2);
        AddField(layout, "Email address", _email, 4);
        AddField(layout, "Password", _password, 6);
        AddField(layout, "Confirm password", _confirmPassword, 8);

        _password.UseSystemPasswordChar = true;
        _confirmPassword.UseSystemPasswordChar = true;

        _createButton.Text = "Create administrator";
        _createButton.Dock = DockStyle.Fill;
        AppTheme.ApplyPrimaryButton(_createButton);
        _createButton.Click += CreateAdministrator;
        layout.Controls.Add(_createButton, 0, 11);

        _error.ForeColor = AppTheme.Error;
        _error.Dock = DockStyle.Fill;
        _error.Padding = new Padding(0, 8, 0, 0);
        layout.Controls.Add(_error, 0, 12);

        AcceptButton = _createButton;
        Controls.Add(layout);
    }

    private static void AddField(
        TableLayoutPanel layout,
        string label,
        TextBox input,
        int row)
    {
        layout.Controls.Add(new Label
        {
            Text = label,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft
        }, 0, row);

        input.Dock = DockStyle.Top;
        input.Margin = new Padding(0, 2, 0, 8);
        layout.Controls.Add(input, 0, row + 1);
    }

    private void CreateAdministrator(object? sender, EventArgs e)
    {
        _error.Text = string.Empty;

        if (_password.Text != _confirmPassword.Text)
        {
            _error.Text = "Passwords do not match.";
            _confirmPassword.Focus();
            return;
        }

        _createButton.Enabled = false;
        UseWaitCursor = true;

        try
        {
            _users.CreateInitialAdministrator(
                _fullName.Text,
                _email.Text,
                _password.Text);

            // Close successfully only after persistence succeeds.
            DialogResult = DialogResult.OK;
            Close();
        }
        catch (Exception ex)
        {
            _error.Text = ex.Message;
        }
        finally
        {
            _createButton.Enabled = true;
            UseWaitCursor = false;
            _password.Clear();
            _confirmPassword.Clear();
        }
    }
}
