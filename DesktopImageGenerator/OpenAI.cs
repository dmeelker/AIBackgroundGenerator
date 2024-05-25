using OpenAI_API;
using OpenAI_API.Chat;
using OpenAI_API.Images;
using OpenAI_API.Models;

namespace DesktopImageGenerator;

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