
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Linq;

namespace DocXSwapper;

public static class ProgramEvents
{
    public static bool MainRunning = false;
    public static void Main(string[] templates, string[] sampleFiles)
    {
        MainRunning = true;

        Console.WriteLine("");
        Console.WriteLine("[{0} templates, {1} samples]", templates.Length, sampleFiles.Length);

        foreach (var sampleFilePath in sampleFiles)
        {
            string samplePrefix = Path.GetFileName(sampleFilePath).Replace(".txt", "");
            string exportPath = Configuration.ExportsFolder;

            if (Configuration.ExportIntoFolders)
            {
                exportPath = Path.Combine(Configuration.ExportsFolder, samplePrefix);

                if (!Directory.Exists(exportPath)) Directory.CreateDirectory(exportPath);
            }

            var words = Discovery.LoadConfigFile(sampleFilePath);
            Console.WriteLine("———┬— Processing sample: {0} | {1} changes", sampleFilePath, words.Count);

            if (words.Count == 0)
            {
                Console.WriteLine("   |");
                Console.WriteLine("   └———  Sample file doesn't contain any values");
                continue;
            }

            int count = 0;
            foreach (var templatePath in templates)
            {
                var filePrefix = Configuration.ExportIntoFolders ? "" : samplePrefix + '-';
                var fileExportPath = GetDestinationFilePath(templatePath, exportPath, filePrefix);

                if (File.Exists(fileExportPath)) File.Delete(fileExportPath);

                ProcessTemplate(templatePath, words, fileExportPath);
                count++;
            }

            File.Delete(sampleFilePath);

            string plural = count > 0 ? "s" : " ";
            Console.WriteLine("   |");
            Console.WriteLine("   └——  {0} template{1} completed", count, plural);
            Console.WriteLine("");
        }

        MainRunning = false;
    }

    public static void ProcessTemplate(string templatePath, Dictionary<string, string> wordsToReplace, string exportPath)
    {
        Console.WriteLine("   |");
        Console.WriteLine($"   |————┬—  Processing template: {templatePath}");

        using WordprocessingDocument originalTemplateDoc = WordprocessingDocument.Open(templatePath, false);
        using MemoryStream memoryStream = new();
        originalTemplateDoc.Clone(memoryStream);

        using WordprocessingDocument clonedTemplateDoc = WordprocessingDocument.Open(memoryStream, true);

        MainDocumentPart mainPart = clonedTemplateDoc.MainDocumentPart;

        int changes = 0;
        Dictionary<string, bool> wordsFound = new();

        // Replace text in the main document part
        changes += ReplaceTextInPart(mainPart, wordsToReplace);

        // Replace text in all headers
        foreach (HeaderPart headerPart in mainPart.HeaderParts)
        {
            changes += ReplaceTextInPart(headerPart, wordsToReplace);
        }

        // Replace text in all footers
        foreach (FooterPart footerPart in mainPart.FooterParts)
        {
            changes += ReplaceTextInPart(footerPart, wordsToReplace);
        }

        string plural = changes > 0 ? "s" : " ";
        Console.WriteLine("   |    └————  {0} change{1} made", changes, plural);

        if (changes > 0)
        {
            clonedTemplateDoc.Save();

            using (FileStream fileStream = new(exportPath, FileMode.Create, FileAccess.Write))
            {
                memoryStream.WriteTo(fileStream);
            }
        }
    }

    private static int ReplaceTextInPart(OpenXmlPart part, Dictionary<string, string> wordsToReplace)
    {
        var texts = part.RootElement.Descendants<Text>();
        int changes = 0;

        foreach (var text in texts)
        {
            foreach (var (wordToFind, replacement) in wordsToReplace)
            {
                string templateWord = Configuration.PlaceholderStart + wordToFind + Configuration.PlaceholderEnd;

                if (text.Text.Contains(templateWord))
                {
                    text.Text = text.Text.Replace(templateWord, replacement);

                    if (Configuration.PrintReplacements)
                        Console.WriteLine("   |    |————  {0} --> {1}", wordToFind, replacement);

                    changes++;
                }
            }
        }

        return changes;
    }

    public static string GetDestinationFilePath(string originalFilePath, string destinationDirectory, string filePrefix = "")
    {
        string fileName = Path.GetFileName(originalFilePath);
        string destinationFilePath = Path.Combine(destinationDirectory, filePrefix + fileName);

        return destinationFilePath;
    }
}