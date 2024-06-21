using DocXSwapper.Exceptions;

namespace DocXSwapper;

public static class Discovery
{
    public static string[] GetTemplateFilePaths()
    {
        if (!Directory.Exists(Configuration.TemplatesFolder))
            throw new HandledException($"Template folder doesn't exist at: {Configuration.TemplatesFolder}");

        return Directory.EnumerateFiles(Configuration.TemplatesFolder)
            .Where(e => e.EndsWith(".docx"))
            .ToArray();
    }

    public static string[] GetSampleFilePaths()
    {
        if (!Directory.Exists(Configuration.SampleFilesFolder))
            throw new HandledException($"Samle files folder doesn't exist at: {Configuration.SampleFilesFolder}");

        return Directory.EnumerateFiles(Configuration.SampleFilesFolder)
            .Where(e => e.EndsWith(".txt"))
            .ToArray();
    }

    public static Dictionary<string, string> LoadConfigFile(string filePath)
    {
        var configDictionary = new Dictionary<string, string>();

        if (!File.Exists(filePath))
        {
            throw new HandledException("Config file 'DocXSwapper.config' doesn't exist!");
        }

        string[] lines = File.ReadAllLines(filePath);

        foreach (string rawline in lines)
        {
            // Ignore empty lines and lines that do not contain '='
            if (string.IsNullOrWhiteSpace(rawline) || !rawline.Contains('='))
            {
                continue;
            }

            // Sanitize line - replace 
            string line = rawline.Replace(" = ", "=");
            line = line.Replace("= ", "=");
            line = line.Replace(" =", "=");

            // Split the line at the first occurrence of '='
            int index = line.IndexOf('=');
            if (index > 0 && index < line.Length - 1)
            {
                string key = line.Substring(0, index).Trim();
                string value = line.Substring(index + 1).Trim();

                // Add to the dictionary if the key is not empty
                if (!string.IsNullOrEmpty(key))
                {
                    configDictionary[key] = value;
                }
            }
        }

        return configDictionary;
    }
}