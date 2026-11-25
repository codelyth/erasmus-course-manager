namespace ErasmusCourseManager.WinForms.Models;

public class Registration
{
    public int Id { get; }
    public int StudentId { get; }
    public int CourseId { get; }
    public DateTime RegisteredAt { get; }

    public Registration(
        int id,
        int studentId,
        int courseId,
        DateTime registeredAt)
    {
        if (id <= 0)
            throw new ArgumentException(
                "Registration ID must be positive.", nameof(id));

        if (studentId <= 0)
            throw new ArgumentException(
                "Student ID must be positive.", nameof(studentId));

        if (courseId <= 0)
            throw new ArgumentException(
                "Course ID must be positive.", nameof(courseId));

        if (registeredAt == default)
            throw new ArgumentException(
                "Registration date is required.",
                nameof(registeredAt));

        Id = id;
        StudentId = studentId;
        CourseId = courseId;
        RegisteredAt = registeredAt;
    }
}