using ErasmusCourseManager.WinForms.Models;

namespace ErasmusCourseManager.WinForms.Services;

public class UserSession
{
    private readonly AppDataStore _store;
    private int? _currentUserId;

    public UserSession(AppDataStore store)
    {
        _store = store;
    }

    public User? CurrentUser => _store.Users.FirstOrDefault(user =>
        user.Id == _currentUserId && user.IsActive);

    public bool IsLoggedIn => CurrentUser is not null;

    internal void SignIn(User user)
    {
        _currentUserId = user.Id;
    }

    public void SignOut()
    {
        _currentUserId = null;
    }

    public User RequireUser()
    {
        return CurrentUser
            ?? throw new UnauthorizedAccessException(
                "Please sign in to continue.");
    }

    public void RequireAdministrator()
    {
        if (RequireUser() is not Administrator)
        {
            throw new UnauthorizedAccessException(
                "Only administrators can perform this operation.");
        }
    }

    public void RequireStudentOrAdministrator(int studentId)
    {
        User user = RequireUser();

        if (user is Administrator)
            return;

        if (user is Student && user.Id == studentId)
            return;

        throw new UnauthorizedAccessException(
            "You can only manage your own registrations.");
    }

    public void RequireCourseAccess(int courseId)
    {
        User user = RequireUser();

        if (user is Administrator)
            return;

        if (user is Academician
            && _store.Courses.Any(course =>
                course.Id == courseId
                && course.AcademicianId == user.Id))
        {
            return;
        }

        throw new UnauthorizedAccessException(
            "You do not have permission to view this course's students.");
    }
}
