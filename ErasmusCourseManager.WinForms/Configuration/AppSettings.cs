namespace ErasmusCourseManager.WinForms.Configuration;

public static class AppSettings
{
    public static string DataDirectory { get; } = Path.Combine(
        Environment.GetFolderPath(
            Environment.SpecialFolder.LocalApplicationData),
        "ErasmusCourseManager",
        "Data");

    public static string DataFilePath { get; } =
        Path.Combine(DataDirectory, "application-data.txt");
}