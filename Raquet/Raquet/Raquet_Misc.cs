using static SDL2.SDL;

namespace Raquet;

public static partial class Raquet
{
    static void ShowCursor(bool toggle)
    {
        SDL_ShowCursor(Convert.ToInt16(toggle));
    }

    static string AbsoluteToAsset(string relativepath)
    {
        string str = string.Empty;
        str += SDL_GetBasePath();
        str += "assets/";
        str += relativepath;
        return str;
    }
}