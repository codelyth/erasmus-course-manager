namespace ErasmusCourseManager.WinForms.Models;

public class RegistrationEventArgs : EventArgs
{
    public Registration Registration { get; }

    public RegistrationEventArgs(Registration registration)
    {
        Registration = registration;
    }
}