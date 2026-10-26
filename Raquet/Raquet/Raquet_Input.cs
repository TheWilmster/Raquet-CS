using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using static SDL2.SDL;

namespace Raquet;

public static partial class Raquet
{
    static nint __KeysPtr = nint.Zero;
    static byte[] SDLKeys = new byte[(int)SDL_Scancode.SDL_NUM_SCANCODES];
    static byte[] PrevSDLKeys = new byte[(int)SDL_Scancode.SDL_NUM_SCANCODES];
    static byte SDLMouse;
    static byte PrevSDLMouse;

    static void __RefreshInputArr()
    {
        int size;
        nint ptr = SDL_GetKeyboardState(out size);
        Marshal.Copy(ptr, SDLKeys, 0, size);
    }
    static bool KeyCheck(SDL_Scancode key)
    {
        return SDLKeys[(uint)key] != 0;
    }

    static bool KeyCheck_Pressed(SDL_Scancode key)
    {
        
        return PrevSDLKeys[(uint)key] != SDLKeys[(uint)key] && SDLKeys[(uint)key] != 0;
    }

    static bool KeyCheck_Released(SDL_Scancode key)
    {
        return PrevSDLKeys[(uint)key] != SDLKeys[(uint)key] && SDLKeys[(uint)key] != 1;
    }

    static bool MouseCheck(uint mouse_button)
    {
        return Convert.ToBoolean(mouse_button & SDLMouse);
    }

    static bool MouseCheck_Pressed(uint mouse_button)
    {
        return (PrevSDLMouse & mouse_button) != (SDLMouse & mouse_button) && MouseCheck(mouse_button);
    }

    static bool MouseCheck_Released(uint mouse_button)
    {
        return (PrevSDLMouse & mouse_button) != (SDLMouse & mouse_button) && !MouseCheck(mouse_button);
    }
}