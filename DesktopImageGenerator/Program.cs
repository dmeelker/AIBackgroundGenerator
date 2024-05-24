using Microsoft.Extensions.Configuration;
using OpenAI_API;
using OpenAI_API.Chat;
using OpenAI_API.Images;
using OpenAI_API.Models;

namespace DesktopImageGenerator;

internal class Program
{
    private const string _outputDirectory = "images";

    static async Task Main(string[] args)
    {
        var config = LoadConfiguration();
        var openAI = new OpenAI(config["apiKey"]!);

        Console.WriteLine("Generating prompt...");
        var prompt = await openAI.GeneratePrompt("An interesting historical event that happened on " + DateTime.Today.ToString("MMMM d"));
        Console.WriteLine(prompt.Description);

        Console.WriteLine("Generating image...");
        var imageData = await openAI.GenerateImage(prompt.Prompt + ". In the foreground, there is a small frame with a short description: '" + prompt.Description + "', be sure the frame is entirely visible.");

        if (imageData is null)
        {
            Console.WriteLine("No image was generated, exiting...");
            return;
        }

        var fileName = new FileInfo(Path.Combine(_outputDirectory, DateTime.Today.ToString("yyyy-MM-dd") + ".png"));
        SaveImage(imageData, fileName);

        Console.WriteLine("Setting windows background...");
        WindowsUtilities.SetWallpaper(fileName.FullName);

        Console.WriteLine("Done!");
    }

    private static void SaveImage(byte[] bytes, FileInfo fileName)
    {
        Directory.CreateDirectory(_outputDirectory);
        File.WriteAllBytes(fileName.FullName, bytes);
    }

    private static IConfiguration LoadConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false);

        return builder.Build();
    }
}

public class OpenAI
{
    private OpenAIAPI _api;
    private Conversation _personConversation;

    public OpenAI(string apiKey)
    {
        _api = new OpenAIAPI(apiKey);
        _personConversation = _api.Chat.CreateConversation();
        _personConversation.Model = Model.GPT4_Turbo;
    }

    public async Task<EventInfo> GeneratePrompt(string instruction)
    {
        _personConversation.AppendUserInput("Generate a dall-e prompt for this instruction: " + instruction);
        var prompt = await _personConversation.GetResponseFromChatbotAsync();

        _personConversation.AppendUserInput("Summarize the event in max two lines");
        var description = await _personConversation.GetResponseFromChatbotAsync();

        return new EventInfo
        {
            Description = description,
            Prompt = prompt
        };
    }

    public async Task<byte[]?> GenerateImage(string prompt)
    {
        var result = await _api.ImageGenerations.CreateImageAsync(new ImageGenerationRequest
        {
            Model = Model.DALLE3,
            Size = ImageSize._1792x1024,
            Quality = "hd",
            NumOfImages = 1,
            ResponseFormat = ImageResponseFormat.B64_json,
            Prompt = prompt,
        });

        if (result.Data.Count == 0)
        {
            return null;
        }

        return Convert.FromBase64String(result.Data[0].Base64Data);
    }
}

public class EventInfo
{
    public required string Description { get; init; }
    public required string Prompt { get; init; }
}