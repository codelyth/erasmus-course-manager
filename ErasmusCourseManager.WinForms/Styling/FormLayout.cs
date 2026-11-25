namespace ErasmusCourseManager.WinForms.Styling;

// Shared layout helpers for editor dialogs.
public static class FormLayout
{
    public static FlowLayoutPanel CreateFields()
    {
        return new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            AutoScroll = true,
            Padding = new Padding(24)
        };
    }

    public static Panel AddField(
        FlowLayoutPanel fields, string caption, Control input)
    {
        var field = new Panel
        {
            Width = 450,
            Height = 76,
            Margin = new Padding(0, 0, 0, 4)
        };
        var label = new Label
        {
            Text = caption,
            Dock = DockStyle.Top,
            Height = 28
        };
        input.Dock = DockStyle.Top;
        var inputArea = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(0, 4, 0, 0)
        };
        inputArea.Controls.Add(input);
        field.Controls.Add(inputArea);
        field.Controls.Add(label);
        fields.Controls.Add(field);

        fields.SizeChanged += (_, _) =>
            field.Width = Math.Max(200,
                fields.ClientSize.Width - fields.Padding.Horizontal - 24);
        return field;
    }

    public static Button CreateButton(string caption, EventHandler click)
    {
        var button = new Button
        {
            Text = caption,
            AutoSize = true,
            MinimumSize = new Size(110, 40),
            Margin = new Padding(0, 0, 10, 0)
        };
        AppTheme.ApplyPrimaryButton(button);
        button.Click += click;
        return button;
    }

    public static void ConfigureGrid(DataGridView grid)
    {
        grid.Dock = DockStyle.Fill;
        grid.ReadOnly = true;
        grid.AllowUserToAddRows = false;
        grid.AllowUserToDeleteRows = false;
        grid.AllowUserToResizeRows = false;
        grid.MultiSelect = false;
        grid.RowHeadersVisible = false;
        grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        grid.BackgroundColor = AppTheme.Surface;
        grid.BorderStyle = BorderStyle.None;
        grid.AutoGenerateColumns = false;
    }
}
