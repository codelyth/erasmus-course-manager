using ErasmusCourseManager.WinForms.Forms;
using ErasmusCourseManager.WinForms.Services;

namespace ErasmusCourseManager.WinForms;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();

        try
        {
            // One shared set of services for the application lifetime.
            var services = new AppServices();

            if (services.Users.NeedsInitialSetup)
            {
                using var setupForm = new InitialSetupForm(services.Users);

                if (setupForm.ShowDialog() != DialogResult.OK)
                    return;
            }

            Application.Run(new LoginForm(services));
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                "The application could not start.\n\n" + ex.Message,
                "Startup Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }
}
