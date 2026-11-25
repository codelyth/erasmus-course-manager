namespace ErasmusCourseManager.WinForms.Styling;

public static class AppTheme
{
    public static Color Background { get; } = Color.FromArgb(245, 247, 251);
    public static Color Surface { get; } = Color.White;
    public static Color Primary { get; } = Color.FromArgb(30, 58, 95);
    public static Color Text { get; } = Color.FromArgb(28, 39, 56);
    public static Color MutedText { get; } = Color.FromArgb(94, 108, 128);
    public static Color Error { get; } = Color.FromArgb(174, 34, 48);

    public static void Apply(Form form)
    {
        form.BackColor = Background;
        form.ForeColor = Text;
        form.Font = new Font("Segoe UI", 10F);
        form.AutoScaleMode = AutoScaleMode.Dpi;
        form.StartPosition = FormStartPosition.CenterScreen;
    }

    public static void ApplyPrimaryButton(Button button)
    {
        button.BackColor = Primary;
        button.ForeColor = Color.White;
        button.FlatStyle = FlatStyle.Flat;
        button.FlatAppearance.BorderSize = 0;
        button.UseVisualStyleBackColor = false;
        button.Cursor = Cursors.Hand;
        button.Height = 44;
    }
}
