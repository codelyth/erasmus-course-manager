using ErasmusCourseManager.WinForms.Repositories;

namespace ErasmusCourseManager.WinForms.Services;

public class AppServices
{
    public UserSession Session { get; }
    public AuthService Auth { get; }
    public UserManager Users { get; }
    public CourseManager Courses { get; }
    public RegistrationManager Registrations { get; }

    public AppServices()
    {
        var repository = new FileRepository();
        var store = new AppDataStore(repository);
        var passwordHasher = new PasswordHasher();

        Session = new UserSession(store);

        Auth = new AuthService(store, passwordHasher, Session);

        Users = new UserManager(store, passwordHasher, Session);

        Courses = new CourseManager(store, Session);

        Registrations = new RegistrationManager(store, Session);
    }
}
