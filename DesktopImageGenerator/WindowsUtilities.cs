using System.Runtime.InteropServices;

namespace DesktopImageGenerator;

public static class WindowsUtilities
{
    private const int SPI_SETDESKWALLPAPER = 0x0014;
    private const int SPIF_UPDATEINIFILE = 0x01;
    private const int SPIF_SENDCHANGE = 0x02;

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

    public static void SetWallpaper(string filePath)
    {
        int result = SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, filePath, SPIF_UPDATEINIFILE | SPIF_SENDCHANGE);
        if (result == 0)
        {
            Console.WriteLine("Failed to set wallpaper.");
        }
    }
}
