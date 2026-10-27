using System.Runtime.InteropServices;
using static SDL2.SDL_mixer;

namespace Raquet;

public static partial class Raquet
{
    public static nint LoadSound(string file)
    {
        return Mix_LoadWAV(file);
    }

    public static void PlaySound(nint wav, bool loops)
    {
        Mix_PlayChannel(-1, wav, Convert.ToInt16(loops));
    }

    public static void DestroySound(nint wav)
    {
        Mix_FreeChunk(wav);
    }
}