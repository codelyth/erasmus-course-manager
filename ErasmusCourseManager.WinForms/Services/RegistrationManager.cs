using ErasmusCourseManager.WinForms.Models;

namespace ErasmusCourseManager.WinForms.Services;

public class RegistrationManager
{
    private readonly AppDataStore _store;
    private readonly UserSession _session;

    public event EventHandler<RegistrationEventArgs>? RegistrationCreated;
    public event EventHandler<RegistrationEventArgs>? RegistrationCancelled;

    public RegistrationManager(AppDataStore store, UserSession session)
    {
        _store = store;
        _session = session;
    }

    public List<Registration> GetStudentRegistrations(int studentId)
    {
        _session.RequireStudentOrAdministrator(studentId);

        return _store.Registrations
            .Where(registration => registration.StudentId == studentId)
            .OrderByDescending(registration => registration.RegisteredAt)
            .ToList();
    }

    public List<Registration> GetCourseRegistrations(int courseId)
    {
        _session.RequireCourseAccess(courseId);

        return _store.Registrations
            .Where(registration => registration.CourseId == courseId)
            .OrderBy(registration => registration.RegisteredAt)
            .ToList();
    }

    public int GetAvailableCapacity(int courseId)
    {
        User currentUser = _session.RequireUser();

        if (currentUser is Academician)
        {
            _session.RequireCourseAccess(courseId);
        }

        Course course = GetRequiredCourse(courseId);

        int enrollmentCount = _store.Registrations.Count(
            registration => registration.CourseId == courseId);

        return course.Capacity - enrollmentCount;
    }

    public void RegisterStudent(int studentId, int courseId)
    {
        _session.RequireStudentOrAdministrator(studentId);

        bool activeStudentExists = _store.Users
            .OfType<Student>()
            .Any(student => student.Id == studentId && student.IsActive);

        if (!activeStudentExists)
        {
            throw new InvalidOperationException(
                "Select an existing, active student.");
        }

        GetRequiredCourse(courseId);

        bool alreadyRegistered = _store.Registrations.Any(
            registration =>
                registration.StudentId == studentId
                && registration.CourseId == courseId);

        if (alreadyRegistered)
        {
            throw new InvalidOperationException(
                "The student is already registered for this course.");
        }

        if (GetAvailableCapacity(courseId) <= 0)
        {
            throw new InvalidOperationException(
                "This course is full.");
        }

        int nextId = _store.Registrations
            .Select(registration => registration.Id)
            .DefaultIfEmpty(0)
            .Max() + 1;

        var registration = new Registration(
            nextId,
            studentId,
            courseId,
            DateTime.UtcNow);

        _store.Update(data => data.Registrations.Add(registration));

        RegistrationCreated?.Invoke(
            this,
            new RegistrationEventArgs(registration));
    }

    public void CancelRegistration(int studentId, int courseId)
    {
        _session.RequireStudentOrAdministrator(studentId);

        Registration registration = _store.Registrations
            .FirstOrDefault(existing =>
                existing.StudentId == studentId
                && existing.CourseId == courseId)
            ?? throw new InvalidOperationException(
                "Registration not found.");

        _store.Update(data =>
            data.Registrations.RemoveAll(
                existing => existing.Id == registration.Id));

        RegistrationCancelled?.Invoke(
            this,
            new RegistrationEventArgs(registration));
    }

    private Course GetRequiredCourse(int courseId)
    {
        return _store.Courses.FirstOrDefault(
            course => course.Id == courseId)
            ?? throw new InvalidOperationException(
                "Course not found.");
    }
}
