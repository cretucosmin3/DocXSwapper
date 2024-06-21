using System.Timers;
using DocXSwapper.Exceptions;

namespace DocXSwapper
{
    public static class Program
    {
        static readonly System.Timers.Timer _timer = new(2500);

        public static void Main()
        {
            bool errors = HandleAnyErrors(() =>
            {
                Configuration.Init();
                Configuration.PrintConfig();
            });

            if (errors)
            {
                Console.WriteLine("Program exited due to errors occuring during initialization.");
                Console.ReadKey();
                return;
            };

            _timer.Elapsed += FileCheckEvent;

            _timer.AutoReset = true;
            _timer.Enabled = true;

            Console.WriteLine("Started, waiting for samples...");

            while (_timer.Enabled)
            {
                Thread.Sleep(10000);
            }
        }

        private static void FileCheckEvent(object _, ElapsedEventArgs e)
        {
            if (ProgramEvents.MainRunning) return;

            HandleAnyErrors(() =>
            {
                string[] templates = Discovery.GetTemplateFilePaths();
                string[] sampleFiles = Discovery.GetSampleFilePaths();

                if (templates.Length > 0 && sampleFiles.Length > 0)
                    ProgramEvents.Main(templates, sampleFiles);
            });
        }

        public static bool HandleAnyErrors(Action act)
        {
            try
            {
                act();
                return false;
            }
            catch (Exception ex)
            {
                if (ex.GetType() == typeof(HandledException))
                {
                    LogError("Program error: " + ex.Message);
                }
                else LogError("An unexpected error happened: " + ex.ToString());

                return true;
            }
        }

        public static void LogError(string message)
        {
            Console.Error.WriteLine(message);
            Console.Error.Flush();
        }
    }
}