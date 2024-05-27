using DesktopImageGenerator.Generators;
using Microsoft.Extensions.Configuration;
using System.CommandLine;

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
        installCommand.SetHandler(Autostart.InstallAutostart);
        uninstallCommand.SetHandler(Autostart.UninstallAutostart);

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

        var imageGenerator = new HistoricalEventImageGenerator(new OpenAI(apiKey));

        var openAI = new OpenAI(apiKey);
        var fileName = GetTodaysFileName();

#if !DEBUG
        if (fileName.Exists)
        {
            Console.WriteLine("Today's image already exists, exiting...");
            return;
        }
#endif

        var imageData = await imageGenerator.GenerateImage();

        if (imageData is null)
        {
            Console.WriteLine("No image was generated, exiting...");
            return;
        }

        SaveImage(imageData, fileName);

        Console.WriteLine("Setting windows background...");
        WindowsUtilities.SetWallpaper(fileName.FullName);

        Console.WriteLine("Done!");
    }

    private static FileInfo GetTodaysFileName()
    {
        return new FileInfo(Path.Combine(_outputDirectory, DateTime.Today.ToString("yyyy-MM-dd") + ".png"));
    }

    private static void SaveImage(byte[] imageData, FileInfo fileName)
    {
        Directory.CreateDirectory(_outputDirectory);
        File.WriteAllBytes(fileName.FullName, imageData);
    }

    private static IConfiguration LoadConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false);

        return builder.Build();
    }
}
