using DocXSwapper.Exceptions;

namespace DocXSwapper;

public static class Configuration
{
    public static string TemplatesFolder { get; private set; }
    public static string ExportsFolder { get; private set; }
    public static string SampleFilesFolder { get; private set; }
    public static string PlaceholderStart { get; private set; }
    public static string PlaceholderEnd { get; private set; }
    public static bool ExportIntoFolders { get; private set; }
    public static bool PrintReplacements { get; private set; }

    public static void PrintConfig()
    {
        Console.WriteLine("--- [CONFIG] ---");
        Console.WriteLine("[Templates] = {0}", TemplatesFolder);
        Console.WriteLine("[Exports] = {0}", ExportsFolder);
        Console.WriteLine("[Samples] = {0}", SampleFilesFolder);
        Console.WriteLine("[Export into folders] = {0}", ExportIntoFolders);
        Console.WriteLine("[Placeholder] = {0} x {1}", PlaceholderStart, PlaceholderEnd);
        Console.WriteLine("[Print replacements] = {0}", PrintReplacements);
        Console.WriteLine();
    }

    public static void Init()
    {
        var config = Discovery.LoadConfigFile("Config.conf");

        var keysToCheck = new (string, Type)[]
        {
            (ConfigKeys.Exports, typeof(string)),
            (ConfigKeys.Templates, typeof(string)),
            (ConfigKeys.SampleFiles, typeof(string)),
            (ConfigKeys.PlaceholderStart, typeof(string)),
            (ConfigKeys.PlaceholderEnd, typeof(string)),
            (ConfigKeys.ExportIntoFolders, typeof(bool)),
            (ConfigKeys.PrintReplacements, typeof(bool)),
        };

        CheckForKeys(config, keysToCheck);

        TemplatesFolder = config[ConfigKeys.Templates];
        ExportsFolder = config[ConfigKeys.Exports];
        SampleFilesFolder = config[ConfigKeys.SampleFiles];
        PlaceholderStart = config[ConfigKeys.PlaceholderStart];
        PlaceholderEnd = config[ConfigKeys.PlaceholderEnd];
        ExportIntoFolders = bool.Parse(config[ConfigKeys.ExportIntoFolders]);
        PrintReplacements = bool.Parse(config[ConfigKeys.PrintReplacements]);

        CheckDirectories([TemplatesFolder, ExportsFolder, SampleFilesFolder]);
    }

    private static void CheckForKeys(Dictionary<string, string> config, (string, Type)[] configKeys)
    {
        foreach (var (key, type) in configKeys)
        {
            if (!config.TryGetValue(key, out string value))
            {
                throw new HandledException($"Config is missing value for '{key}'.");
            }

            try
            {
                if (type == typeof(int))
                {
                    int.Parse(value);
                }
                else if (type == typeof(bool))
                {
                    bool.Parse(value);
                }
                else if (type == typeof(double))
                {
                    double.Parse(value);
                }
                else if (type == typeof(DateTime))
                {
                    DateTime.Parse(value);
                }
                else if (type == typeof(string))
                {
                    var x = value[1].ToString() != "";
                }
                else
                {
                    Convert.ChangeType(value, type);
                }
            }
            catch (Exception)
            {
                throw new HandledException($"Config value for '{key}' is invalid!");
            }
        }
    }

    private static void CheckDirectories(string[] directories)
    {
        foreach (var directoryPath in directories)
        {
            if (!Directory.Exists(directoryPath))
                throw new HandledException($"Directory path doesn't exist: {directoryPath}");
        }
    }
}

public class ConfigKeys
{
    public const string Templates = "templates_folder";
    public const string Exports = "exports_folder";
    public const string SampleFiles = "sample_files_folder";
    public const string ExportIntoFolders = "export_into_folders";
    public const string PlaceholderStart = "placeholder_start";
    public const string PlaceholderEnd = "placeholder_end";
    public const string PrintReplacements = "print_replacements";
}