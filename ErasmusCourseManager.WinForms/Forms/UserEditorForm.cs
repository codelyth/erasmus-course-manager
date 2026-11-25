using ErasmusCourseManager.WinForms.Models;
using ErasmusCourseManager.WinForms.Services;
using ErasmusCourseManager.WinForms.Styling;

namespace ErasmusCourseManager.WinForms.Forms;

public class UserEditorForm : Form
{
    private readonly UserManager _users;
    private readonly User? _existing;
    private readonly ComboBox _role = new();
    private readonly TextBox _name = new();
    private readonly TextBox _email = new();
    private readonly TextBox _password = new();
    private readonly TextBox _confirmPassword = new();
    private readonly TextBox _studentNumber = new();
    private readonly TextBox _university = new();
    private readonly TextBox _department = new();
    private readonly CheckBox _active = new() { Text = "Account active", Checked = true };
    private readonly Panel _studentNumberField;
    private readonly Panel _universityField;
    private readonly Panel _departmentField;

    public UserEditorForm(UserManager users, User? existing = null)
    {
        _users = users;
        _existing = existing;
        AppTheme.Apply(this);
        Text = existing is null ? "Create user" : "Edit user";
        ClientSize = new Size(550, 680);
        MinimumSize = new Size(500, 520);

        var fields = FormLayout.CreateFields();
        _role.DropDownStyle = ComboBoxStyle.DropDownList;
        _role.Items.AddRange(["Student", "Academician", "Administrator"]);
        FormLayout.AddField(fields, "Role", _role);
        FormLayout.AddField(fields, "Full name", _name);
        FormLayout.AddField(fields, "Email address", _email);
        var passwordField = FormLayout.AddField(fields, "Password", _password);
        var confirmField = FormLayout.AddField(fields, "Confirm password", _confirmPassword);
        _password.UseSystemPasswordChar = true;
        _confirmPassword.UseSystemPasswordChar = true;
        passwordField.Visible = existing is null;
        confirmField.Visible = existing is null;
        _studentNumberField = FormLayout.AddField(fields, "Student number", _studentNumber);
        _universityField = FormLayout.AddField(fields, "Home university", _university);
        _departmentField = FormLayout.AddField(fields, "Department", _department);
        var activeField = FormLayout.AddField(fields, "Account status", _active);
        activeField.Visible = existing is not null;

        var actions = new FlowLayoutPanel
        {
            Dock = DockStyle.Bottom,
            Height = 72,
            Padding = new Padding(24, 12, 24, 12)
        };
        var save = FormLayout.CreateButton("Save", SaveUser);
        var cancel = FormLayout.CreateButton("Cancel", (_, _) => Close());
        cancel.DialogResult = DialogResult.Cancel;
        actions.Controls.Add(save);
        actions.Controls.Add(cancel);
        AcceptButton = save;
        CancelButton = cancel;
        Controls.Add(fields);
        Controls.Add(actions);

        if (existing is not null)
        {
            _name.Text = existing.FullName;
            _email.Text = existing.Email;
            _active.Checked = existing.IsActive;
            if (existing is Student student)
            {
                _studentNumber.Text = student.StudentNumber;
                _university.Text = student.HomeUniversity;
            }
            if (existing is Academician academician)
                _department.Text = academician.Department;
        }

        _role.SelectedIndexChanged += (_, _) => UpdateRoleFields();
        _role.SelectedIndex = existing switch
        {
            Academician => 1,
            Administrator => 2,
            _ => 0
        };
        _role.Enabled = existing is null;
        UpdateRoleFields();
    }

    private void UpdateRoleFields()
    {
        _studentNumberField.Visible = _role.SelectedIndex == 0;
        _universityField.Visible = _role.SelectedIndex == 0;
        _departmentField.Visible = _role.SelectedIndex == 1;
    }

    private void SaveUser(object? sender, EventArgs e)
    {
        try
        {
            if (_existing is null)
            {
                if (_password.Text != _confirmPassword.Text)
                    throw new InvalidOperationException("Passwords do not match.");

                switch (_role.SelectedIndex)
                {
                    case 0:
                        _users.AddStudent(_name.Text, _email.Text, _password.Text,
                            _studentNumber.Text, _university.Text);
                        break;
                    case 1:
                        _users.AddAcademician(_name.Text, _email.Text,
                            _password.Text, _department.Text);
                        break;
                    case 2:
                        _users.AddAdministrator(_name.Text, _email.Text, _password.Text);
                        break;
                    default:
                        throw new InvalidOperationException("Select a role.");
                }
            }
            else
            {
                User updated = _existing switch
                {
                    Student => new Student(_existing.Id, _name.Text, _email.Text,
                        _existing.PasswordHash, _studentNumber.Text,
                        _university.Text, _active.Checked),
                    Academician => new Academician(_existing.Id, _name.Text,
                        _email.Text, _existing.PasswordHash,
                        _department.Text, _active.Checked),
                    Administrator => new Administrator(_existing.Id, _name.Text,
                        _email.Text, _existing.PasswordHash, _active.Checked),
                    _ => throw new InvalidOperationException("Unsupported user type.")
                };
                _users.UpdateUser(updated);
            }
            DialogResult = DialogResult.OK;
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Could not save user",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        finally
        {
            _password.Clear();
            _confirmPassword.Clear();
        }
    }
}
