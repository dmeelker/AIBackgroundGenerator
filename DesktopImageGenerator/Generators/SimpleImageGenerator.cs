using Microsoft.Extensions.Configuration;

namespace DesktopImageGenerator.Generators;

public class SimpleImageGenerator : IImageGenerator
{
    private readonly OpenAI _openAI;
    private IConfiguration _config;

    public SimpleImageGenerator(OpenAI openAI, IConfiguration config)
    {
        _openAI = openAI;
        _config = config;
    }

    public async Task<byte[]?> GenerateImage()
    {
        var prompt = _config["prompt"];

        if (string.IsNullOrEmpty(prompt))
        {
            throw new InvalidOperationException("Prompt is missing, place your prompt in appsettings.json to get started");
        }

        Console.WriteLine("Generating prompt...");
        var generatedPrompt = await _openAI.GeneratePrompt(prompt);
        Console.WriteLine(generatedPrompt.Prompt);

        return await _openAI.GenerateImage(generatedPrompt.Prompt);
    }
}
