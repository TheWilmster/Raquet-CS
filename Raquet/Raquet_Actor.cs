using SDL2;
using System;
using System.Collections.Generic;
using System.Text;

namespace Raquet;

public static partial class Raquet
{
    public struct BoundingBox
    {
        public int x1;
        public int y1;
        public int x2;
        public int y2;
    }

    public class Actor(CHR chr)
    {
        SDL.SDL_Point position;

        CHR chr = chr;
        SDL.SDL_Point origin;
        int width;
        int height;
        int angle;

        BoundingBox bbox;

        SDL.SDL_RendererFlip flip;

        void Draw(ref Camera camera)
        {
            chr.DrawExt(position.x - camera.x, position.y - camera.y, width, height, angle, origin, flip);
        }

        bool Colliding(int x, int y, Actor collidee)
        {
            return (x - position.x + bbox.x2)
        }
    }
}