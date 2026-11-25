using ErasmusCourseManager.WinForms.Services;
using ErasmusCourseManager.WinForms.Styling;

namespace ErasmusCourseManager.WinForms.Forms;

public class LoginForm : Form
{
    private readonly AppServices _services;
    private readonly TextBox _email = new();
    private readonly TextBox _password = new();
    private readonly Button _loginButton = new();
    private readonly Label _error = new();

    public LoginForm(AppServices services)
    {
        _services = services;
        AppTheme.Apply(this);
        Text = "Erasmus Course Manager - Sign in";
        ClientSize = new Size(900, 540);
        MinimumSize = new Size(840, 560);

        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1
        };
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 42));
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 58));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        var brand = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            BackColor = AppTheme.Primary,
            ForeColor = Color.White,
            Padding = new Padding(30),
            ColumnCount = 1,
            RowCount = 3
        };
        brand.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        brand.RowStyles.Add(new RowStyle(SizeType.Percent, 35));
        brand.RowStyles.Add(new RowStyle(SizeType.Absolute, 160));
        brand.RowStyles.Add(new RowStyle(SizeType.Percent, 65));
        brand.Controls.Add(new Label
        {
            Text = "ERASMUS\nCOURSE MANAGER",
            Font = new Font(Font.FontFamily, 22F, FontStyle.Bold),
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft
        }, 0, 1);
        brand.Controls.Add(new Label
        {
            Text = "Your courses. Your Erasmus journey.",
            Dock = DockStyle.Fill
        }, 0, 2);

        var fields = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(36),
            ColumnCount = 1,
            RowCount = 10,
            BackColor = AppTheme.Surface
        };
        fields.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        float[] heights = [54, 42, 28, 44, 28, 44, 36, 52];
        foreach (float height in heights)
            fields.RowStyles.Add(new RowStyle(SizeType.Absolute, height));
        fields.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        fields.RowStyles.Add(new RowStyle(SizeType.Absolute, 24));

        fields.Controls.Add(new Label
        {
            Text = "Welcome back",
            Dock = DockStyle.Fill,
            Font = new Font(Font.FontFamily, 20F, FontStyle.Bold)
        }, 0, 0);
        fields.Controls.Add(new Label
        {
            Text = "Sign in with your account to continue.",
            ForeColor = AppTheme.MutedText,
            Dock = DockStyle.Fill
        }, 0, 1);

        AddField(fields, "Email address", _email, 2);
        AddField(fields, "Password", _password, 4);
        _password.UseSystemPasswordChar = true;

        var showPassword = new CheckBox
        {
            Text = "Show password",
            Dock = DockStyle.Fill
        };
        showPassword.CheckedChanged += (_, _) =>
            _password.UseSystemPasswordChar = !showPassword.Checked;
        fields.Controls.Add(showPassword, 0, 6);

        _loginButton.Text = "Sign in";
        _loginButton.Dock = DockStyle.Fill;
        AppTheme.ApplyPrimaryButton(_loginButton);
        _loginButton.Click += SignIn;
        fields.Controls.Add(_loginButton, 0, 7);

        _error.ForeColor = AppTheme.Error;
        _error.Dock = DockStyle.Fill;
        _error.Padding = new Padding(0, 10, 0, 0);
        fields.Controls.Add(_error, 0, 8);

        fields.Controls.Add(new Label
        {
            Text = "Need an account? Contact your administrator.",
            ForeColor = AppTheme.MutedText,
            Dock = DockStyle.Fill
        }, 0, 9);

        root.Controls.Add(brand, 0, 0);
        root.Controls.Add(fields, 1, 0);
        Controls.Add(root);
        AcceptButton = _loginButton;
        Shown += (_, _) => _email.Focus();
    }

    private static void AddField(
        TableLayoutPanel layout, string text, TextBox input, int row)
    {
        layout.Controls.Add(new Label
        {
            Text = text,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft
        }, 0, row);
        input.Dock = DockStyle.Top;
        input.Margin = new Padding(0, 2, 0, 8);
        layout.Controls.Add(input, 0, row + 1);
    }

    private void SignIn(object? sender, EventArgs e)
    {
        _error.Text = string.Empty;
        _loginButton.Enabled = false;
        UseWaitCursor = true;

        try
        {
            _services.Auth.LoginWithPassword(_email.Text, _password.Text);
        }
        catch (Exception ex)
        {
            _error.Text = ex.Message;
            return;
        }
        finally
        {
            _password.Clear();
            _loginButton.Enabled = true;
            UseWaitCursor = false;
        }

        try
        {
            using var dashboard = new DashboardForm(_services);
            Hide();
            DialogResult result = dashboard.ShowDialog();

            if (result != DialogResult.Retry)
            {
                Close();
                return;
            }
        }
        catch (Exception ex)
        {
            _error.Text = "Could not open the dashboard. " + ex.Message;
        }
        finally
        {
            _services.Auth.Logout();
        }

        Show();
        Activate();
        _password.Focus();
    }
}
