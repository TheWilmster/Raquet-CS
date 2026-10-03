using System;
using System.Collections.Generic;
using System.Text;
using SDL2;

namespace Raquet;

public static partial class Raquet
{
    public abstract partial class Program
    {
        byte[] SDLKeys = new byte[(int)SDL.SDL_Scancode.SDL_NUM_SCANCODES];
        byte[] PrevSDLKeys = new byte[(int)SDL.SDL_Scancode.SDL_NUM_SCANCODES];
        byte SDLMouse;
        byte PrevSDLMouse;

        bool KeyCheck(uint key)
        {
            return SDLKeys[key] != 0;
        }

        bool KeyCheck_Pressed(uint key)
        {
            return PrevSDLKeys[key] != SDLKeys[key] && SDLKeys[key] != 0;
        }

        bool KeyCheck_Released(uint key)
        {
            return PrevSDLKeys[key] != SDLKeys[key] && SDLKeys[key] != 1;
        }

        bool MouseCheck(uint mouse_button)
        {
            return Convert.ToBoolean(mouse_button & SDLMouse);
        }

        bool MouseCheck_Pressed(uint mouse_button)
        {
            return (PrevSDLMouse & mouse_button) != (SDLMouse & mouse_button) && MouseCheck(mouse_button);
        }

        bool MouseCheck_Released(uint mouse_button)
        {
            return (PrevSDLMouse & mouse_button) != (SDLMouse & mouse_button) && !MouseCheck(mouse_button);
        }
    }
}