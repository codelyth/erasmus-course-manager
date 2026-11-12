namespace ErasmusCourseManager.WinForms.Models;

public abstract class User
{
    public int Id { get; private set; }
    public string FullName { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public bool IsActive { get; private set; }

    protected User(
        int id,
        string fullName,
        string email,
        string passwordHash,
        bool isActive = true)
    {
        if (id <= 0)
            throw new ArgumentException("User ID must be positive.", nameof(id));

        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("Full name is required.", nameof(fullName));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required.", nameof(email));

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Password hash is required.", nameof(passwordHash));

        Id = id;
        FullName = fullName.Trim();
        Email = email.Trim();
        PasswordHash = passwordHash;
        IsActive = isActive;
    }
}