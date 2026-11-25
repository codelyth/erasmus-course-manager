using ErasmusCourseManager.WinForms.Models;

namespace ErasmusCourseManager.WinForms.Services;

public class AuthService
{
    private readonly AppDataStore _store;
    private readonly PasswordHasher _passwordHasher;
    private readonly UserSession _session;

    public AuthService(
        AppDataStore store,
        PasswordHasher passwordHasher,
        UserSession session)
    {
        _store = store;
        _passwordHasher = passwordHasher;
        _session = session;
    }

    public User LoginWithPassword(string email, string password)
    {
        _session.SignOut();

        if (string.IsNullOrWhiteSpace(email)
            || string.IsNullOrWhiteSpace(password))
        {
            throw new InvalidOperationException(
                "Enter your email and password.");
        }

        User? user = _store.Users.FirstOrDefault(existing =>
            string.Equals(
                existing.Email,
                email.Trim(),
                StringComparison.OrdinalIgnoreCase));

        if (user is null)
        {
            throw new InvalidOperationException(
                "Invalid email or password.");
        }

        bool passwordMatches = _passwordHasher.Verify(
            password, user.PasswordHash);

        if (!passwordMatches || !user.IsActive)
        {
            throw new InvalidOperationException(
                "Invalid email or password.");
        }

        _session.SignIn(user);

        return user;
    }

    public void Logout()
    {
        _session.SignOut();
    }
}