using ShellLink;
using ShellLink.Flags;
using System.Reflection;

namespace DesktopImageGenerator;

public static class Autostart
{
    public static void InstallAutostart()
    {
        string exeFullName = GetCurrentBinary();
        var shortcutFile = GetAutostartShortcutFileName();

        if (File.Exists(shortcutFile))
        {
            Console.WriteLine("Autostart already set up, exiting...");
            return;
        }

        var shortcut = Shortcut.CreateShortcut(
            path: exeFullName,
            args: "",
            workdir: Path.GetDirectoryName(exeFullName)!,
            iconpath: exeFullName,
            iconindex: 0
        );
        shortcut.ShowCommand = ShowCommand.SW_SHOWMINNOACTIVE;
        shortcut.WriteToFile(shortcutFile);
        Console.WriteLine("Autostart entry installed");
    }

    public static void UninstallAutostart()
    {
        var shortcutFile = GetAutostartShortcutFileName();

        if (!File.Exists(shortcutFile))
        {
            Console.WriteLine("The app has not been set to autostart");
            return;
        }

        File.Delete(shortcutFile);
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