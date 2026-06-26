namespace DQ5SaveEditor;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main(string[] args)
    {
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();

        // Optional: a save-state path on the command line is opened on startup.
        string? initialFile = args.Length > 0 && File.Exists(args[0]) ? args[0] : null;
        Application.Run(new MainForm(initialFile));
    }
}