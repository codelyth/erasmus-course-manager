using ErasmusCourseManager.WinForms.Models;
using ErasmusCourseManager.WinForms.Services;
using ErasmusCourseManager.WinForms.Styling;

namespace ErasmusCourseManager.WinForms.Forms;

public class UserManagementForm : Form
{
    private readonly AppServices _services;
    private readonly TextBox _search = new();
    private readonly ComboBox _roleFilter = new();
    private readonly DataGridView _grid = new();
    private readonly Label _status = new();

    public UserManagementForm(AppServices services)
    {
        services.Session.RequireAdministrator();
        _services = services;
        AppTheme.Apply(this);
        Text = "User management";
        ClientSize = new Size(1120, 650);
        MinimumSize = new Size(930, 540);

        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(24),
            ColumnCount = 1,
            RowCount = 4
        };
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 56));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 48));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));

        var actions = new FlowLayoutPanel { Dock = DockStyle.Fill };
        actions.Controls.Add(FormLayout.CreateButton("Add User", (_, _) => RunAction(AddUser)));
        actions.Controls.Add(FormLayout.CreateButton("Edit", (_, _) => RunAction(EditUser)));
        actions.Controls.Add(FormLayout.CreateButton("Delete", (_, _) => RunAction(DeleteUser)));
        actions.Controls.Add(FormLayout.CreateButton("Reset Password", (_, _) => RunAction(ResetPassword)));
        root.Controls.Add(actions, 0, 0);

        var filters = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1
        };
        filters.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        filters.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 190));
        _search.PlaceholderText = "Search by name, email or student number...";
        _search.Dock = DockStyle.Top;
        _roleFilter.DropDownStyle = ComboBoxStyle.DropDownList;
        _roleFilter.Items.AddRange(["All roles", "Student", "Academician", "Administrator"]);
        _roleFilter.Dock = DockStyle.Top;
        _roleFilter.SelectedIndex = 0;
        _search.TextChanged += (_, _) => RefreshUsers();
        _roleFilter.SelectedIndexChanged += (_, _) => RefreshUsers();
        filters.Controls.Add(_search, 0, 0);
        filters.Controls.Add(_roleFilter, 1, 0);
        root.Controls.Add(filters, 0, 1);

        FormLayout.ConfigureGrid(_grid);
        AddColumn("Id", "ID", 30);
        AddColumn("FullName", "Full Name", 100);
        AddColumn("Email", "Email", 130);
        AddColumn("Role", "Role", 70);
        AddColumn("StudentNumber", "Student Number", 70);
        AddColumn("Details", "University / Department", 130);
        AddColumn("Status", "Status", 50);
        root.Controls.Add(_grid, 0, 2);
        _status.Dock = DockStyle.Fill;
        _status.TextAlign = ContentAlignment.MiddleLeft;
        _status.ForeColor = AppTheme.MutedText;
        root.Controls.Add(_status, 0, 3);
        Controls.Add(root);
        Shown += (_, _) => RefreshUsers();
    }

    private void AddColumn(string property, string title, float weight)
    {
        _grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = property,
            DataPropertyName = property,
            HeaderText = title,
            FillWeight = weight
        });
    }

    private void RefreshUsers()
    {
        try
        {
            string? selectedRole = _roleFilter.SelectedIndex == 0
                ? null : _roleFilter.SelectedItem?.ToString();
            var rows = _services.Users.SearchUsers(_search.Text)
                .Where(user => selectedRole is null || user.GetType().Name == selectedRole)
                .Select(user => new
                {
                    user.Id,
                    user.FullName,
                    user.Email,
                    Role = user.GetType().Name,
                    StudentNumber = user is Student student ? student.StudentNumber : "",
                    Details = user switch
                    {
                        Student s => s.HomeUniversity,
                        Academician a => a.Department,
                        _ => ""
                    },
                    Status = user.IsActive ? "Active" : "Inactive"
                }).ToList();
            _grid.DataSource = rows;
            _status.Text = $"{rows.Count} User(s)";
        }
        catch (Exception ex)
        {
            _grid.DataSource = null;
            _status.Text = ex.Message;
        }
    }

    private int SelectedUserId()
    {
        if (_grid.CurrentRow?.Cells["Id"].Value is int id)
            return id;
        throw new InvalidOperationException("Select a user first.");
    }

    private void AddUser()
    {
        using var editor = new UserEditorForm(_services.Users);
        if (editor.ShowDialog(this) == DialogResult.OK)
            RefreshUsers();
    }

    private void EditUser()
    {
        User user = _services.Users.GetUserById(SelectedUserId());
        using var editor = new UserEditorForm(_services.Users, user);
        if (editor.ShowDialog(this) == DialogResult.OK)
            RefreshUsers();
    }

    private void DeleteUser()
    {
        User user = _services.Users.GetUserById(SelectedUserId());
        if (MessageBox.Show(this,
                $"Delete the account for {user.FullName}? This cannot be undone.",
                "Delete User", MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
            return;
        _services.Users.RemoveUser(user.Id);
        RefreshUsers();
    }

    private void ResetPassword()
    {
        using var dialog = new ResetPasswordForm(_services.Users, SelectedUserId());
        if (dialog.ShowDialog(this) == DialogResult.OK)
            _status.Text = "Password reset successfully.";
    }

    private void RunAction(Action action)
    {
        try
        {
            _services.Session.RequireAdministrator();
            action();
            if (!_services.Session.IsLoggedIn)
                Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "User Management",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
