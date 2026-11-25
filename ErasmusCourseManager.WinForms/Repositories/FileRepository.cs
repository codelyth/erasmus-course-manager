using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using ErasmusCourseManager.WinForms.Configuration;
using ErasmusCourseManager.WinForms.Models;

namespace ErasmusCourseManager.WinForms.Repositories;

public class FileRepository
{
    private readonly JsonSerializerOptions _jsonOptions;

    public FileRepository()
    {
        var resolver = new DefaultJsonTypeInfoResolver();

        resolver.Modifiers.Add(typeInfo =>
        {
            if (typeInfo.Type != typeof(User))
                return;

            typeInfo.PolymorphismOptions = new JsonPolymorphismOptions
            {
                TypeDiscriminatorPropertyName = "$type",
                DerivedTypes =
                {
                    new JsonDerivedType(typeof(Student), "student"),
                    new JsonDerivedType(typeof(Academician), "academician"),
                    new JsonDerivedType(typeof(Administrator), "administrator")
                }
            };
        });

        _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            TypeInfoResolver = resolver
        };
    }

    public AppData Load()
    {
        if (!File.Exists(AppSettings.DataFilePath))
            return new AppData();

        string json = File.ReadAllText(AppSettings.DataFilePath);

        return JsonSerializer.Deserialize<AppData>(json, _jsonOptions)
            ?? throw new InvalidDataException(
                "The application data file is invalid.");
    }

    public void Save(AppData data)
    {
        ArgumentNullException.ThrowIfNull(data);

        Directory.CreateDirectory(AppSettings.DataDirectory);

        string json = JsonSerializer.Serialize(data, _jsonOptions);
        string temporaryPath = AppSettings.DataFilePath + ".tmp";

        try
        {
            File.WriteAllText(temporaryPath, json);

            if (File.Exists(AppSettings.DataFilePath))
            {
                File.Replace(
                    temporaryPath,
                    AppSettings.DataFilePath,
                    destinationBackupFileName: null);
            }
            else
            {
                File.Move(temporaryPath, AppSettings.DataFilePath);
            }
        }
        finally
        {
            if (File.Exists(temporaryPath))
                File.Delete(temporaryPath);
        }
    }
}