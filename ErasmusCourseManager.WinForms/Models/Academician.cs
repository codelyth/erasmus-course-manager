namespace ErasmusCourseManager.WinForms.Models;

public class Academician : User
{
    public string Department { get; private set; }

    public Academician(
        int id,
        string fullName,
        string email,
        string passwordHash,
        string department,
        bool isActive = true)
        : base(id, fullName, email, passwordHash, isActive)
    {
        if (string.IsNullOrWhiteSpace(department))
        {
            throw new ArgumentException(
                "Department is required.",
                nameof(department));
        }

        Department = department.Trim();
    }
}