namespace ErasmusCourseManager.WinForms.Models;

public class Administrator : User
{
    public Administrator(
        int id,
        string fullName,
        string email,
        string passwordHash,
        bool isActive = true)
        : base(id, fullName, email, passwordHash, isActive)
    {
    }
}