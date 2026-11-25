namespace ErasmusCourseManager.WinForms.Models;

public class AppData
{
    public List<User> Users { get; init; } = new();
    public List<Course> Courses { get; init; } = new();
    public List<Registration> Registrations { get; init; } = new();
}