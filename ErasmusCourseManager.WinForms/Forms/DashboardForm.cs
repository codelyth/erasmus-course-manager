using ErasmusCourseManager.WinForms.Models;
using ErasmusCourseManager.WinForms.Services;
using ErasmusCourseManager.WinForms.Styling;

namespace ErasmusCourseManager.WinForms.Forms;

public class DashboardForm : Form
{
    private readonly AppServices _services;
    private readonly TextBox _search = new();
    private readonly DataGridView _courses = new();
    private readonly Label _status = new();

    public DashboardForm(AppServices services)
    {
        _services = services;
        User user = services.Session.RequireUser();
        string role = user switch
        {
            Administrator => "Administrator",
            Academician => "Academician",
            Student => "Student",
            _ => throw new UnauthorizedAccessException("Unsupported user role.")
        };

        AppTheme.Apply(this);
        Text = $"Erasmus Course Manager - {role}";
        ClientSize = new Size(1100, 680);
        MinimumSize = new Size(880, 560);

        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(24),
            ColumnCount = 1,
            RowCount = 5
        };
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 72));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 44));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));

        var header = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1
        };
        header.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        header.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120));
        header.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        header.Controls.Add(new Label
        {
            Text = $"Welcome, {user.FullName}\n{role} · {user.Email}",
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft
        }, 0, 0);

        var logout = new Button
        {
            Text = "Sign out",
            Dock = DockStyle.Top,
            Margin = new Padding(0, 12, 0, 0)
        };
        AppTheme.ApplyPrimaryButton(logout);
        logout.Click += (_, _) =>
        {
            _services.Auth.Logout();
            DialogResult = DialogResult.Retry;
            Close();
        };
        header.Controls.Add(logout, 1, 0);
        root.Controls.Add(header, 0, 0);

        root.Controls.Add(new Label
        {
            Text = user is Academician ? "My assigned courses" : "Course catalogue",
            Font = new Font(Font.FontFamily, 16F, FontStyle.Bold),
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft
        }, 0, 1);

        _search.PlaceholderText = "Search by course code or name...";
        _search.Dock = DockStyle.Top;
        _search.TextChanged += (_, _) => RefreshCourses();
        root.Controls.Add(_search, 0, 2);

        _courses.Dock = DockStyle.Fill;
        _courses.ReadOnly = true;
        _courses.AllowUserToAddRows = false;
        _courses.AllowUserToDeleteRows = false;
        _courses.AllowUserToResizeRows = false;
        _courses.MultiSelect = false;
        _courses.RowHeadersVisible = false;
        _courses.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _courses.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        _courses.BackgroundColor = AppTheme.Surface;
        _courses.BorderStyle = BorderStyle.None;
        _courses.AutoGenerateColumns = false;

        AddColumn("Code", "Code", 65);
        AddColumn("Name", "Course", 180);
        AddColumn("Ects", "ECTS", 45);
        AddColumn("Capacity", "Capacity", 60);
        AddColumn("Available", "Available places", 80);
        root.Controls.Add(_courses, 0, 3);

        _status.Dock = DockStyle.Fill;
        _status.ForeColor = AppTheme.MutedText;
        _status.TextAlign = ContentAlignment.MiddleLeft;
        root.Controls.Add(_status, 0, 4);

        Controls.Add(root);
        Shown += (_, _) => RefreshCourses();
    }

    private void AddColumn(string property, string title, float weight)
    {
        _courses.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = property,
            HeaderText = title,
            FillWeight = weight
        });
    }

    private void RefreshCourses()
    {
        try
        {
            var rows = _services.Courses.SearchCourses(_search.Text)
                .Select(course => new
                {
                    course.Code,
                    course.Name,
                    course.Ects,
                    course.Capacity,
                    Available = _services.Registrations
                        .GetAvailableCapacity(course.Id)
                })
                .ToList();

            _courses.DataSource = rows;
            _status.Text = rows.Count == 0
                ? "No courses found."
                : $"{rows.Count} course(s)";
        }
        catch (Exception ex)
        {
            _courses.DataSource = null;
            _status.Text = ex.Message;
        }
    }
}
