using ErasmusCourseManager.WinForms.Models;

namespace ErasmusCourseManager.WinForms.Services;

public class CourseManager
{
    private readonly AppDataStore _store;

    public CourseManager(AppDataStore store)
    {
        _store = store;
    }

    public List<Course> SearchCourses(string searchText = "")
    {
        string query = searchText.Trim();

        return _store.Courses
            .Where(course =>
                course.Code.Contains(
                    query, StringComparison.OrdinalIgnoreCase)
                || course.Name.Contains(
                    query, StringComparison.OrdinalIgnoreCase))
            .OrderBy(course => course.Code)
            .ToList();
    }

    public List<Course> GetCoursesByAcademician(int academicianId)
    {
        return _store.Courses
            .Where(course => course.AcademicianId == academicianId)
            .OrderBy(course => course.Code)
            .ToList();
    }

    public void AddCourse(
        string code,
        string name,
        int capacity,
        int ects,
        int? academicianId = null)
    {
        int nextId = _store.Courses
            .Select(course => course.Id)
            .DefaultIfEmpty(0)
            .Max() + 1;

        var course = new Course(
            nextId, code, name, capacity, ects, academicianId);

        ValidateCourse(course);

        _store.Update(data => data.Courses.Add(course));
    }

    public void UpdateCourse(Course updatedCourse)
    {
        GetRequiredCourse(updatedCourse.Id);

        ValidateCourse(updatedCourse);

        int registrationCount = _store.Registrations
            .Count(registration =>
                registration.CourseId == updatedCourse.Id);

        if (updatedCourse.Capacity < registrationCount)
        {
            throw new InvalidOperationException(
                $"Capacity cannot be lower than the current " +
                $"enrollment count ({registrationCount}).");
        }

        _store.Update(data =>
        {
            int index = data.Courses.FindIndex(
                course => course.Id == updatedCourse.Id);

            data.Courses[index] = updatedCourse;
        });
    }

    public void AssignAcademician(int courseId, int? academicianId)
    {
        Course course = GetRequiredCourse(courseId);

        var updatedCourse = new Course(
            course.Id,
            course.Code,
            course.Name,
            course.Capacity,
            course.Ects,
            academicianId);

        UpdateCourse(updatedCourse);
    }

    public void RemoveCourse(int courseId)
    {
        GetRequiredCourse(courseId);

        bool hasRegistrations = _store.Registrations
            .Any(registration => registration.CourseId == courseId);

        if (hasRegistrations)
        {
            throw new InvalidOperationException(
                "Cancel the course registrations before deleting this course.");
        }

        _store.Update(data =>
            data.Courses.RemoveAll(course => course.Id == courseId));
    }

    private Course GetRequiredCourse(int courseId)
    {
        return _store.Courses.FirstOrDefault(
            course => course.Id == courseId)
            ?? throw new InvalidOperationException("Course not found.");
    }

    private void ValidateCourse(Course course)
    {
        bool duplicateCode = _store.Courses.Any(existing =>
            existing.Id != course.Id
            && string.Equals(
                existing.Code,
                course.Code,
                StringComparison.OrdinalIgnoreCase));

        if (duplicateCode)
        {
            throw new InvalidOperationException(
                "A course with this code already exists.");
        }

        if (course.AcademicianId is int academicianId)
        {
            bool validAcademician = _store.Users.Any(user =>
                user.Id == academicianId
                && user is Academician
                && user.IsActive);

            if (!validAcademician)
            {
                throw new InvalidOperationException(
                    "Select an existing, active academician.");
            }
        }
    }
}