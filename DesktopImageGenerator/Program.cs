using IWshRuntimeLibrary;
using Microsoft.Extensions.Configuration;
using System.CommandLine;
using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection;

namespace DesktopImageGenerator;

internal class Program
{
    private const string _outputDirectory = "images";

    static async Task<int> Main(string[] args)
    {
        var rootCommand = new RootCommand("Generates a daily background image");

        var generateCommand = new Command("generate", "Generate a new desktop image");
        var installCommand = new Command("install", "Install the application to the startup folder");
        var uninstallCommand = new Command("uninstall", "Uninstall the application from the startup folder");

        rootCommand.SetHandler(GenerateImage);
        generateCommand.SetHandler(GenerateImage);
        installCommand.SetHandler(InstallAutostart);
        uninstallCommand.SetHandler(UninstallAutostart);

        rootCommand.AddCommand(generateCommand);
        rootCommand.AddCommand(installCommand);
        rootCommand.AddCommand(uninstallCommand);

        return await rootCommand.InvokeAsync(args);
    }

    private static async Task GenerateImage()
    {
        var config = LoadConfiguration();
        var apiKey = config["apiKey"];

        if (string.IsNullOrEmpty(apiKey))
        {
            Console.WriteLine("API key is missing, place your OpenAI api key in appsettings.json to get started");
            return;
        }

        var openAI = new OpenAI(apiKey);
        var fileName = GetTodaysFileName();

#if !DEBUG
        if (fileName.Exists)
        {
            Console.WriteLine("Today's image already exists, exiting...");
            return;
        }
#endif

        Console.WriteLine("Generating prompt...");
        var prompt = await openAI.GeneratePrompt("An interesting historical event that happened on " + DateTime.Today.ToString("MMMM d"));
        Console.WriteLine(prompt.Description);

        Console.WriteLine("Generating image...");
        var imageData = await openAI.GenerateImage(prompt.Prompt);

        if (imageData is null)
        {
            Console.WriteLine("No image was generated, exiting...");
            return;
        }

        var image = Image.FromStream(new MemoryStream(imageData));
        AddOverlay(image, prompt.Description);
        SaveImage(image, fileName);

        Console.WriteLine("Setting windows background...");
        WindowsUtilities.SetWallpaper(fileName.FullName);

        Console.WriteLine("Done!");
    }

    private static void AddOverlay(Image image, string text)
    {
        using Graphics graphics = Graphics.FromImage(image);
        using Font font = new Font("Arial", 10);

        var rect = new RectangleF(20, 20, 500, 100);
        using var brush = new SolidBrush(Color.FromArgb(128, Color.White));
        graphics.FillRectangle(brush, rect);
        graphics.DrawString(text, font, Brushes.Black, rect, new StringFormat()
        {
            Alignment = StringAlignment.Center,
            LineAlignment = StringAlignment.Center,
        });
    }

    private static FileInfo GetTodaysFileName()
    {
        return new FileInfo(Path.Combine(_outputDirectory, DateTime.Today.ToString("yyyy-MM-dd") + ".png"));
    }

    private static void SaveImage(Image image, FileInfo fileName)
    {
        Directory.CreateDirectory(_outputDirectory);
        image.Save(fileName.FullName, ImageFormat.Png);
    }

    private static void InstallAutostart()
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

    private static void UninstallAutostart()
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

    private static IConfiguration LoadConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false);

        return builder.Build();
    }
}