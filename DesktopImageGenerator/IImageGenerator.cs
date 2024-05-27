namespace DesktopImageGenerator;

public interface IImageGenerator
{
    Task<byte[]?> GenerateImage();
}
