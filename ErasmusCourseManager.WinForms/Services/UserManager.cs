using System.Net.Mail;
using ErasmusCourseManager.WinForms.Models;

namespace ErasmusCourseManager.WinForms.Services;

public class UserManager
{
    private readonly AppDataStore _store;
    private readonly PasswordHasher _passwordHasher;

    public UserManager(
        AppDataStore store,
        PasswordHasher passwordHasher)
    {
        _store = store;
        _passwordHasher = passwordHasher;
    }

    public List<User> SearchUsers(string searchText = "")
    {
        string query = searchText.Trim();

        return _store.Users
            .Where(user =>
                user.FullName.Contains(
                    query, StringComparison.OrdinalIgnoreCase)
                || user.Email.Contains(
                    query, StringComparison.OrdinalIgnoreCase)
                || (user is Student student
                    && student.StudentNumber.Contains(
                        query, StringComparison.OrdinalIgnoreCase)))
            .OrderBy(user => user.FullName)
            .ToList();
    }

    public User GetUserById(int userId)
    {
        return _store.Users.FirstOrDefault(user => user.Id == userId)
            ?? throw new InvalidOperationException("User not found.");
    }

    public void AddStudent(
        string fullName,
        string email,
        string password,
        string studentNumber,
        string homeUniversity)
    {
        AddUser(id => new Student(
            id,
            fullName,
            email,
            _passwordHasher.Hash(password),
            studentNumber,
            homeUniversity));
    }

    public void AddAcademician(
        string fullName,
        string email,
        string password,
        string department)
    {
        AddUser(id => new Academician(
            id,
            fullName,
            email,
            _passwordHasher.Hash(password),
            department));
    }

    public void AddAdministrator(
        string fullName,
        string email,
        string password)
    {
        AddUser(id => new Administrator(
            id,
            fullName,
            email,
            _passwordHasher.Hash(password)));
    }

    public void UpdateUser(User updatedUser)
    {
        ArgumentNullException.ThrowIfNull(updatedUser);

        User existing = GetUserById(updatedUser.Id);

        if (existing.GetType() != updatedUser.GetType())
        {
            throw new InvalidOperationException(
                "An existing user's role cannot be changed.");
        }

        if (existing.PasswordHash != updatedUser.PasswordHash)
        {
            throw new InvalidOperationException(
                "Use the password change operation to change a password.");
        }

        ValidateUser(updatedUser);

        if (existing.IsActive && !updatedUser.IsActive)
        {
            EnsureAnotherActiveAdministrator(existing);
            EnsureNoAssignedCourses(existing);
        }

        _store.Update(data =>
        {
            int index = data.Users.FindIndex(
                user => user.Id == updatedUser.Id);

            data.Users[index] = updatedUser;
        });
    }

    public void ChangePassword(int userId, string newPassword)
    {
        User existing = GetUserById(userId);

        string newHash = _passwordHasher.Hash(newPassword);

        User updatedUser = existing switch
        {
            Student student => new Student(
                student.Id,
                student.FullName,
                student.Email,
                newHash,
                student.StudentNumber,
                student.HomeUniversity,
                student.IsActive),

            Academician academician => new Academician(
                academician.Id,
                academician.FullName,
                academician.Email,
                newHash,
                academician.Department,
                academician.IsActive),

            Administrator administrator => new Administrator(
                administrator.Id,
                administrator.FullName,
                administrator.Email,
                newHash,
                administrator.IsActive),

            _ => throw new InvalidOperationException(
                "Unsupported user type.")
        };

        _store.Update(data =>
        {
            int index = data.Users.FindIndex(
                user => user.Id == userId);

            data.Users[index] = updatedUser;
        });
    }

    public void RemoveUser(int userId)
    {
        User user = GetUserById(userId);

        EnsureAnotherActiveAdministrator(user);
        EnsureNoAssignedCourses(user);

        if (user is Student
            && _store.Registrations.Any(
                registration => registration.StudentId == userId))
        {
            throw new InvalidOperationException(
                "Cancel the student's registrations before deleting the user.");
        }

        _store.Update(data =>
            data.Users.RemoveAll(existing => existing.Id == userId));
    }

    private void AddUser(Func<int, User> createUser)
    {
        int nextId = _store.Users
            .Select(user => user.Id)
            .DefaultIfEmpty(0)
            .Max() + 1;

        User user = createUser(nextId);

        ValidateUser(user);

        _store.Update(data => data.Users.Add(user));
    }

    private void ValidateUser(User user)
    {
        if (!MailAddress.TryCreate(user.Email, out var address)
            || !string.Equals(
                address.Address,
                user.Email,
                StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException(
                "Enter a valid email address.");
        }

        bool duplicateEmail = _store.Users.Any(existing =>
            existing.Id != user.Id
            && string.Equals(
                existing.Email,
                user.Email,
                StringComparison.OrdinalIgnoreCase));

        if (duplicateEmail)
        {
            throw new InvalidOperationException(
                "This email address is already in use.");
        }

        if (user is Student student)
        {
            bool duplicateNumber = _store.Users
                .OfType<Student>()
                .Any(existing =>
                    existing.Id != student.Id
                    && string.Equals(
                        existing.StudentNumber,
                        student.StudentNumber,
                        StringComparison.OrdinalIgnoreCase));

            if (duplicateNumber)
            {
                throw new InvalidOperationException(
                    "This student number is already in use.");
            }
        }
    }

    private void EnsureAnotherActiveAdministrator(User user)
    {
        if (user is not Administrator || !user.IsActive)
            return;

        bool anotherExists = _store.Users
            .OfType<Administrator>()
            .Any(other => other.Id != user.Id && other.IsActive);

        if (!anotherExists)
        {
            throw new InvalidOperationException(
                "The last active administrator cannot be removed or deactivated.");
        }
    }

    private void EnsureNoAssignedCourses(User user)
    {
        if (user is Academician
            && _store.Courses.Any(
                course => course.AcademicianId == user.Id))
        {
            throw new InvalidOperationException(
                "Unassign the academician's courses before removing " +
                "or deactivating the account.");
        }
    }
}