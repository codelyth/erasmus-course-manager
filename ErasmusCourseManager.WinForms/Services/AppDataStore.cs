using ErasmusCourseManager.WinForms.Models;
using ErasmusCourseManager.WinForms.Repositories;

namespace ErasmusCourseManager.WinForms.Services;

public class AppDataStore
{
    private readonly FileRepository _repository;
    private AppData _data;

    public IReadOnlyList<User> Users => _data.Users.AsReadOnly();

    public IReadOnlyList<Course> Courses => _data.Courses.AsReadOnly();

    public IReadOnlyList<Registration> Registrations =>
        _data.Registrations.AsReadOnly();

    public AppDataStore(FileRepository repository)
    {
        _repository = repository;
        _data = repository.Load();
    }

    internal void Update(Action<AppData> change)
    {
        var candidate = new AppData
        {
            Users = new List<User>(_data.Users),
            Courses = new List<Course>(_data.Courses),
            Registrations = new List<Registration>(_data.Registrations)
        };

        change(candidate);

        _repository.Save(candidate);

        _data = candidate;
    }
}