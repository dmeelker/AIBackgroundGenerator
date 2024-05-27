using IWshRuntimeLibrary;
using System.Reflection;

namespace DesktopImageGenerator;

public static class Autostart
{
    public static void InstallAutostart()
    {
        var shellClass = new WshShellClass();
        string exeFullName = GetCurrentBinary();
        var shortcutFile = GetAutostartShortcutFileName();

        if (System.IO.File.Exists(shortcutFile))
        {
            Console.WriteLine("Autostart already set up, exiting...");
            return;
        }

        var shortcut = (IWshShortcut)shellClass.CreateShortcut(shortcutFile);
        shortcut.TargetPath = exeFullName;
        shortcut.IconLocation = exeFullName;
        shortcut.WorkingDirectory = Path.GetDirectoryName(exeFullName);
        shortcut.Arguments = "";
        shortcut.Description = "Generate a desktop image";
        shortcut.WindowStyle = 7; // Minimized
        shortcut.Save();

        Console.WriteLine("Autostart entry installed");
    }

    public static void UninstallAutostart()
    {
        var shortcutFile = GetAutostartShortcutFileName();

        if (!System.IO.File.Exists(shortcutFile))
        {
            Console.WriteLine("The app has not been set to autostart");
            return;
        }

        System.IO.File.Delete(shortcutFile);
        Console.WriteLine("Autostart entry removed");
    }

    private static string GetAutostartShortcutFileName()
    {
        var startupDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
        return Path.Combine(startupDirectory, "DesktopImageGenerator.lnk");
    }

    private static string GetCurrentBinary()
    {
        string exeFullName = Assembly.GetExecutingAssembly().Location;
        var directory = Path.GetDirectoryName(exeFullName)!;
        var exeName = Path.GetFileNameWithoutExtension(exeFullName) + ".exe";

        return Path.Combine(directory, exeName);
    }
}