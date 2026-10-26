using System;
using System.Collections.Generic;
using System.Text;
using static SDL2.SDL;

namespace Raquet;
public static partial class Raquet
{

    public static Camera camera = new Camera(0, 0);

    static void SetPalette(ref uint[] dest, uint pal1, uint pal2, uint pal3, uint pal4)
    {
        dest[0] = pal1;
        dest[1] = pal2;
        dest[2] = pal3;
        dest[3] = pal4;
    }

    static void CopyPalette(ref uint[] dest, ref uint[] origin)
    {
        dest[0] = origin[0];
        dest[1] = origin[1];
        dest[2] = origin[2];
        dest[3] = origin[3];
    }

    static void SetDrawColor(uint pal, byte alpha)
    {
        uint palr = (pal >> 24) & 0x000000FF;
        uint palg = (pal >> 16) & 0x000000FF;
        uint palb = (pal >> 8) & 0x000000FF;
        SDL_SetRenderDrawColor(Renderer, (byte)palr, (byte)palg, (byte)palb, alpha);
    }

    public static uint[] gClearColor = new uint[4];

    static void Clear(uint pal)
    {
        SetDrawColor(pal, 255);
        SDL_RenderFillRect(Renderer, nint.Zero);

        if (Settings.BACKGROUND_CLEARING_COLOR)
            ClearColor = pal;
    }

    static void DrawRectangle(int x1, int y1, int width, int height, uint pal, byte alpha, bool fill)
    {
        SDL_Rect rect = new()
        {
            x = x1,
            y = y1,
            w = width,
            h = height
        };
        SetDrawColor(pal, alpha);

        if (fill)
            SDL_RenderFillRect(Renderer, ref rect);
        else
            SDL_RenderDrawRect(Renderer, ref rect);
    }

    static void DrawPoint(int x, int y, uint pal, byte alpha)
    {
        SetDrawColor(pal, alpha);
        SDL_RenderDrawPoint(Renderer, x, y);
    }

    static void DrawLine(int x1, int y1, int x2, int y2, uint pal, byte alpha)
    {
        SetDrawColor(pal, alpha);
        SDL_RenderDrawLine(Renderer, x1, y1, x2, y2);
    }
}

public struct Camera
{
    // no reason for this to be a class
    // literally just SDL_Point/Raquet_Point but with floats

    public float x;
    public float y;

    public Camera() { }
    public Camera(float x, float y)
    {
        this.x = x;
        this.y = y;
    }
}
