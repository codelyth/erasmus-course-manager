namespace ErasmusCourseManager.WinForms.Models;

public class Student : User
{
    public string StudentNumber { get; private set; }
    public string HomeUniversity { get; private set; }

    public Student(
        int id,
        string fullName,
        string email,
        string passwordHash,
        string studentNumber,
        string homeUniversity,
        bool isActive = true)
        : base(id, fullName, email, passwordHash, isActive)
    {
        if (string.IsNullOrWhiteSpace(studentNumber))
        {
            throw new ArgumentException(
                "Student number is required.",
                nameof(studentNumber));
        }

        if (string.IsNullOrWhiteSpace(homeUniversity))
        {
            throw new ArgumentException(
                "Home university is required.",
                nameof(homeUniversity));
        }

        StudentNumber = studentNumber.Trim();
        HomeUniversity = homeUniversity.Trim();
    }
}