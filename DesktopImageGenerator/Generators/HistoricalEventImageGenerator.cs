using System.Drawing;
using System.Drawing.Imaging;

namespace DesktopImageGenerator.Generators;

public class HistoricalEventImageGenerator : IImageGenerator
{
    private readonly OpenAI _openAI;

    public HistoricalEventImageGenerator(OpenAI openAI)
    {
        _openAI = openAI;
    }

    public async Task<byte[]?> GenerateImage()
    {
        Console.WriteLine("Generating prompt...");
        var prompt = await _openAI.GeneratePrompt("An interesting historical event that happened on " + DateTime.Today.ToString("MMMM d"));
        Console.WriteLine(prompt.Description);

        Console.WriteLine("Generating image...");
        var imageData = await _openAI.GenerateImage(prompt.Prompt);

        if (imageData is null)
        {
            Console.WriteLine("No image was generated, exiting...");
            return null;
        }

        var image = Image.FromStream(new MemoryStream(imageData));
        AddOverlay(image, prompt.Description);

        var stream = new MemoryStream();
        image.Save(stream, ImageFormat.Png);

        return stream.ToArray();
    }

    private void AddOverlay(Image image, string text)
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
}
