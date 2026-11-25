namespace ErasmusCourseManager.WinForms.Models;

public class Course
{
    public int Id { get; }
    public string Code { get; }
    public string Name { get; }
    public int Capacity { get; }
    public int Ects { get; }
    public int? AcademicianId { get; }

    public Course(
        int id,
        string code,
        string name,
        int capacity,
        int ects,
        int? academicianId = null)
    {
        if (id <= 0)
            throw new ArgumentException(
                "Course ID must be positive.", nameof(id));

        if (string.IsNullOrWhiteSpace(code))
            throw new ArgumentException(
                "Course code is required.", nameof(code));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException(
                "Course name is required.", nameof(name));

        if (capacity <= 0)
            throw new ArgumentException(
                "Capacity must be positive.", nameof(capacity));

        if (ects <= 0)
            throw new ArgumentException(
                "ECTS must be positive.", nameof(ects));

        if (academicianId.HasValue && academicianId.Value <= 0)
            throw new ArgumentException(
                "Academician ID must be positive.",
                nameof(academicianId));

        Id = id;
        Code = code.Trim();
        Name = name.Trim();
        Capacity = capacity;
        Ects = ects;
        AcademicianId = academicianId;
    }
}